using MyBeatSaberScore.APIs;
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
        private readonly MapUtil _mapUtil;
        private readonly PlayerData _playerData;
        private List<ScoreSaber.PlayerScore> _allScores;

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

            public GridItem(string key, string cover, ScoreSaber.PlayerScore score, MapUtil mapUtil)
            {
                string hash = score.leaderboard.songHash.ToLower();
                int maxScore = score.leaderboard.maxScore > 0 ? score.leaderboard.maxScore : mapUtil.GetMaxScore(hash, score.leaderboard.difficulty.difficultyRawInt);
                double acc = maxScore > 0 ? (double)score.score.modifiedScore * 100 / maxScore : 0;

                Bsr = key.ToLower();
                Cover = cover;
                Song = score.leaderboard.songName + " " + score.leaderboard.songSubName + " / " + score.leaderboard.songAuthorName + " [ " + score.leaderboard.levelAuthorName + " ]";
                TimeSet = score.score.timeSet.Length > 0 ? score.score.timeSet : "";
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
            _mapUtil = new MapUtil();
            _playerData = new PlayerData();
            _allScores = new();
            _gridItemsViewSource.Filter += new FilterEventHandler(DataGridFilter);
            xaDataGrid.ItemsSource = _gridItemsViewSource.View;
            xaTextPlayerId.Text = Config.ScoreSaberProfileId;
        }

        private void DataGridFilter(object sender, FilterEventArgs e)
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

        private List<ScoreSaber.PlayerScore> GetAllScores()
        {
            _allScores.Clear();

            // プレイ済みスコアを追加
            _allScores.AddRange(_playerData.playedMaps.Values);

            // 未プレイランク譜面を追加
            foreach (var map in _mapUtil._mapList)
            {
                map.Diffs.ForEach(diff =>
                {
                    if (diff.Ranked && !_playerData.playedRankHash.Contains(map.Hash + diff.difficultyInt))
                    {
                        var score = new ScoreSaber.PlayerScore()
                        {
                            score = new ScoreSaber.Score()
                            {
                                modifiedScore = -1
                            },
                            leaderboard = new ScoreSaber.LeaderboardInfo()
                            {
                                ranked = true,
                                songHash = map.Hash,
                                songName = map.SongName,
                                songSubName = map.SongSubName,
                                songAuthorName = map.SongAuthorName,
                                levelAuthorName = map.LevelAuthorName,
                                stars = diff.Stars,
                                coverImage = $"https://cdn.scoresaber.com/covers/{map.Hash.ToUpper()}.png",
                                difficulty = new ScoreSaber.Difficulty()
                                {
                                    gameMode = "SoloStandard",
                                    difficulty = diff.difficultyInt,
                                }
                            }
                        };
                        _allScores.Add(score);
                    }
                });
            }

            return _allScores;
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            button.IsEnabled = false;

            var profileId = xaTextPlayerId.Text;

            await Task.Run(() =>
            {
                System.Diagnostics.Debug.WriteLine(DateTime.Now.ToString("yyyy/MM/dd/ hh:mm:ss.fff tt") + " step1");
                _mapUtil.LoadLocalFile();
                _playerData.LoadLocalFile(profileId);

                System.Diagnostics.Debug.WriteLine(DateTime.Now.ToString("yyyy/MM/dd/ hh:mm:ss.fff tt") + " step2");
                _allScores = GetAllScores();
                System.Diagnostics.Debug.WriteLine(DateTime.Now.ToString("yyyy/MM/dd/ hh:mm:ss.fff tt") + " step3");

                foreach (var score in _allScores.OrderByDescending(a => a.score.timeSet))
                {
                    string key = _mapUtil.GetAlleadyKey(score.leaderboard.songHash);
                    string cover = MapUtil.GetCoverLocalPath(score);
                    xaDataGrid.Dispatcher.Invoke(() =>
                    {
                        _gridItems.Add(new GridItem(key, cover, score, _mapUtil));
                    });
                }
                System.Diagnostics.Debug.WriteLine(DateTime.Now.ToString("yyyy/MM/dd/ hh:mm:ss.fff tt") + " step4");
            });

            _gridItemsViewSource?.View.Refresh();

            button.IsEnabled = true;
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            button.IsEnabled = false;

            string t1txt = "Task1=0.00%";
            string t2txt = "Task2=0.00%";
            string t3txt = "Task3=0.00%";

            progress.Text = $"{t1txt} {t2txt} {t3txt}";

            _gridItems.Clear();

            _playerData.LoadLocalFile(xaTextPlayerId.Text);

            // スコアセイバーから最新スコアを取得
            Task t1 = Task1DownloadLatestScores((max, count) =>
            {
                progress.Dispatcher.Invoke(() =>
                {
                    t1txt = $"Task1={(double)count * 100 / max:0.00}%";
                    progress.Text = progress.Text = $"{t1txt} {t2txt} {t3txt}";
                });
            });

            // ランク譜面リストを取得
            Task t2 = Task2DownloadRankedMaps((max, count) =>
            {
                progress.Dispatcher.Invoke(() =>
                {
                    t2txt = $"Task2={(double)count * 100 / max:0.00}%";
                    progress.Text = progress.Text = $"{t1txt} {t2txt} {t3txt}";
                });
            });

            await Task.WhenAll(t1, t2);

            _allScores = GetAllScores();

            // 未取得のKey(bsr)とカバー画像はなしでGridItemを構築。
            foreach (var score in _allScores.OrderByDescending(a => a.score.timeSet))
            {
                string key = _mapUtil.GetAlleadyKey(score.leaderboard.songHash);
                string cover = MapUtil.GetCoverLocalPath(score);
                _gridItems.Add(new GridItem(key, cover, score, _mapUtil));
            }

            // 未取得のカバー画像を取得しながら逐次更新
            Task t3 = Task3DownloadUnacquiredCover((max, count) =>
            {
                progress.Dispatcher.Invoke(() =>
                {
                    t3txt = $"Task4={(double)count * 100 / max:0.00}%";
                    progress.Text = progress.Text = $"{t1txt} {t2txt} {t3txt}";
                    _gridItemsViewSource.View.Refresh();
                });
            });

            await Task.WhenAll(t3);

            _gridItemsViewSource.View.Refresh();

            Config.ScoreSaberProfileId = xaTextPlayerId.Text;

            button.IsEnabled = true;
        }

        private async Task Task1DownloadLatestScores(Action<int, int> callback)
        {
            await _playerData.DownloadLatestScores(callback);
            _playerData.SaveLocalFile();
            callback(1, 1);
        }

        private async Task Task2DownloadRankedMaps(Action<int, int> callback)
        {
            await BeatSaberScrappedData.DownlaodCombinedScrappedData();
            callback(10, 5);
            _mapUtil.LoadLocalFile();
            callback(10, 10);
        }

        private async Task Task3DownloadUnacquiredCover(Action<int, int> callback)
        {
            int count = 0;
            // カバー画像を並列で取得
            var parallelOptions = new ParallelOptions()
            {
                MaxDegreeOfParallelism = 10
            };
            await Parallel.ForEachAsync(_gridItems, parallelOptions, async (item, y) =>
            {
                string cover = await MapUtil.GetCover(item.Hash, item.CoverUrl);
                xaDataGrid.Dispatcher.Invoke(new Action(() =>
                {
                    item.Cover = MapUtil.GetCoverLocalPath(item.Hash);
                }));
                count++;
                callback(_gridItems.Count, count);
            });
            callback(1, 1);
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
