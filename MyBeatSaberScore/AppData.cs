using MyBeatSaberScore.Model;
using System;
using System.Windows.Controls;

namespace MyBeatSaberScore
{
    internal static class AppData
    {
        public static bool IsFirst { get; set; } = true;

        public static UserData SelectedUser { get; set; } = new();

        public static TabItem? XaTabMain { get; set; }

        public static FilterValue FilterValue { get; set; } = new();

        public static ViewTarget ViewTarget { get => Config.ViewTarget; set => Config.ViewTarget = value; }
    }

    [Flags]
    public enum ViewTarget
    {
        None = 0x00,
        BeatLeader = 0x01,
        ScoreSaber = 0x02,
    }

    public class FilterValue : ObservableBase
    {
        private string _SongName = "";
        private string _Bsr = "";
        private string _Hash = "";
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
        private bool _IsShowCheckedOnly = true;
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

        public string SongName { get { return _SongName; } set { SetProperty(ref _SongName, value); } }
        public string Bsr { get { return _Bsr; } set { SetProperty(ref _Bsr, value); } }
        public string Hash { get { return _Hash; } set { SetProperty(ref _Hash, value); } }
        public bool IsShowRank { get { return _IsShowRank; } set { SetProperty(ref _IsShowRank, value); } }
        public bool IsShowUnRank { get { return _IsShowUnRank; } set { SetProperty(ref _IsShowUnRank, value); } }
        public bool IsShowClear { get { return _IsShowClear; } set { SetProperty(ref _IsShowClear, value); } }
        public bool IsShowFailure { get { return _IsShowFailure; } set { SetProperty(ref _IsShowFailure, value); } }
        public bool IsShowNotPlay { get { return _IsShowNotPlay; } set { SetProperty(ref _IsShowNotPlay, value); } }
        public bool IsShowFullCombo { get { return _IsShowFullCombo; } set { SetProperty(ref _IsShowFullCombo, value); } }
        public bool IsShowNotFullCombo { get { return _IsShowNotFullCombo; } set { SetProperty(ref _IsShowNotFullCombo, value); } }
        public bool IsShowStandard { get { return _IsShowStandard; } set { SetProperty(ref _IsShowStandard, value); } }
        public bool IsShowLawless { get { return _IsShowLawless; } set { SetProperty(ref _IsShowLawless, value); } }
        public bool IsShowOneSaber { get { return _IsShowOneSaber; } set { SetProperty(ref _IsShowOneSaber, value); } }
        public bool IsShowLightShow { get { return _IsShowLightShow; } set { SetProperty(ref _IsShowLightShow, value); } }
        public bool IsShow90Degree { get { return _IsShow90Degree; } set { SetProperty(ref _IsShow90Degree, value); } }
        public bool IsShow360Degree { get { return _IsShow360Degree; } set { SetProperty(ref _IsShow360Degree, value); } }
        public bool IsShowNoArrows { get { return _IsShowNoArrows; } set { SetProperty(ref _IsShowNoArrows, value); } }
        public bool IsShowEasy { get { return _IsShowEasy; } set { SetProperty(ref _IsShowEasy, value); } }
        public bool IsShowNormal { get { return _IsShowNormal; } set { SetProperty(ref _IsShowNormal, value); } }
        public bool IsShowHard { get { return _IsShowHard; } set { SetProperty(ref _IsShowHard, value); } }
        public bool IsShowExpert { get { return _IsShowExpert; } set { SetProperty(ref _IsShowExpert, value); } }
        public bool IsShowExpertPlus { get { return _IsShowExpertPlus; } set { SetProperty(ref _IsShowExpertPlus, value); } }
        public bool IsShowCheckedOnly { get { return _IsShowCheckedOnly; } set { SetProperty(ref _IsShowCheckedOnly, value); } }
        public double MinStar { get { return _MinStar; } set { SetProperty(ref _MinStar, value); } }
        public double MaxStar { get { return _MaxStar; } set { SetProperty(ref _MaxStar, value); } }
        public double MinPp { get { return _MinPp; } set { SetProperty(ref _MinPp, value); } }
        public double MaxPp { get { return _MaxPp; } set { SetProperty(ref _MaxPp, value); } }
        public double MinAcc { get { return _MinAcc; } set { SetProperty(ref _MinAcc, value); } }
        public double MaxAcc { get { return _MaxAcc; } set { SetProperty(ref _MaxAcc, value); } }
        public long MinScoreRank { get { return _MinScoreRank; } set { SetProperty(ref _MinScoreRank, value); } }
        public long MaxScoreRank { get { return _MaxScoreRank; } set { SetProperty(ref _MaxScoreRank, value); } }
        public DateTime? DateStart { get { return _DateStart; } set { SetProperty(ref _DateStart, value); } }
        public DateTime? DateEnd { get { return _DateEnd; } set { SetProperty(ref _DateEnd, value); } }
        public DateTime? RankedDateStart { get { return _RankedDateStart; } set { SetProperty(ref _RankedDateStart, value); } }
        public DateTime? RankedDateEnd { get { return _RankedDateEnd; } set { SetProperty(ref _RankedDateEnd, value); } }
    }
}
