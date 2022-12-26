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
    /// PageMain.xaml の相互作用ロジック
    /// </summary>
    public partial class PageMain : Page
    {
        private static readonly log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType);

        private readonly PlayerData _playerData;
        private readonly List<ScoreSaber.PlayerScore> _allScores;

        static readonly ObservableCollection<GridItem> _gridItems = new();
        static readonly CollectionViewSource _gridItemsViewSource = new() { Source = _gridItems };
        static readonly BindingSource _bindingSource = new();

        public class FilterValue : INotifyPropertyChanged
        {
            private bool _IsShowRank = true;
            private bool _IsShowUnRank = true;
            private bool _IsShowClear = true;
            private bool _IsShowFailure = true;
            private bool _IsShowNotPlay = true;
            private bool _IsShowFullCombo = true;
            private bool _IsShowNotFullCombo = true;
            private bool _IsShowStandard = true;
            private bool _IsShowLawless = true;
            private bool _IsShowOneSaber = true;
            private bool _IsShowLightShow = true;
            private bool _IsShow90Degree = true;
            private bool _IsShow360Degree = true;
            private bool _IsShowNoArrows = true;
            private bool _IsShowEasy = true;
            private bool _IsShowNormal = true;
            private bool _IsShowHard = true;
            private bool _IsShowExpert = true;
            private bool _IsShowExpertPlus = true;
            private double _MinStar = 0.0;
            private double _MaxStar = 20.0;
            private double _MinPp = 0.0;
            private double _MaxPp = 1000.0;
            private double _MinAcc = 0.0;
            private double _MaxAcc = 101.0;
            private long _MinScoreRank = 0;
            private long _MaxScoreRank = 9999999;
            private DateTime? _DateStart;
            private DateTime? _DateEnd;
            private DateTime? _RankedDateStart;
            private DateTime? _RankedDateEnd;

            public bool IsShowRank { get { return _IsShowRank; } set { _IsShowRank = value; OnPropertyChanged(); } }
            public bool IsShowUnRank { get { return _IsShowUnRank; } set { _IsShowUnRank = value; OnPropertyChanged(); } }
            public bool IsShowClear { get { return _IsShowClear; } set { _IsShowClear = value; OnPropertyChanged(); } }
            public bool IsShowFailure { get { return _IsShowFailure; } set { _IsShowFailure = value; OnPropertyChanged(); } }
            public bool IsShowNotPlay { get { return _IsShowNotPlay; } set { _IsShowNotPlay = value; OnPropertyChanged(); } }
            public bool IsShowFullCombo { get { return _IsShowFullCombo; } set { _IsShowFullCombo = value; OnPropertyChanged(); } }
            public bool IsShowNotFullCombo { get { return _IsShowNotFullCombo; } set { _IsShowNotFullCombo = value; OnPropertyChanged(); } }
            public bool IsShowStandard { get { return _IsShowStandard; } set { _IsShowStandard = value; OnPropertyChanged(); } }
            public bool IsShowLawless { get { return _IsShowLawless; } set { _IsShowLawless = value; OnPropertyChanged(); } }
            public bool IsShowOneSaber { get { return _IsShowOneSaber; } set { _IsShowOneSaber = value; OnPropertyChanged(); } }
            public bool IsShowLightShow { get { return _IsShowLightShow; } set { _IsShowLightShow = value; OnPropertyChanged(); } }
            public bool IsShow90Degree { get { return _IsShow90Degree; } set { _IsShow90Degree = value; OnPropertyChanged(); } }
            public bool IsShow360Degree { get { return _IsShow360Degree; } set { _IsShow360Degree = value; OnPropertyChanged(); } }
            public bool IsShowNoArrows { get { return _IsShowNoArrows; } set { _IsShowNoArrows = value; OnPropertyChanged(); } }
            public bool IsShowEasy { get { return _IsShowEasy; } set { _IsShowEasy = value; OnPropertyChanged(); } }
            public bool IsShowNormal { get { return _IsShowNormal; } set { _IsShowNormal = value; OnPropertyChanged(); } }
            public bool IsShowHard { get { return _IsShowHard; } set { _IsShowHard = value; OnPropertyChanged(); } }
            public bool IsShowExpert { get { return _IsShowExpert; } set { _IsShowExpert = value; OnPropertyChanged(); } }
            public bool IsShowExpertPlus { get { return _IsShowExpertPlus; } set { _IsShowExpertPlus = value; OnPropertyChanged(); } }
            public double MinStar { get { return _MinStar; } set { _MinStar = value; OnPropertyChanged(); } }
            public double MaxStar { get { return _MaxStar; } set { _MaxStar = value; OnPropertyChanged(); } }
            public double MinPp { get { return _MinPp; } set { _MinPp = value; OnPropertyChanged(); } }
            public double MaxPp { get { return _MaxPp; } set { _MaxPp = value; OnPropertyChanged(); } }
            public double MinAcc { get { return _MinAcc; } set { _MinAcc = value; OnPropertyChanged(); } }
            public double MaxAcc { get { return _MaxAcc; } set { _MaxAcc = value; OnPropertyChanged(); } }
            public long MinScoreRank { get { return _MinScoreRank; } set { _MinScoreRank = value; OnPropertyChanged(); } }
            public long MaxScoreRank { get { return _MaxScoreRank; } set { _MaxScoreRank = value; OnPropertyChanged(); } }
            public DateTime? DateStart { get { return _DateStart; } set { _DateStart = value; OnPropertyChanged(); } }
            public DateTime? DateEnd { get { return _DateEnd; } set { _DateEnd = value; OnPropertyChanged(); } }
            public DateTime? RankedDateStart { get { return _RankedDateStart; } set { _RankedDateStart = value; OnPropertyChanged(); } }
            public DateTime? RankedDateEnd { get { return _RankedDateEnd; } set { _RankedDateEnd = value; OnPropertyChanged(); } }

            public event PropertyChangedEventHandler? PropertyChanged;

            protected void OnPropertyChanged([CallerMemberName] string? name = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
            }
        }

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
            public FilterValue _filterValue = new();

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

            /// <summary>
            /// フィルター：ランク
            /// </summary>
            public bool IsShowRank
            {
                get
                {
                    return _filterValue.IsShowRank;
                }
                set
                {
                    _filterValue.IsShowRank = value;
                    OnPropertyChanged();
                }
            }

            /// <summary>
            /// フィルター：アンランク
            /// </summary>
            public bool IsShowUnRank
            {
                get
                {
                    return _filterValue.IsShowUnRank;
                }
                set
                {
                    _filterValue.IsShowUnRank = value;
                    OnPropertyChanged();
                }
            }

            /// <summary>
            /// フィルター：Clear
            /// </summary>
            public bool IsShowClear
            {
                get
                {
                    return _filterValue.IsShowClear;
                }
                set
                {
                    _filterValue.IsShowClear = value;
                    OnPropertyChanged();
                }
            }

            /// <summary>
            /// フィルター：Failure
            /// </summary>
            public bool IsShowFailure
            {
                get
                {
                    return _filterValue.IsShowFailure;
                }
                set
                {
                    _filterValue.IsShowFailure = value;
                    OnPropertyChanged();
                }
            }

            /// <summary>
            /// フィルター：NotPlay
            /// </summary>
            public bool IsShowNotPlay
            {
                get
                {
                    return _filterValue.IsShowNotPlay;
                }
                set
                {
                    _filterValue.IsShowNotPlay = value;
                    OnPropertyChanged();
                }
            }

            /// <summary>
            /// フィルター：星
            /// </summary>
            public double MinStar
            {
                get
                {
                    return _filterValue.MinStar;
                }
                set
                {
                    _filterValue.MinStar = value;
                    OnPropertyChanged();
                }
            }

            /// <summary>
            /// フィルター：星
            /// </summary>
            public double MaxStar
            {
                get
                {
                    return _filterValue.MaxStar;
                }
                set
                {
                    _filterValue.MaxStar = value;
                    OnPropertyChanged();
                }
            }

            /// <summary>
            /// フィルター：PP
            /// </summary>
            public double MinPp
            {
                get
                {
                    return _filterValue.MinPp;
                }
                set
                {
                    _filterValue.MinPp = value;
                    OnPropertyChanged();
                }
            }

            /// <summary>
            /// フィルター：PP
            /// </summary>
            public double MaxPp
            {
                get
                {
                    return _filterValue.MaxPp;
                }
                set
                {
                    _filterValue.MaxPp = value;
                    OnPropertyChanged();
                }
            }

            /// <summary>
            /// フィルター：ACC
            /// </summary>
            public double MinAcc
            {
                get
                {
                    return _filterValue.MinAcc;
                }
                set
                {
                    _filterValue.MinAcc = value;
                    OnPropertyChanged();
                }
            }

            /// <summary>
            /// フィルター：ACC
            /// </summary>
            public double MaxAcc
            {
                get
                {
                    return _filterValue.MaxAcc;
                }
                set
                {
                    _filterValue.MaxAcc = value;
                    OnPropertyChanged();
                }
            }

            /// <summary>
            /// フィルター：MinScoreRank
            /// </summary>
            public long MinScoreRank
            {
                get
                {
                    return _filterValue.MinScoreRank;
                }
                set
                {
                    _filterValue.MinScoreRank = value;
                    OnPropertyChanged();
                }
            }

            /// <summary>
            /// フィルター：MaxScoreRank
            /// </summary>
            public long MaxScoreRank
            {
                get
                {
                    return _filterValue.MaxScoreRank;
                }
                set
                {
                    _filterValue.MaxScoreRank = value;
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

            public void OnPropertyChangeFilterValue()
            {
                OnPropertyChanged("IsShowRank");
                OnPropertyChanged("IsShowUnRank");
                OnPropertyChanged("IsShowClear");
                OnPropertyChanged("IsShowFailure");
                OnPropertyChanged("IsShowNotPlay");
                OnPropertyChanged("MinStar");
                OnPropertyChanged("MaxStar");
                OnPropertyChanged("MinPp");
                OnPropertyChanged("MaxPp");
                OnPropertyChanged("MinAcc");
                OnPropertyChanged("MaxAcc");
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
            /// 順位
            /// </summary>
            public long ScoreRank { get; set; }

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
            /// スコア更新日
            /// </summary>
            public DateTime? TimeSetDT { get; set; }

            /// <summary>
            /// Ranked Date
            /// </summary>
            public string RankedDate { get; set; }

            /// <summary>
            /// Ranked Date
            /// </summary>
            public DateTime? RankedDateDT { get; set; }

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
            /// 最新の精度 - 前回の精度
            /// </summary>
            public double AccDifference { get; set; }

            /// <summary>
            /// 初スコアかどうか
            /// </summary>
            public bool IsFirstScore { get; set; }

            /// <summary>
            /// 初クリアかどうか
            /// </summary>
            public bool IsFirstClear { get; set; }

            /// <summary>
            /// 0:スコア回数>1＆初クリア=false
            /// 1:スコア回数=1＆初クリア=false
            /// 2:スコア回数>1＆初クリア=true
            /// 3:スコア回数=1＆初クリア=true
            /// </summary>
            public int ClearStatus { get; set; }

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
            /// スコア更新回数
            /// </summary>
            public long ScoreCount { get; set; }

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
            public long MissPlusBad { get; set; }

            /// <summary>
            /// ミスカットの数
            /// </summary>
            public long Miss { get; set; }

            /// <summary>
            /// バッドカットの数
            /// </summary>
            public long Bad { get; set; }

            /// <summary>
            /// BPM
            /// </summary>
            public double Bpm { get; set; }

            /// <summary>
            /// 曲の長さ。単位秒。
            /// </summary>
            public double Duration { get; set; }

            /// <summary>
            /// Notes Jump Speed
            /// </summary>
            public double Njs { get; set; }

            /// <summary>
            /// 秒間ノーツ数。Notes per sec.
            /// </summary>
            public double Nps { get; set; }

            /// <summary>
            /// ノーツの数
            /// </summary>
            public long Notes { get; set; }

            /// <summary>
            /// ボムの数
            /// </summary>
            public long Bombs { get; set; }

            /// <summary>
            /// 壁の数
            /// </summary>
            public long Obstacles { get; set; }

            /// <summary>
            /// フルコン
            /// </summary>
            public string FullCombo { get; set; }

            /// <summary>
            /// Checked Onlyが有効なときに表示するかどうか
            /// </summary>
            public bool Selected { get; set; }

            /// <summary>
            /// ScoreSaberのリーダーボードID (ハッシュ＋難易度に対してユニークなID)
            /// </summary>
            public long LeaderbordId { get; set; }

            public GridItem(BeatSaberScrappedData.MapInfo map, ScoreSaber.PlayerScore score, PlayerData.DifficultyResults results)
            {
                string hash = score.leaderboard.songHash.ToLower();
                BeatSaberScrappedData.Difficulty diff = map.GetDifficulty(score.leaderboard.difficulty.difficultyRawInt);
                long maxScore = MapUtil.MaxScore(diff.Notes); // ScoreSaberから取得したランク譜面のMaxScoreがいくつか間違っているので常にノーツ数から計算した値を使う。アークとチェインは未対応。

                Key = map.Key;
                NumOfKey = (Key.Length > 0) ? Convert.ToInt64(Key, 16) : 0;
                ScoreRank = score.score.rank;
                Cover = MapUtil.GetCoverLocalPath(score);
                SongFullName = $"{score.leaderboard.songName} {score.leaderboard.songSubName} / {score.leaderboard.songAuthorName} [ {score.leaderboard.levelAuthorName} ]";
                SongName = score.leaderboard.songName;
                SongSubName = score.leaderboard.songSubName;
                SongAuthor = score.leaderboard.songAuthorName;
                LevelAuthor = score.leaderboard.levelAuthorName;
                if (score.score.timeSet.Length > 0)
                {
                    TimeSet = score.score.timeSet;
                    TimeSetDT = DateTime.Parse(TimeSet).ToLocalTime();
                }
                else
                {
                    TimeSet = "";
                    TimeSetDT = null;
                }
                if (diff.Char.Contains("Standard") && diff.Ranked)
                {
                    RankedDate = diff.RankedUpdateTime;
                    RankedDateDT = DateTime.Parse(RankedDate).ToLocalTime();
                }
                else
                {
                    RankedDate = "";
                    RankedDateDT = null;
                }
                GameMode = score.leaderboard.difficulty.gameMode;
                Difficulty = score.leaderboard.difficulty.difficulty;
                Stars = score.leaderboard.ranked ? diff.Stars : -1;
                ModifiedScore = score.score.modifiedScore;
                MaxScore = maxScore;
                Acc = (maxScore > 0 && score.score.modifiedScore > 0) ? (double)score.score.modifiedScore * 100 / maxScore : 0;
                AccDifference = (maxScore > 0 && results.LatestChange() > 0) ? (double)results.LatestChange() * 100 / maxScore : 0;
                IsFirstScore = results.Count == 1;
                IsFirstClear = results.IsFirstClear();
                ClearStatus = (IsFirstScore ? 1 : 0)  + (IsFirstClear ? 2 : 0);
                PP = score.leaderboard.ranked ? score.score.pp : 0;
                Modifiers = score.score.modifiers;
                ScoreCount = results.Count;
                Hash = hash;
                CoverUrl = score.leaderboard.coverImage;
                if (score.leaderboard.coverImage.Contains("steam.png"))
                {
                    CoverUrl = $"https://eu.cdn.beatsaver.com/{hash}.jpg";
                }
                Ranked = score.leaderboard.ranked;
                MissPlusBad = score.score.badCuts + score.score.missedNotes;
                Miss = score.score.missedNotes;
                Bad = score.score.badCuts;
                Bpm = map.Bpm;
                Duration = map.Duration;
                Njs = diff.Njs;
                Nps = diff.Notes / map.Duration;
                Notes = diff.Notes;
                Bombs = diff.Bombs;
                Obstacles = diff.Obstacles;
                FullCombo = (score.score.fullCombo) ? "FC" : "";
                Selected = true;
                LeaderbordId = score.leaderboard.id;
            }
        }

        public PageMain()
        {
            InitializeComponent();

            MapUtil.Initialize();
            _playerData = new PlayerData();
            _allScores = new();
            _gridItemsViewSource.Filter += new FilterEventHandler(DataGridFilter);
            XaDataGrid.ItemsSource = _gridItemsViewSource.View;
            XaProfileId.Text = Config.ScoreSaberProfileId;
            DataContext = _bindingSource;
            Application.Current.Properties["FilterValue"] = _bindingSource._filterValue;
            Application.Current.Properties["XaDataGrid"] = XaDataGrid;
        }

        private void RefreshGrid()
        {
            try
            {
                XaDataGrid?.CommitEdit(DataGridEditingUnit.Row, true);
                _gridItemsViewSource?.View?.Refresh();
            }
            catch (Exception ex)
            {
                // 強制終了よりはましそうなので例外を握りつぶす
                _logger.Debug(ex.ToString());
            }
        }

        /// <summary>
        /// itemが曲名、BSR、HASHの指定がすべて満たしているかどうか。
        /// </summary>
        /// <param name="item"></param>
        /// <returns>曲名、BSR、HASHの指定をすべて満たしたitemである場合にtrue</returns>
        private bool FilterBySearch(GridItem item)
        {
            if (XaSongNameFilter.Text.Length > 0)
            {
                if (!item.SongFullName.Contains(XaSongNameFilter.Text, StringComparison.OrdinalIgnoreCase))
                {
                    return false;
                }
            }

            if (XaBsrFilter.Text.Length > 0)
            {
                if (!item.Key.Contains(XaBsrFilter.Text, StringComparison.OrdinalIgnoreCase))
                {
                    return false;
                }
            }

            if (XaHashFilter.Text.Length > 0)
            {
                if (!item.Hash.Contains(XaHashFilter.Text, StringComparison.OrdinalIgnoreCase))
                {
                    return false;
                }
            }

            return true;
        }

        private bool FilterByMapGameMode(GridItem item)
        {
            switch (item.GameMode)
            {
                case "SoloStandard":
                    if (!_bindingSource._filterValue.IsShowStandard)
                    {
                        return false;
                    }
                    break;
                case "SoloLawless":
                    if (!_bindingSource._filterValue.IsShowLawless)
                    {
                        return false;
                    }
                    break;
                case "SoloOneSaber":
                    if (!_bindingSource._filterValue.IsShowOneSaber)
                    {
                        return false;
                    }
                    break;
                case "SoloLightShow":
                    if (!_bindingSource._filterValue.IsShowLightShow)
                    {
                        return false;
                    }
                    break;
                case "Solo90Degree":
                    if (!_bindingSource._filterValue.IsShow90Degree)
                    {
                        return false;
                    }
                    break;
                case "Solo360Degree":
                    if (!_bindingSource._filterValue.IsShow360Degree)
                    {
                        return false;
                    }
                    break;
                case "SoloNoArrows":
                    if (!_bindingSource._filterValue.IsShowNoArrows)
                    {
                        return false;
                    }
                    break;
                default:
                    break;
            }

            return true;
        }

        private bool FilterByMapGameDifficulty(GridItem item)
        {
            switch (item.Difficulty)
            {
                case 1:
                    if (!_bindingSource._filterValue.IsShowEasy)
                    {
                        return false;
                    }
                    break;
                case 3:
                    if (!_bindingSource._filterValue.IsShowNormal)
                    {
                        return false;
                    }
                    break;
                case 5:
                    if (!_bindingSource._filterValue.IsShowHard)
                    {
                        return false;
                    }
                    break;
                case 7:
                    if (!_bindingSource._filterValue.IsShowExpert)
                    {
                        return false;
                    }
                    break;
                case 9:
                    if (!_bindingSource._filterValue.IsShowExpertPlus)
                    {
                        return false;
                    }
                    break;
                default:
                    break;
            }

            return true;
        }

        private bool FilterByMapStatus(GridItem item)
        {
            if (item.Ranked)
            {
                return (XaCheckBoxRank.IsChecked == true);
            }
            else
            {
                return (XaCheckBoxUnRank.IsChecked == true);
            }
        }

        private bool FilterByMapStar(GridItem item)
        {
            if (item.Ranked)
            {
                return (_bindingSource.MinStar <= item.Stars && item.Stars < _bindingSource.MaxStar);
            }

            return true;
        }

        private bool FilterByMapRankedDate(GridItem item)
        {
            if (item.RankedDateDT != null)
            {
                if (_bindingSource._filterValue.RankedDateStart != null)
                {
                    if (item.RankedDateDT < _bindingSource._filterValue.RankedDateStart)
                    {
                        return false;
                    }
                }

                if (_bindingSource._filterValue.RankedDateEnd != null)
                {
                    if (item.RankedDateDT > _bindingSource._filterValue.RankedDateEnd)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private bool FilterByResultPp(GridItem item)
        {
            if (item.Ranked)
            {
                return (_bindingSource.MinPp <= item.PP && item.PP < _bindingSource.MaxPp);
            }

            return true;
        }

        private bool IsFailureByConfig(string modifiers)
        {
            if (modifiers.Length > 0)
            {
                foreach (var f in Config.Failures)
                {
                    if (modifiers.Contains(f))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private bool FilterByResultFullCombo(GridItem item)
        {
            if (item.FullCombo.Length > 0)
            {
                return _bindingSource._filterValue.IsShowFullCombo;
            }
            else
            {
                return _bindingSource._filterValue.IsShowNotFullCombo;
            }
        }

        private bool FilterByResultScoreUpdateDate(GridItem item)
        {
            if (item.TimeSetDT != null)
            {
                if (_bindingSource._filterValue.DateStart != null)
                {
                    if (item.TimeSetDT < _bindingSource._filterValue.DateStart)
                    {
                        return false;
                    }
                }

                if (_bindingSource._filterValue.DateEnd != null)
                {
                    if (item.TimeSetDT > _bindingSource._filterValue.DateEnd)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private bool FilterByResultPlayStatus(GridItem item)
        {
            if (item.ModifiedScore < 0) // スコアなし＝プレイしていない
            {
                return (XaCheckBoxNoPlayRank.IsChecked == true);
            }
            else if (IsFailureByConfig(item.Modifiers)) // モディファイに失敗相当の文字列あり＝フェイルしている
            {
                return (XaCheckBoxFail.IsChecked == true);
            }
            else // 上記以外＝クリアしている
            {
                return (XaCheckBoxClear.IsChecked == true);
            }
        }

        private bool FilterByResultAcc(GridItem item)
        {
            return (_bindingSource.MinAcc <= item.Acc && item.Acc < _bindingSource.MaxAcc);
        }

        private bool FilterByResultScoreRank(GridItem item)
        {
            return (_bindingSource.MinScoreRank <= item.ScoreRank && item.ScoreRank < _bindingSource.MaxScoreRank);
        }

        private bool FilterByOther(GridItem item)
        {
            if (XaCheckBoxCheckedOnly.IsChecked == true)
            {
                return item.Selected;
            }

            return true;
        }

        private void DataGridFilter(object sender, FilterEventArgs e)
        {
            if (e.Item is GridItem item)
            {
                e.Accepted =
                    FilterBySearch(item) &&
                    FilterByMapStatus(item) &&
                    FilterByMapGameMode(item) &&
                    FilterByMapGameDifficulty(item) &&
                    FilterByMapStar(item) &&
                    FilterByMapRankedDate(item) &&
                    FilterByResultFullCombo(item) &&
                    FilterByResultScoreUpdateDate(item) &&
                    FilterByResultPlayStatus(item) &&
                    FilterByResultAcc(item) &&
                    FilterByResultPp(item) &&
                    FilterByResultScoreRank(item) &&
                    FilterByOther(item);
            }
        }

        private void UpdateAllScores()
        {
            _allScores.Clear();

            // プレイ済みスコアを追加
            _allScores.AddRange(_playerData.playedMaps.Values);

            // 未プレイランク譜面を追加
            foreach (var map in MapUtil._mapList)
            {
                map.Diffs.ForEach(diff =>
                {
                    if (diff.Ranked && diff.Char.Contains("Standard") && !_playerData.playedRankHash.Contains(map.Hash + diff.difficultyInt))
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
                        score.Normalize();
                        _allScores.Add(score);
                    }
                });
            }
        }

        /// <summary>
        /// 起動時、タブ切り替えのときに呼び出される。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            XaButtonGetData.IsEnabled = false;

            _bindingSource.OnPropertyChangeFilterValue();

            if (_playerData.PlayerId.Equals(Config.ScoreSaberProfileId))
            {
                // もともと表示していたユーザを選択した場合は何もしなくてよい
            }
            else
            {
                // もともと表示していたユーザ以外を選択した場合は保持データや描画の更新が必要
                XaProfileId.Text = Config.ScoreSaberProfileId;
                _playerData.LoadLocalFile(Config.ScoreSaberProfileId);
                _bindingSource.SetPlayerProfile(_playerData.GetPlayerProfileFromLocal());

                XaDataGrid.Dispatcher.Invoke(() =>
                {
                    _gridItems.Clear();
                });

                await Task.Run(async () =>
                {
                    if (_playerData.IsExistProfile)
                    {
                        // データを取得したことがあるユーザを選択した場合は取得済みデータを表示する
                        UpdateAllScores();
                        foreach (var score in _allScores.OrderByDescending(a => a.score.timeSet))
                        {
                            BeatSaberScrappedData.MapInfo map = MapUtil.GetMapInfo(score.leaderboard.songHash);
                            XaDataGrid.Dispatcher.Invoke(() =>
                            {
                                var results = _playerData.History.GetDifficultyResults(score.leaderboard.id);
                                _gridItems.Add(new GridItem(map, score, results));
                            });
                        }
                    }
                    else
                    {
                        // データを取得したことがないユーザを選択した場合はデータ取得を行う
                        await DownaloadAndRefleshView(true);
                    }
                });
            }

            RefreshGrid();

            XaButtonGetData.IsEnabled = true;
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            XaButtonGetData.IsEnabled = false;
            Config.ScoreSaberProfileId = XaProfileId.Text;
            await DownaloadAndRefleshView(XaRadioGetModeAll.IsChecked ?? false);
            XaButtonGetData.IsEnabled = true;
        }

        private async Task DownaloadAndRefleshView(bool isGetAll)
        {
            _bindingSource.Task1Progress = 0.0;
            _bindingSource.Task2Progress = 0.0;
            _bindingSource.Task3Progress = 0.0;

            XaDataGrid.Dispatcher.Invoke(() =>
            {
                _gridItems.Clear();
            });

            _playerData.LoadLocalFile(Config.ScoreSaberProfileId);

            // スコアセイバーから最新スコアを取得
            Task downloadLatestScores = TaskDownloadLatestScores(isGetAll, (max, count) =>
            {
                _bindingSource.Task1Progress = 100.0 * count / max;
            });

            // ランク譜面リストを取得
            Task downloadRankedMaps = TaskDownloadMapList((max, count) =>
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
                BeatSaberScrappedData.MapInfo map = MapUtil.GetMapInfo(score.leaderboard.songHash);
                XaDataGrid.Dispatcher.Invoke(() =>
                {
                    var reuslts = _playerData.History.GetDifficultyResults(score.leaderboard.id);
                    _gridItems.Add(new GridItem(map, score, reuslts));
                });
            }

            // 未取得のカバー画像を取得しながら逐次表示を更新する
            Task downloadUnacquiredCover = TaskDownloadUnacquiredCover((max, count) =>
            {
                _bindingSource.Task3Progress = 100.0 * count / max;
                Dispatcher.Invoke(() =>
                {
                    RefreshGrid();
                });
            });

            await Task.WhenAll(downloadUnacquiredCover, downloadPlayerProfile);

            RefreshGrid();
        }

        private async Task TaskDownloadLatestScores(bool isGetAll, Action<int, int> callback)
        {
            _bindingSource.StatusText = "";
            var isSuccess = await _playerData.DownloadLatestScores(isGetAll, callback);
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

        private async Task TaskDownloadMapList(Action<int, int> callback)
        {
            callback(100, 10);
            await BeatSaberScrappedData.DownlaodCombinedScrappedData();
            callback(100, 80);
            MapUtil.UpdateMapListByScrappedData();
            callback(100, 100);
        }

        private async Task TaskDownloadUnacquiredCover(Action<int, int> callback)
        {
            int count = 0;
            Dictionary<string, List<GridItem>> needAcquireCovers = new();

            foreach (var item in _gridItems)
            {
                if (!MapUtil.IsExistCoverAtLocal(item.Hash))
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
                XaDataGrid.Dispatcher.Invoke(new Action(() =>
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

        private void OnFilterEnableChanged(object sender, RoutedEventArgs e)
        {
            RefreshGrid();
        }

        private void OnFilterSliderValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            RefreshGrid();
        }

        private void OnFilterTextChanged(object sender, TextChangedEventArgs e)
        {
            RefreshGrid();
        }

        private void OnClickCopyBSR(object sender, RoutedEventArgs e)
        {
            try
            {
                GridItem? obj = ((FrameworkElement)sender).DataContext as GridItem;
                Clipboard.SetData(DataFormats.Text, $"!bsr {obj?.Key}");
            }
            catch (Exception ex)
            {
                // クリップボード監視系のアプリを使っているとエラーが出やすい？
                // クリップボードにコピーできていてもエラーが出ることがあるので握りつぶす
                // コピーでてきていなくてもユーザーはクリック失敗したかなと思う程度なのて問題ない
                _logger.Debug(ex.ToString());
            }
        }

        private System.Diagnostics.Process? OpenUrl(string url)
        {
            System.Diagnostics.ProcessStartInfo pi = new()
            {
                FileName = url,
                UseShellExecute = true,
            };
            return System.Diagnostics.Process.Start(pi);
        }

        private void OnClickJumpBeatSaver(object sender, RoutedEventArgs e)
        {
            try
            {
                if (((FrameworkElement)sender).DataContext is GridItem item)
                {
                    if (item.Key.Length > 0)
                    {
                        var url = $"https://beatsaver.com/maps/{item.Key}";
                        _ = OpenUrl(url);
                    }
                    else
                    {
                        // リパブされた譜面のKeyはScrappedDataに含まれていないのでHashで検索する。
                        // 古いHashで検索するとリパブ後のページがヒットする。
                        var url = $"https://beatsaver.com/?q={item.Hash.ToLower()}";
                        _ = OpenUrl(url);
                    }
                }
            }
            catch (System.Exception ex)
            {
                _logger.Warn(ex);
            }
        }

        private async void OnClickJumpScoreSaber(object sender, RoutedEventArgs e)
        {
            try
            {
                if (((FrameworkElement)sender).DataContext is GridItem item)
                {
                    if (item.LeaderbordId > 0)
                    {
                        var url = $"https://scoresaber.com/leaderboard/{item.LeaderbordId}";
                        _ = OpenUrl(url);
                    }
                    else
                    {
                        // 未プレイランク譜面のLeaderbordIdはScrappedDataに含まれていないので取得してくる必要がある
                        var info = await ScoreSaber.GetLeaderboard(item.Hash, (int)item.Difficulty, item.GameMode);
                        var url = $"https://scoresaber.com/leaderboard/{info.difficulty.leaderboardId}";
                        _ = OpenUrl(url);
                    }
                }
            }
            catch (System.Exception ex)
            {
                _logger.Warn(ex);
            }
        }

        private void OnClickGridItemCheckBox(object sender, RoutedEventArgs e)
        {
            if (((FrameworkElement)sender).DataContext is GridItem item)
            {
                item.Selected = !item.Selected;
                RefreshGrid();
            }
        }

        private void OnClickCheckFiltered(object sender, RoutedEventArgs e)
        {
            XaButtonCheckFiltered.IsEnabled = XaButtonClearFiltered.IsEnabled = false;
            foreach (var item in XaDataGrid.Items)
            {
                if (item is GridItem gridItem)
                {
                    gridItem.Selected = true;
                }
            }
            RefreshGrid();
            XaButtonCheckFiltered.IsEnabled = XaButtonClearFiltered.IsEnabled = true;
        }

        private void OnClickClearFiltered(object sender, RoutedEventArgs e)
        {
            XaButtonCheckFiltered.IsEnabled = XaButtonClearFiltered.IsEnabled = false;
            foreach (var item in XaDataGrid.Items)
            {
                if (item is GridItem gridItem)
                {
                    gridItem.Selected = false;
                }
            }
            RefreshGrid();
            XaButtonCheckFiltered.IsEnabled = XaButtonClearFiltered.IsEnabled = true;
        }

        private void OnClickCreatePlaylist(object sender, RoutedEventArgs e)
        {
            var dialog = new SaveFileDialog
            {
                Filter = "JSONファイル(*.json)|*.json|プレイリストファイル(*.bplist)|*.bplist|全てのファイル(*.*)|*.*"
            };

            var result = dialog.ShowDialog() ?? false;

            // 保存ボタン以外が押下された場合
            if (!result)
            {
                return;
            }

            _logger.Info($"Create {dialog.FileName}, count={XaDataGrid.Items.Count}");
            var playlist = new PlayList
            {
                Title = System.IO.Path.GetFileName(dialog.FileName)
            };
            foreach (var item in XaDataGrid.Items)
            {
                if (item is GridItem i)
                {
                    playlist.AddSong(i.Key, i.Hash, i.SongName, i.LevelAuthor, i.GameMode, i.Difficulty);
                }
            }
            playlist.Save(dialog.FileName);
        }

        private void OnClickCreateCSV(object sender, RoutedEventArgs e)
        {
            var dialog = new SaveFileDialog
            {
                Filter = "CSVファイル(*.csv)|*.csv|全てのファイル(*.*)|*.*"
            };

            var result = dialog.ShowDialog() ?? false;

            // 保存ボタン以外が押下された場合
            if (!result)
            {
                return;
            }

            String delmiter = ",";
            StringBuilder sb = new();
            sb.Append("bsr").Append(delmiter);
            sb.Append("曲名").Append(delmiter);
            sb.Append("サブ曲名").Append(delmiter);
            sb.Append("曲作者").Append(delmiter);
            sb.Append("譜面作者").Append(delmiter);
            sb.Append("更新日").Append(delmiter);
            sb.Append("モード").Append(delmiter);
            sb.Append("難易度").Append(delmiter);
            sb.Append("✖").Append(delmiter);
            sb.Append("FC").Append(delmiter);
            sb.Append("スコア").Append(delmiter);
            sb.Append("精度").Append(delmiter);
            sb.Append("ミス").Append(delmiter);
            sb.Append("PP").Append(delmiter);
            sb.Append("Modifiers").Append(delmiter);
            sb.Append("Miss").Append(delmiter);
            sb.Append("Bad");
            sb.Append(Environment.NewLine);

            foreach (var item in XaDataGrid.Items)
            {
                if (item is GridItem i)
                {
                    sb.Append(i.Key).Append(delmiter);
                    sb.Append($"\"{TrimDoubleQuotationMarks(i.SongName)}\"").Append(delmiter);
                    sb.Append($"\"{TrimDoubleQuotationMarks(i.SongSubName)}\"").Append(delmiter);
                    sb.Append($"\"{TrimDoubleQuotationMarks(i.SongAuthor)}\"").Append(delmiter);
                    sb.Append($"\"{TrimDoubleQuotationMarks(i.LevelAuthor)}\"").Append(delmiter);
                    if (i.TimeSet.Length > 0)
                    {
                        sb.Append(DateTime.Parse(i.TimeSet).ToLocalTime().ToString("yyyy/MM/dd HH:mm:ss (ddd)")).Append(delmiter);
                    }
                    else
                    {
                        sb.Append("").Append(delmiter);
                    }
                    sb.Append(i.GameMode).Append(delmiter);
                    sb.Append(i.Difficulty).Append(delmiter);
                    sb.Append(i.Stars).Append(delmiter);
                    sb.Append(i.ModifiedScore).Append(delmiter);
                    sb.Append(i.Acc).Append(delmiter);
                    sb.Append(i.MissPlusBad).Append(delmiter);
                    sb.Append(i.FullCombo).Append(delmiter);
                    sb.Append(i.PP).Append(delmiter);
                    sb.Append($"\"{i.Modifiers}\"").Append(delmiter);
                    sb.Append(i.Miss).Append(delmiter);
                    sb.Append(i.Bad);
                    sb.Append(Environment.NewLine);
                }
            }

            try
            {
                using var st = dialog.OpenFile();
                using var sw = new System.IO.StreamWriter(st, Encoding.GetEncoding("UTF-8"));
                sw.Write(sb.ToString());
            }
            catch (Exception ex)
            {
                _logger.Warn(ex.ToString());
            }
        }

        private static string TrimDoubleQuotationMarks(string target)
        {
            return target.Trim(new char[] { '"' });
        }
    }
}
