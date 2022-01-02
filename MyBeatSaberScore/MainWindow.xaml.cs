using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MyBeatSaberScore
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly BeatSaviorData _beatSaviorData;
        private readonly BeatSaverData _beatSaverData;
        private readonly ScoreSaberData _scoreSaberData;
        private List<PlayerScore> _allScores;

        static ObservableCollection<GridItem> _gridItems = new();
        static CollectionViewSource _gridItemsViewSource = new() { Source = _gridItems };

        private class GridItem
        {
            public string Bsr { get; set; }
            public string Cover { get; set; }
            public string Song { get; set; }
            public string TimeSet { get; set; }
            public string GameMode { get; set; }
            public int Difficulty { get; set; }
            public double Stars { get; set; }
            public int ModifiedScore { get; set; }
            public int MaxScore { get; set; }
            public double Acc { get; set; }
            public double PP { get; set; }
            public string Modifiers { get; set; }
            public string Hash { get; set; }
            public string CoverUrl { get; set; }
            public bool Ranked { get; set; }

            public GridItem(string key, string cover, PlayerScore score, BeatSaverData bsData)
            {
                string hash = score.leaderboard.songHash.ToLower();
                int maxScore = score.leaderboard.maxScore > 0 ? score.leaderboard.maxScore : bsData.GetMaxScore(hash, score.leaderboard.difficulty.difficultyRawInt);
                double acc = maxScore > 0 ? (double)score.score.modifiedScore * 100 / maxScore : 0;

                Bsr = key.ToLower();
                Cover = cover;
                Song = score.leaderboard.songName + " " + score.leaderboard.songSubName + " / " + score.leaderboard.songAuthorName + " [ " + score.leaderboard.levelAuthorName + " ]";
                TimeSet = score.score.timeSet.Length > 0 ? score.score.timeSet : "000-00-00T00:00:00.000Z";
                GameMode = score.leaderboard.difficulty.gameMode;
                Difficulty = score.leaderboard.difficulty.difficulty;
                Stars = score.leaderboard.ranked ? score.leaderboard.stars : -1;
                ModifiedScore = score.score.modifiedScore;
                MaxScore = maxScore;
                Acc = acc;
                PP = score.leaderboard.ranked ? score.score.pp : 0;
                Modifiers = score.score.modifiers;
                Hash = hash;
                CoverUrl = score.leaderboard.coverImage;
                Ranked = score.leaderboard.ranked;
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            Config.LoadLocalFile();
            _beatSaviorData = new BeatSaviorData();
            _beatSaverData = new BeatSaverData();
            _scoreSaberData = new ScoreSaberData();
            _allScores = new();
            _gridItemsViewSource.Filter += new FilterEventHandler(ShowOnlyRankFilter);
            xaDataGrid.ItemsSource = _gridItemsViewSource.View;
            xaTextPlayerId.Text = Config.ScoreSaberProfileId;
        }

        private void ShowOnlyRankFilter(object sender, FilterEventArgs e)
        {
            if (e.Item is GridItem item)
            {
                bool cond1st = false;
                bool cond2nd = false;
                bool cond3rd = false;

                // 曲名＆曲作者＆譜面作者でフィルタリング
                {
                    if (filterTextBox.Text.Length > 0)
                    {
                        if (item.Song.Contains(filterTextBox.Text, StringComparison.OrdinalIgnoreCase))
                        {
                            cond1st = true;
                        }
                    }
                    else
                    {
                        cond1st = true;
                    }
                }

                if (cond1st)
                {
                    cond1st = false;
                    if (filterBsrBox.Text.Length > 0)
                    {
                        if (item.Bsr.Contains(filterBsrBox.Text, StringComparison.OrdinalIgnoreCase))
                        {
                            cond1st = true;
                        }
                        if (item.Hash.Contains(filterBsrBox.Text, StringComparison.OrdinalIgnoreCase))
                        {
                            cond1st = true;
                        }
                    }
                    else
                    {
                        cond1st = true;
                    }
                }

                // 譜面の種類でフィルタリング
                if (cond1st)
                {
                    if (checkBoxUnRank.IsChecked == true)
                    {
                        if (item.Stars < 0)
                        {
                            cond2nd = true;
                        }
                    }

                    if (checkBoxRank.IsChecked == true)
                    {
                        if (sliderMinStar.Value <= item.Stars && item.Stars < sliderMaxStar.Value)
                        {
                            cond2nd = true;
                        }
                    }
                }

                // プレイ結果(クリア、未クリア)でフィルタリング
                if (cond2nd)
                {
                    if (checkBoxClear.IsChecked == true)
                    {
                        if (item.ModifiedScore >= 0 && !item.Modifiers.Contains("NF"))
                        {
                            cond3rd = true;
                        }
                    }

                    if (checkBoxFail.IsChecked == true)
                    {
                        if (item.ModifiedScore >= 0 && item.Modifiers.Contains("NF"))
                        {
                            cond3rd = true;
                        }
                    }

                    if (checkBoxNoPlayRank.IsChecked == true)
                    {
                        if (item.ModifiedScore < 0)
                        {
                            cond3rd = true;
                        }
                    }
                }

                e.Accepted = cond1st && cond2nd && cond3rd;
            }
        }

        private bool TryCreateNotPlayRankScore(RankedMap map, RankedDifficulty? diff, int diffNum, out PlayerScore score)
        {
            if (diff != null)
            {
                if (!_scoreSaberData.playedRankHash.Contains(map.hash + diffNum.ToString()))
                {
                    score =  new PlayerScore()
                    {
                        score = new Score()
                        {
                            modifiedScore = -1
                        },
                        leaderboard = new LeaderboardInfo()
                        {
                            ranked = true,
                            songHash = map.hash,
                            songName = map.songName,
                            songSubName = map.songSubName,
                            songAuthorName = map.songAuthorName,
                            levelAuthorName = map.levelAuthorName,
                            stars = diff.Stars,
                            coverImage = map.coverURL,
                            difficulty = new Difficulty()
                            {
                                gameMode = "SoloStandard",
                                difficulty = diffNum,
                            }
                        }
                    };
                    return true;
                }
            }
            score = new PlayerScore();
            return false;
        }

        private List<PlayerScore> GetAllScores()
        {
            _allScores.Clear();

            // プレイ済みスコアを追加
            _allScores.AddRange(_scoreSaberData.playedMaps.Values);

            // 未プレイランク譜面を追加
            foreach (var map in _beatSaviorData.rankedMapCollection.maps)
            {
                if (TryCreateNotPlayRankScore(map, map.diffs.easy, 1, out var score))
                {
                    _allScores.Add(score);
                }
                if (TryCreateNotPlayRankScore(map, map.diffs.normal, 3, out score))
                {
                    _allScores.Add(score);
                }
                if (TryCreateNotPlayRankScore(map, map.diffs.hard, 5, out score))
                {
                    _allScores.Add(score);
                }
                if (TryCreateNotPlayRankScore(map, map.diffs.expert, 7, out score))
                {
                    _allScores.Add(score);
                }
                if (TryCreateNotPlayRankScore(map, map.diffs.expertplus, 9, out score))
                {
                    _allScores.Add(score);
                }
            }

            return _allScores;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            button.IsEnabled = false;

            _beatSaviorData.LoadLocalFile();
            _beatSaverData.LoadLocalFile();
            _scoreSaberData.LoadLocalFile(xaTextPlayerId.Text);

            _allScores = GetAllScores();

            foreach (var score in _allScores.OrderByDescending(a => a.score.timeSet))
            {
                string key = _beatSaverData.GetAlleadyKey(score.leaderboard.songHash);
                string cover = BeatSaverData.GetCoverLocalPath(score);
                _gridItems.Add(new GridItem(key, cover, score, _beatSaverData));
            }

            _gridItemsViewSource?.View.Refresh();

            button.IsEnabled = true;
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            button.IsEnabled = false;

            string t1txt = "Task1=0.00%";
            string t2txt = "Task2=0.00%";
            string t3txt = "Task3=0.00%";
            string t4txt = "Task4=0.00%";

            progress.Text = $"{t1txt} {t2txt} {t3txt} {t4txt}";

            _gridItems.Clear();

            _scoreSaberData.LoadLocalFile(xaTextPlayerId.Text);

            // スコアセイバーから最新スコアを取得
            Task t1 = Task1DownloadLatestScores(() =>
            {
                progress.Dispatcher.Invoke(() =>
                {
                    t1txt = "Task1=100.00%";
                    progress.Text = progress.Text = $"{t1txt} {t2txt} {t3txt} {t4txt}";
                });
            });

            // BeatSaviorからランク譜面リストを取得
            Task t2 = Task2DownloadRankedMaps(() =>
            {
                progress.Dispatcher.Invoke(() =>
                {
                    t2txt = "Task2=100.00%";
                    progress.Text = progress.Text = $"{t1txt} {t2txt} {t3txt} {t4txt}";
                });
            });

            await Task.WhenAll(t1, t2);

            _allScores = GetAllScores();

            // 未取得のKey(bsr)とカバー画像はなしでGridItemを構築。
            foreach (var score in _allScores.OrderByDescending(a => a.score.timeSet))
            {
                string key = _beatSaverData.GetAlleadyKey(score.leaderboard.songHash);
                string cover = BeatSaverData.GetCoverLocalPath(score);
                _gridItems.Add(new GridItem(key, cover, score, _beatSaverData));
            }

            // BeatSaverから未取得のKey(bsr)を取得しながらgridItemを逐次更新
            Task t3 = Task3DownloadUnacquiredKey((max, count) =>
            {
                progress.Dispatcher.Invoke(() =>
                {
                    t3txt = $"Task3={(double)count*100/max:0.00}%";
                    progress.Text = progress.Text = $"{t1txt} {t2txt} {t3txt} {t4txt}";
                });
            });

            // 未取得のカバー画像を取得しながら逐次更新
            Task t4 = Task4DownloadUnacquiredCover((max, count) =>
            {
                progress.Dispatcher.Invoke(() =>
                {
                    t4txt = $"Task4={(double)count * 100 / max:0.00}%";
                    progress.Text = progress.Text = $"{t1txt} {t2txt} {t3txt} {t4txt}";
                });
            });

            await Task.WhenAll(t3, t4);

            _gridItemsViewSource.View.Refresh();

            Config.ScoreSaberProfileId = xaTextPlayerId.Text;

            button.IsEnabled = true;
        }

        private async Task Task1DownloadLatestScores(Action callback)
        {
            await _scoreSaberData.DownloadLatestScores();
            _scoreSaberData.SaveLocalFile();
            callback();
        }

        private async Task Task2DownloadRankedMaps(Action callback)
        {
            await _beatSaviorData.DownloadRankedMaps();
            _beatSaviorData.SaveLocalFile();
            callback();
        }

        private async Task Task3DownloadUnacquiredKey(Action<int, int> callback)
        {
            int count = 0;
            // BeatSaverからKey(bsr)を取得
            // リクエスト過剰になるとToo Many Requestになるので並列化しない
            foreach (var item in _gridItems)
            {
                string key = await _beatSaverData.GetKey(item.Hash);
                xaDataGrid.Dispatcher.Invoke(new Action(() =>
                {
                    item.Bsr = key;
                }));
                count++;
                callback(_gridItems.Count, count);
            }
            _beatSaverData.SaveLocalFile();
        }

        private async Task Task4DownloadUnacquiredCover(Action<int, int> callback)
        {
            int count = 0;
            // カバー画像を並列で取得
            var parallelOptions = new ParallelOptions()
            {
                MaxDegreeOfParallelism = 10
            };
            await Parallel.ForEachAsync(_gridItems, parallelOptions, async (item, y) =>
            {
                string cover = await BeatSaverData.GetCover(item.Hash, item.CoverUrl);
                xaDataGrid.Dispatcher.Invoke(new Action(() =>
                {
                    item.Cover = BeatSaverData.GetCoverLocalPath(item.Hash);
                }));
                count++;
                callback(_gridItems.Count, count);
            });
        }

        private void checkBoxRank_Checked(object sender, RoutedEventArgs e)
        {
            _gridItemsViewSource?.View.Refresh();
        }

        private void checkBoxRank_Unchecked(object sender, RoutedEventArgs e)
        {
            _gridItemsViewSource?.View.Refresh();
        }

        private void checkBoxUnRank_Checked(object sender, RoutedEventArgs e)
        {
            _gridItemsViewSource?.View.Refresh();
        }

        private void checkBoxUnRank_Unchecked(object sender, RoutedEventArgs e)
        {
            _gridItemsViewSource?.View.Refresh();
        }

        private void checkBoxNoPlayRank_Checked(object sender, RoutedEventArgs e)
        {
            _gridItemsViewSource?.View.Refresh();
        }

        private void checkBoxNoPlayRank_Unchecked(object sender, RoutedEventArgs e)
        {
            _gridItemsViewSource?.View.Refresh();
        }

        private void checkBoxFail_Checked(object sender, RoutedEventArgs e)
        {
            _gridItemsViewSource?.View.Refresh();
        }

        private void checkBoxFail_Unchecked(object sender, RoutedEventArgs e)
        {
            _gridItemsViewSource?.View.Refresh();
        }

        private void sliderMinStar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            _gridItemsViewSource?.View.Refresh();
        }

        private void sliderMaxStar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            _gridItemsViewSource?.View.Refresh();
        }

        private void filterTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            _gridItemsViewSource?.View.Refresh();
        }
    }
}
