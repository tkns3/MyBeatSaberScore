using Microsoft.Win32;
using MyBeatSaberScore.APIs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
        static BindingSource _bindingSource = new();

        private class BindingSource : INotifyPropertyChanged
        {
            public string Name { get; set; }

            public string ProfilePicture { get; set; }

            public string Country { get; set; }

            public double Pp { get; set; }

            public long GlobalRank { get; set; }

            public long CountryRank { get; set; }

            public long TotalScore { get; set; }

            public long TotalRankedScore { get; set; }

            public double AverageRankedAccuracy { get; set; }

            public long TotalPlayCount { get; set; }

            public long RankedPlayCount { get; set; }

            public long ReplaysWatched { get; set; }

            private double _task1progress;
            public double Task1Progress {
                get
                {
                    return _task1progress;
                }
                set
                {
                    _task1progress = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Task1Progress)));
                } 
            }

            private double _task2progress;
            public double Task2Progress
            {
                get
                {
                    return _task2progress;
                }
                set
                {
                    _task2progress = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Task2Progress)));
                }
            }

            private double _task3progress;
            public double Task3Progress
            {
                get
                {
                    return _task3progress;
                }
                set
                {
                    _task3progress = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Task3Progress)));
                }
            }

            public BindingSource()
            {
                Name = "";
                ProfilePicture = "Resources/_404.png";
                Country = "JP";
                Pp = 0.0;
                GlobalRank = 0;
                CountryRank = 0;
                TotalScore = 0;
                TotalRankedScore = 0;
                AverageRankedAccuracy = 0.0;
                TotalPlayCount = 0;
                RankedPlayCount = 0;
                ReplaysWatched = 0;
            }

            public event PropertyChangedEventHandler? PropertyChanged;

            public void Set(ScoreSaber.PlayerProfile profile)
            {
                Name = profile.name;
                ProfilePicture = profile.profilePicture;
                Country = profile.country;
                Pp = profile.pp;
                GlobalRank = profile.rank;
                CountryRank = profile.countryRank;
                TotalScore = profile.scoreStats.totalScore;
                TotalRankedScore = profile.scoreStats.totalRankedScore;
                AverageRankedAccuracy = profile.scoreStats.averageRankedAccuracy;
                TotalPlayCount = profile.scoreStats.totalPlayCount;
                RankedPlayCount = profile.scoreStats.rankedPlayCount;
                ReplaysWatched = profile.scoreStats.replaysWatched;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Name)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ProfilePicture)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Country)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Pp)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(GlobalRank)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CountryRank)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TotalScore)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TotalRankedScore)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AverageRankedAccuracy)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TotalPlayCount)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(RankedPlayCount)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ReplaysWatched)));
            }
        }

        private class GridItem
        {
            public string Key { get; set; }
            public long NumOfKey { get; set; }
            public string Cover { get; set; }
            public string SongFullName { get; set; }
            public string SongName { get; set; }
            public string SongSubName { get; set; }
            public string SongAuthor { get; set; }
            public string LevelAuthor { get; set; }
            public string TimeSet { get; set; }
            public string GameMode { get; set; }
            public long Difficulty { get; set; }
            public double Stars { get; set; }
            public long ModifiedScore { get; set; }
            public long MaxScore { get; set; }
            public double Acc { get; set; }
            public double PP { get; set; }
            public string Modifiers { get; set; }
            public string Hash { get; set; }
            public string CoverUrl { get; set; }
            public bool Ranked { get; set; }
            public long Miss { get; set; }

            public GridItem(string key, string cover, ScoreSaber.PlayerScore score, MapUtil mapUtil)
            {
                string hash = score.leaderboard.songHash.ToLower();
                long maxScore = score.leaderboard.maxScore > 0 ? score.leaderboard.maxScore : mapUtil.GetMaxScore(hash, score.leaderboard.difficulty.difficultyRawInt);
                double acc = maxScore > 0 ? (double)score.score.modifiedScore * 100 / maxScore : 0;

                Key = key.ToLower();
                NumOfKey = (key.Length > 0) ? Convert.ToInt64(key, 16) : 0;
                Cover = cover;
                SongFullName = $"{score.leaderboard.songName} {score.leaderboard.songSubName} / {score.leaderboard.songAuthorName} [ {score.leaderboard.levelAuthorName} ]";
                SongName = score.leaderboard.songName;
                SongSubName = score.leaderboard.songSubName;
                SongAuthor = score.leaderboard.songAuthorName;
                LevelAuthor = score.leaderboard.levelAuthorName;
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
                Miss = score.score.badCuts + score.score.missedNotes;
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
            xaProfileId.Text = Config.ScoreSaberProfileId;
            DataContext = _bindingSource;
        }

        /// <summary>
        /// itemが曲名、BSR、HASHの指定がすべて満たしているかどうか。
        /// </summary>
        /// <param name="item"></param>
        /// <returns>曲名、BSR、HASHの指定をすべて満たしたitemである場合にtrue</returns>
        private bool FilterBySearch(GridItem item)
        {
            if (xaSongNameFilter.Text.Length > 0)
            {
                if (!item.SongFullName.Contains(xaSongNameFilter.Text, StringComparison.OrdinalIgnoreCase))
                {
                    return false;
                }
            }

            if (xaBsrFilter.Text.Length > 0)
            {
                if (!item.Key.Contains(xaBsrFilter.Text, StringComparison.OrdinalIgnoreCase))
                {
                    return false;
                }
            }

            if (xaHashFilter.Text.Length > 0)
            {
                if (!item.Hash.Contains(xaHashFilter.Text, StringComparison.OrdinalIgnoreCase))
                {
                    return false;
                }
            }

            return true;
        }

        private bool FilterByMapKind(GridItem item)
        {
            if (xaCheckBoxUnRank.IsChecked == true)
            {
                if (item.Stars < 0)
                {
                    return true;
                }
            }

            if (xaCheckBoxRank.IsChecked == true)
            {
                if (xaSliderMinStar.Value <= item.Stars && item.Stars < xaSliderMaxStar.Value)
                {
                    return true;
                }
            }

            return false;
        }

        private bool IsFailureByConfig(string modifiers)
        {
            foreach (var f in Config.Failures)
            {
                if (modifiers.Contains(f))
                {
                    return true;
                }
            }
            return false;
        }

        private bool FilterByResult(GridItem item)
        {
            if (xaCheckBoxClear.IsChecked == true)
            {
                if (item.ModifiedScore >= 0 && !IsFailureByConfig(item.Modifiers))
                {
                    return true;
                }
            }

            if (xaCheckBoxFail.IsChecked == true)
            {
                if (item.ModifiedScore >= 0 && IsFailureByConfig(item.Modifiers))
                {
                    return true;
                }
            }

            if (xaCheckBoxNoPlayRank.IsChecked == true)
            {
                if (item.ModifiedScore < 0)
                {
                    return true;
                }
            }

            return false;
        }

        private void DataGridFilter(object sender, FilterEventArgs e)
        {
            if (e.Item is GridItem item)
            {
                e.Accepted = FilterBySearch(item) && FilterByMapKind(item) && FilterByResult(item);
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
            xaButtonGetData.IsEnabled = false;

            var profileId = xaProfileId.Text;

            _playerData.LoadLocalFile(profileId);
            var profile = _playerData.GetPlayerProfileFromLocal();
            _bindingSource.Set(profile);

            await Task.Run(() =>
            {
                _mapUtil.LoadLocalFile();

                _allScores = GetAllScores();

                foreach (var score in _allScores.OrderByDescending(a => a.score.timeSet))
                {
                    string key = _mapUtil.GetAlleadyKey(score.leaderboard.songHash);
                    string cover = MapUtil.GetCoverLocalPath(score);
                    xaDataGrid.Dispatcher.Invoke(() =>
                    {
                        _gridItems.Add(new GridItem(key, cover, score, _mapUtil));
                    });
                }
            });

            _gridItemsViewSource?.View.Refresh();

            xaButtonGetData.IsEnabled = true;
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            xaButtonGetData.IsEnabled = false;

            _bindingSource.Task1Progress = 0.0;
            _bindingSource.Task2Progress = 0.0;
            _bindingSource.Task3Progress = 0.0;

            _gridItems.Clear();

            _playerData.LoadLocalFile(xaProfileId.Text);

            // スコアセイバーから最新スコアを取得
            Task t1 = Task1DownloadLatestScores((max, count) =>
            {
                _bindingSource.Task1Progress = (double)count * 100 / max;
            });

            // ランク譜面リストを取得
            Task t2 = Task2DownloadRankedMaps((max, count) =>
            {
                _bindingSource.Task2Progress = (double)count * 100 / max;
            });

            Task t4 = Task.Run(async () =>
            {
                var profile = await _playerData.GetPlayerProfile();
                _bindingSource.Set(profile);
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
                _bindingSource.Task3Progress = (double)count * 100 / max;
                Dispatcher.Invoke(() =>
                {
                    _gridItemsViewSource.View.Refresh();
                });
            });

            await Task.WhenAll(t3, t4);

            _gridItemsViewSource.View.Refresh();

            Config.ScoreSaberProfileId = xaProfileId.Text;

            xaButtonGetData.IsEnabled = true;
        }

        private async Task Task1DownloadLatestScores(Action<int, int> callback)
        {
            await _playerData.DownloadLatestScores(callback);
            _playerData.SaveLocalFile();
            callback(1, 1);
        }

        private async Task Task2DownloadRankedMaps(Action<int, int> callback)
        {
            callback(10, 1);
            await BeatSaberScrappedData.DownlaodCombinedScrappedData();
            callback(10, 8);
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

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            GridItem? obj = ((FrameworkElement)sender).DataContext as GridItem;
            Clipboard.SetData(DataFormats.Text, $"!bsr {obj?.Key}");
        }

        private void xaButtonCreatePlaylist_Click(object sender, RoutedEventArgs e)
        {
            // ファイル保存ダイアログを生成します。
            var dialog = new SaveFileDialog();

            // フィルターを設定します。
            // この設定は任意です。
            dialog.Filter = "JSONファイル(*.json)|*.json|プレイリストファイル(*.bplist)|*.bplist|全てのファイル(*.*)|*.*";

            // ファイル保存ダイアログを表示します。
            var result = dialog.ShowDialog() ?? false;

            // 保存ボタン以外が押下された場合
            if (!result)
            {
                // 終了します。
                return;
            }

            System.Diagnostics.Debug.WriteLine(DateTime.Now.ToString("yyyy/MM/dd/ hh:mm:ss.fff tt") + " " + xaDataGrid.Items.Count);
            var playlist = new PlayList();
            foreach (var item in xaDataGrid.Items)
            {
                if (item is GridItem i)
                {
                    playlist.AddSong(i.Key, i.Hash, i.SongName, i.LevelAuthor, i.GameMode, i.Difficulty);
                }
            }
            playlist.Save(dialog.FileName);
        }
    }
}
