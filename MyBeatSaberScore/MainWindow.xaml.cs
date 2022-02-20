using Microsoft.Win32;
using MyBeatSaberScore.APIs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
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
            private string _Name = "";
            private string _ProfilePicture = "Resources/_404.png";
            private string _Country = "JP";
            private double _Pp;
            private long _GlobalRank;
            private long _CountryRank;
            private long _TotalScore;
            private long _TotalRankedScore;
            private double _AverageRankedAccuracy;
            private long _TotalPlayCount;
            private long _RankedPlayCount;
            private long _ReplaysWatched;
            private double _Task1Progress;
            private double _Task2Progress;
            private double _Task3Progress;
            private string _StatusText = "";

            public event PropertyChangedEventHandler? PropertyChanged;

            public BindingSource()
            {
            }

            protected void OnPropertyChanged([CallerMemberName] string? name = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
            }

            /// <summary>
            /// プレイヤープロフィール：プレイヤー名
            /// </summary>
            public string Name
            {
                get
                {
                    return _Name;
                }
                set
                {
                    _Name = value;
                    OnPropertyChanged();
                }
            }

            /// <summary>
            /// プレイヤープロフィール：画像URL
            /// </summary>
            public string ProfilePicture
            {
                get
                {
                    return _ProfilePicture;
                }
                set
                {
                    _ProfilePicture = value;
                    OnPropertyChanged();
                }
            }

            /// <summary>
            /// プレイヤープロフィール：所属国
            /// </summary>
            public string Country
            {
                get
                {
                    return _Country;
                }
                set
                {
                    _Country = value;
                    OnPropertyChanged();
                }
            }

            /// <summary>
            /// プレイヤープロフィール：PP
            /// </summary>
            public double Pp
            {
                get
                {
                    return _Pp;
                }
                set
                {
                    _Pp = value;
                    OnPropertyChanged();
                }
            }

            /// <summary>
            /// プレイヤープロフィール：世界順位
            /// </summary>
            public long GlobalRank
            {
                get
                {
                    return _GlobalRank;
                }
                set
                {
                    _GlobalRank = value;
                    OnPropertyChanged();
                }
            }

            /// <summary>
            /// プレイヤープロフィール：国内順位
            /// </summary>
            public long CountryRank
            {
                get
                {
                    return _CountryRank;
                }
                set 
                {
                    _CountryRank = value;
                    OnPropertyChanged();
                }
            }

            /// <summary>
            /// プレイヤープロフィール：全譜面合計スコア
            /// </summary>
            public long TotalScore
            {
                get
                {
                    return _TotalScore;
                }
                set
                {
                    _TotalScore = value;
                    OnPropertyChanged();
                }
            }

            /// <summary>
            /// プレイヤープロフィール：ランク譜面合計スコア
            /// </summary>
            public long TotalRankedScore
            {
                get
                {
                    return _TotalRankedScore;
                }
                set
                {
                    _TotalRankedScore = value;
                    OnPropertyChanged();
                }
            }

            /// <summary>
            /// プレイヤープロフィール：ランク譜面平均精度
            /// </summary>
            public double AverageRankedAccuracy
            {
                get
                {
                    return _AverageRankedAccuracy;
                }
                set
                {
                    _AverageRankedAccuracy = value;
                    OnPropertyChanged();
                }
            }

            /// <summary>
            /// プレイヤープロフィール：スコアを送信した難易度の数
            /// </summary>
            public long TotalPlayCount
            {
                get
                {
                    return _TotalPlayCount;
                }
                set
                {
                    _TotalPlayCount = value;
                    OnPropertyChanged();
                }
            }

            /// <summary>
            /// プレイヤープロフィール：ランク譜面のスコアを送信した難易度の数
            /// </summary>
            public long RankedPlayCount
            {
                get
                {
                    return _RankedPlayCount;
                }
                set
                {
                    _RankedPlayCount = value;
                    OnPropertyChanged();
                }
            }

            /// <summary>
            /// プレイヤープロフィール：他の人に再生されたリプレイの数
            /// </summary>
            public long ReplaysWatched
            {
                get
                {
                    return _ReplaysWatched;
                }
                set
                {
                    _ReplaysWatched = value;
                    OnPropertyChanged();
                }
            }

            /// <summary>
            /// タスク１の進捗
            /// </summary>
            public double Task1Progress
            {
                get
                {
                    return _Task1Progress;
                }
                set
                {
                    _Task1Progress = value;
                    OnPropertyChanged();
                }
            }

            /// <summary>
            /// タスク２の進捗
            /// </summary>
            public double Task2Progress
            {
                get
                {
                    return _Task2Progress;
                }
                set
                {
                    _Task2Progress = value;
                    OnPropertyChanged();
                }
            }

            /// <summary>
            /// タスク３の進捗
            /// </summary>
            public double Task3Progress
            {
                get
                {
                    return _Task3Progress;
                }
                set
                {
                    _Task3Progress = value;
                    OnPropertyChanged();
                }
            }

            public string StatusText
            {
                get
                {
                    return _StatusText;
                }
                set
                {
                    _StatusText = value;
                    OnPropertyChanged();
                }
            }

            public void SetPlayerProfile(ScoreSaber.PlayerProfile profile)
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
            }
        }

        private class GridItem
        {
            /// <summary>
            /// BeatSaverのKey(bsr)。16進数のやつ。ex. "1a3fe"
            /// </summary>
            public string Key { get; set; }

            /// <summary>
            /// Keyを10進数になおしたやつ。
            /// </summary>
            public long NumOfKey { get; set; }

            /// <summary>
            /// ローカルに保存したカバー画像のパス。
            /// </summary>
            public string Cover { get; set; }

            /// <summary>
            /// 曲名、曲作者、譜面作者を合成した文字列
            /// </summary>
            public string SongFullName { get; set; }

            /// <summary>
            /// 曲名
            /// </summary>
            public string SongName { get; set; }

            /// <summary>
            /// 曲サブ名
            /// </summary>
            public string SongSubName { get; set; }

            /// <summary>
            /// 曲作者
            /// </summary>
            public string SongAuthor { get; set; }

            /// <summary>
            /// 譜面作者
            /// </summary>
            public string LevelAuthor { get; set; }

            /// <summary>
            /// スコア更新日
            /// </summary>
            public string TimeSet { get; set; }

            /// <summary>
            /// SoloStandard, SoloLawless, SoloOneSaber, SoloLightShow, Solo90Degree, Solo360Degree, SoloNoArrows
            /// </summary>
            public string GameMode { get; set; }

            /// <summary>
            /// 1:Easy, 3:Normal, 5:Hard, 7:Expert 9:ExpertPlus
            /// </summary>
            public long Difficulty { get; set; }

            /// <summary>
            /// ランク譜面の星。ランク譜面以外は-1。
            /// </summary>
            public double Stars { get; set; }

            /// <summary>
            /// スコア
            /// </summary>
            public long ModifiedScore { get; set; }

            /// <summary>
            /// 最大スコア
            /// </summary>
            public long MaxScore { get; set; }

            /// <summary>
            /// 精度=100*スコア/最大スコア
            /// </summary>
            public double Acc { get; set; }

            /// <summary>
            /// PP。ランク譜面以外は0。
            /// </summary>
            public double PP { get; set; }

            /// <summary>
            /// [[[modifire],[modifirs],...]
            /// 略称を把握済みのmodifireは以下
            /// BE:Battery Energy(4 Lives).
            /// DA:Disappearing Arrows.
            /// FS:Faster Song.
            /// GN:Ghost Notes.
            /// IF:Insta Fail(1 Life).
            /// NA:No Arrows.
            /// NB:No Bombs.
            /// NF:No Fail.
            /// NO:No Obstacles(No Walls).
            /// PM:Pro Mode.
            /// SF:Super Fast Song.
            /// SS:Slower Song.
            /// 略称を未確認のmodifireは以下
            /// Small Notes, Zen Mode
            /// </summary>
            public string Modifiers { get; set; }

            /// <summary>
            /// BeatSaverのハッシュ
            /// </summary>
            public string Hash { get; set; }

            /// <summary>
            /// カバー画像のURL
            /// </summary>
            public string CoverUrl { get; set; }

            /// <summary>
            /// ランク譜面かどうか
            /// </summary>
            public bool Ranked { get; set; }

            /// <summary>
            /// ミス＋バッドカットの数
            /// </summary>
            public long Miss { get; set; }

            public GridItem(BeatSaberScrappedData.MapInfo map, ScoreSaber.PlayerScore score)
            {
                string hash = score.leaderboard.songHash.ToLower();
                BeatSaberScrappedData.Difficulty diff = map.GetDifficulty(score.leaderboard.difficulty.difficultyRawInt);
                long maxScore = score.leaderboard.maxScore > 0 ? score.leaderboard.maxScore : diff.GetMaxScore();
                double acc = maxScore > 0 ? (double)score.score.modifiedScore * 100 / maxScore : 0;

                Key = map.Key;
                NumOfKey = (Key.Length > 0) ? Convert.ToInt64(Key, 16) : 0;
                Cover = MapUtil.GetCoverLocalPath(score);
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

        private void UpdateAllScores()
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
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            xaButtonGetData.IsEnabled = false;

            var profileId = xaProfileId.Text;

            _playerData.LoadLocalFile(profileId);
            var profile = _playerData.GetPlayerProfileFromLocal();
            _bindingSource.SetPlayerProfile(profile);

            await Task.Run(() =>
            {
                _mapUtil.LoadLocalFile();

                UpdateAllScores();

                foreach (var score in _allScores.OrderByDescending(a => a.score.timeSet))
                {
                    BeatSaberScrappedData.MapInfo map = _mapUtil.GetMapInfo(score.leaderboard.songHash);
                    xaDataGrid.Dispatcher.Invoke(() =>
                    {
                        _gridItems.Add(new GridItem(map, score));
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
            Task downloadLatestScores = TaskDownloadLatestScores((max, count) =>
            {
                _bindingSource.Task1Progress = 100.0 * count / max;
            });

            // ランク譜面リストを取得
            Task downloadRankedMaps = TaskDownloadRankedMaps((max, count) =>
            {
                _bindingSource.Task2Progress = 100.0 * count / max;
            });

            // スコアセイバーからプレイヤー情報を取得して表示を更新する
            Task downloadPlayerProfile = Task.Run(async () =>
            {
                var profile = await _playerData.GetPlayerProfile();
                _bindingSource.SetPlayerProfile(profile);
            });

            await Task.WhenAll(downloadLatestScores, downloadRankedMaps);

            UpdateAllScores();

            // GridItemを構築。未取得のカバー画像は後で取得する。
            foreach (var score in _allScores.OrderByDescending(a => a.score.timeSet))
            {
                BeatSaberScrappedData.MapInfo map = _mapUtil.GetMapInfo(score.leaderboard.songHash);
                _gridItems.Add(new GridItem(map, score));
            }

            // 未取得のカバー画像を取得しながら逐次表示を更新する
            Task downloadUnacquiredCover = TaskDownloadUnacquiredCover((max, count) =>
            {
                _bindingSource.Task3Progress = 100.0 * count / max;
                Dispatcher.Invoke(() =>
                {
                    _gridItemsViewSource.View.Refresh();
                });
            });

            await Task.WhenAll(downloadUnacquiredCover, downloadPlayerProfile);

            _gridItemsViewSource.View.Refresh();

            Config.ScoreSaberProfileId = xaProfileId.Text;

            xaButtonGetData.IsEnabled = true;
        }

        private async Task TaskDownloadLatestScores(Action<int, int> callback)
        {
            _bindingSource.StatusText = "";
            var isSuccess = await _playerData.DownloadLatestScores(callback);
            if (isSuccess)
            {
                _playerData.SaveLocalFile();
            }
            else
            {
                _bindingSource.StatusText = "最新スコアの取得に失敗しました";
            }
            callback(1, 1);
        }

        private async Task TaskDownloadRankedMaps(Action<int, int> callback)
        {
            callback(100, 10);
            await BeatSaberScrappedData.DownlaodCombinedScrappedData();
            callback(100, 80);
            _mapUtil.LoadLocalFile();
            callback(100, 100);
        }

        private async Task TaskDownloadUnacquiredCover(Action<int, int> callback)
        {
            int count = 0;
            Dictionary<string, List<GridItem>> needAcquireCovers = new();

            foreach (var item in _gridItems)
            {
                if (!MapUtil.isExistCoverAtLocal(item.Hash))
                {
                    if (!needAcquireCovers.ContainsKey(item.Hash))
                    {
                        needAcquireCovers[item.Hash] = new();
                    }
                    needAcquireCovers[item.Hash].Add(item);
                }
            }

            // カバー画像を並列で取得
            var parallelOptions = new ParallelOptions()
            {
                MaxDegreeOfParallelism = 10
            };
            await Parallel.ForEachAsync(needAcquireCovers, parallelOptions, async (cover, y) =>
            {
                _ = await MapUtil.GetCover(cover.Key, cover.Value.First().CoverUrl);
                xaDataGrid.Dispatcher.Invoke(new Action(() =>
                {
                    cover.Value.ForEach(item =>
                    {
                        item.Cover = MapUtil.GetCoverLocalPath(item.Hash);
                    });
                }));
                count++;
                callback(needAcquireCovers.Count, count);
            });
            callback(100, 100);
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
