using MyBeatSaberScore.Model;
using System;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using System.Xml.Linq;

namespace MyBeatSaberScore
{
    internal static class AppData
    {
        public static bool IsFirst { get; set; } = true;

        public static UserData SelectedUser { get; set; } = new();

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
        // 譜面情報
        internal FilterStringSearch _MapFullName = new(IntegrationScore.GetFilterTargetMapName);
        internal FilterStringSearch _MapBsr = new(IntegrationScore.GetFilterTargetMapName);
        internal FilterStringSearch _MapHash = new(IntegrationScore.GetFilterTargetMapHash);
        internal FilterMapRankStatus _MapRankStatus = new(IntegrationScore.GetFilterTargetMapTypeIsRanked);
        internal FilterMapMode _MapMode = new(IntegrationScore.GetFilterTargetMapMode);
        internal FilterMapDifficulty _MapDifficulty = new(IntegrationScore.GetFilterTargetMapDifficulty);
        internal FilterDoubleRange _MapStar = new(0, 20, IntegrationScore.GetFilterTargetMapStar);
        internal FilterDoubleRange _MapDuration = new(double.MinValue, double.MaxValue, IntegrationScore.GetFilterTargetMapDuration);
        internal FilterDoubleRange _MapBpm = new(double.MinValue, double.MaxValue, IntegrationScore.GetFilterTargetMapBpm);
        internal FilterLongRange _MapNotes = new(long.MinValue, long.MaxValue, IntegrationScore.GetFilterTargetMapNotes);
        internal FilterLongRange _MapBombs = new(long.MinValue, long.MaxValue, IntegrationScore.GetFilterTargetMapBombs);
        internal FilterLongRange _MapWalls = new(long.MinValue, long.MaxValue, IntegrationScore.GetFilterTargetMapWalls);
        internal FilterDoubleRange _MapNps = new(double.MinValue, double.MaxValue, IntegrationScore.GetFilterTargetMapNps);
        internal FilterDoubleRange _MapNjs = new(double.MinValue, double.MaxValue, IntegrationScore.GetFilterTargetMapNjs);
        internal FilterDateTimeRange _MapRankedDate = new(null, null, IntegrationScore.GetFilterTargetMapRankedDate);

        // プレイ結果
        internal FilterDateTimeRange _PlayUpdateDate = new(null, null, IntegrationScore.GetFilterTargetPlayUpdateDate);
        internal FilterPlayResult _PlayResult = new(IntegrationScore.GetFilterTargetPlayResultType);
        internal FilterPlayFullCombo _PlayFullCombo = new(IntegrationScore.GetFilterTargetPlayIsFullCombo);
        internal FilterDoubleRange _PlayPp = new(double.MinValue, double.MaxValue, IntegrationScore.GetFilterTargetPlayPp);
        internal FilterDoubleRange _PlayAcc = new(double.MinValue, double.MaxValue, IntegrationScore.GetFilterTargetPlayAcc);
        internal FilterLongRange _PlayWorldRank = new(long.MinValue, long.MaxValue, IntegrationScore.GetFilterTargetPlayWorldRank);
        internal FilterLongRange _PlayMissPlusBad = new(long.MinValue, long.MaxValue, IntegrationScore.GetFilterTargetPlayMissPlusBad);
        internal FilterLongRange _PlayMiss = new(long.MinValue, long.MaxValue, IntegrationScore.GetFilterTargetPlayMiss);
        internal FilterLongRange _PlayBad = new(long.MinValue, long.MaxValue, IntegrationScore.GetFilterTargetPlayBad);
        internal FilterPlayModifiers _PlayModifiers = new(IntegrationScore.GetFilterTargetPlayModifiers);

        // Etc
        internal FilterEtcCheckedOnly _EtcCheckedOnly = new(IntegrationScore.GetFilterTargetEtcCheckedOnly);


        // 譜面情報
        public string SongName { get { return _MapFullName.SearchValue; } set { SetProperty(ref _MapFullName.SearchValue, value); } }
        public string Bsr { get { return _MapBsr.SearchValue; } set { SetProperty(ref _MapBsr.SearchValue, value); } }
        public string Hash { get { return _MapHash.SearchValue; } set { SetProperty(ref _MapHash.SearchValue, value); } }
        public bool IsShowRank { get { return _MapRankStatus.ShowRanked; } set { SetProperty(ref _MapRankStatus.ShowRanked, value); } }
        public bool IsShowUnRank { get { return _MapRankStatus.ShowUnRanked; } set { SetProperty(ref _MapRankStatus.ShowUnRanked, value); } }
        public bool IsShowStandard { get { return _MapMode.ShowStandard; } set { SetProperty(ref _MapMode.ShowStandard, value); } }
        public bool IsShowLawless { get { return _MapMode.ShowLawless; } set { SetProperty(ref _MapMode.ShowLawless, value); } }
        public bool IsShowOneSaber { get { return _MapMode.ShowOneSaber; } set { SetProperty(ref _MapMode.ShowOneSaber, value); } }
        public bool IsShowLightShow { get { return _MapMode.ShowLightShow; } set { SetProperty(ref _MapMode.ShowLightShow, value); } }
        public bool IsShow90Degree { get { return _MapMode.ShowDegree90; } set { SetProperty(ref _MapMode.ShowDegree90, value); } }
        public bool IsShow360Degree { get { return _MapMode.ShowDegree360; } set { SetProperty(ref _MapMode.ShowDegree360, value); } }
        public bool IsShowNoArrows { get { return _MapMode.ShowNoArrows; } set { SetProperty(ref _MapMode.ShowNoArrows, value); } }
        public bool IsShowEasy { get { return _MapDifficulty.ShowEasy; } set { SetProperty(ref _MapDifficulty.ShowEasy, value); } }
        public bool IsShowNormal { get { return _MapDifficulty.ShowNormal; } set { SetProperty(ref _MapDifficulty.ShowNormal, value); } }
        public bool IsShowHard { get { return _MapDifficulty.ShowHard; } set { SetProperty(ref _MapDifficulty.ShowHard, value); } }
        public bool IsShowExpert { get { return _MapDifficulty.ShowExpert; } set { SetProperty(ref _MapDifficulty.ShowExpert, value); } }
        public bool IsShowExpertPlus { get { return _MapDifficulty.ShowExpertPlus; } set { SetProperty(ref _MapDifficulty.ShowExpertPlus, value); } }
        public double MinStar { get { return _MapStar.MinValue; } set { SetProperty(ref _MapStar.MinValue, value); } }
        public double MaxStar { get { return _MapStar.MaxValue; } set { SetProperty(ref _MapStar.MaxValue, value); } }
        public double MinDuration { get { return _MapDuration.MinValue; } set { SetProperty(ref _MapDuration.MinValue, value); } }
        public double MaxDuration { get { return _MapDuration.MaxValue; } set { SetProperty(ref _MapDuration.MaxValue, value); } }
        public double MinBpm { get { return _MapBpm.MinValue; } set { SetProperty(ref _MapBpm.MinValue, value); } }
        public double MaxBpm { get { return _MapBpm.MaxValue; } set { SetProperty(ref _MapBpm.MaxValue, value); } }
        public long MinNote { get { return _MapNotes.MinValue; } set { SetProperty(ref _MapNotes.MinValue, value); } }
        public long MaxNote { get { return _MapNotes.MaxValue; } set { SetProperty(ref _MapNotes.MaxValue, value); } }
        public long MinBomb { get { return _MapBombs.MinValue; } set { SetProperty(ref _MapBombs.MinValue, value); } }
        public long MaxBomb { get { return _MapBombs.MaxValue; } set { SetProperty(ref _MapBombs.MaxValue, value); } }
        public long MinWall { get { return _MapWalls.MinValue; } set { SetProperty(ref _MapWalls.MinValue, value); } }
        public long MaxWall { get { return _MapWalls.MaxValue; } set { SetProperty(ref _MapWalls.MaxValue, value); } }
        public double MinNps { get { return _MapNps.MinValue; } set { SetProperty(ref _MapNps.MinValue, value); } }
        public double MaxNps { get { return _MapNps.MaxValue; } set { SetProperty(ref _MapNps.MaxValue, value); } }
        public double MinNjs { get { return _MapNjs.MinValue; } set { SetProperty(ref _MapNjs.MinValue, value); } }
        public double MaxNjs { get { return _MapNjs.MaxValue; } set { SetProperty(ref _MapNjs.MaxValue, value); } }
        public DateTime? RankedDateStart { get { return _MapRankedDate.MinValue; } set { SetProperty(ref _MapRankedDate.MinValue, value); } }
        public DateTime? RankedDateEnd { get { return _MapRankedDate.MaxValue; } set { SetProperty(ref _MapRankedDate.MaxValue, value); } }

        // プレイ結果
        public DateTime? DateStart { get { return _PlayUpdateDate.MinValue; } set { SetProperty(ref _PlayUpdateDate.MinValue, value); } }
        public DateTime? DateEnd { get { return _PlayUpdateDate.MaxValue; } set { SetProperty(ref _PlayUpdateDate.MaxValue, value); } }
        public bool IsShowClear { get { return _PlayResult.ShowClear; } set { SetProperty(ref _PlayResult.ShowClear, value); } }
        public bool IsShowFailure { get { return _PlayResult.ShowFailure; } set { SetProperty(ref _PlayResult.ShowFailure, value); } }
        public bool IsShowNotPlay { get { return _PlayResult.ShowNotPlay; } set { SetProperty(ref _PlayResult.ShowNotPlay, value); } }
        public bool IsShowFullCombo { get { return _PlayFullCombo.ShowFullCombo; } set { SetProperty(ref _PlayFullCombo.ShowFullCombo, value); } }
        public bool IsShowNotFullCombo { get { return _PlayFullCombo.ShowNotFullCombo; } set { SetProperty(ref _PlayFullCombo.ShowNotFullCombo, value); } }
        public double MinPp { get { return _PlayPp.MinValue; } set { SetProperty(ref _PlayPp.MinValue, value); } }
        public double MaxPp { get { return _PlayPp.MaxValue; } set { SetProperty(ref _PlayPp.MaxValue, value); } }
        public double MinAcc { get { return _PlayAcc.MinValue; } set { SetProperty(ref _PlayAcc.MinValue, value); } }
        public double MaxAcc { get { return _PlayAcc.MaxValue; } set { SetProperty(ref _PlayAcc.MaxValue, value); } }
        public long MinScoreRank { get { return _PlayWorldRank.MinValue; } set { SetProperty(ref _PlayWorldRank.MinValue, value); } }
        public long MaxScoreRank { get { return _PlayWorldRank.MaxValue; } set { SetProperty(ref _PlayWorldRank.MaxValue, value); } }
        public long MinMissPlusBad { get { return _PlayMissPlusBad.MinValue; } set { SetProperty(ref _PlayMissPlusBad.MinValue, value); } }
        public long MaxMissPusBad { get { return _PlayMissPlusBad.MaxValue; } set { SetProperty(ref _PlayMissPlusBad.MaxValue, value); } }
        public long MinMiss { get { return _PlayMiss.MinValue; } set { SetProperty(ref _PlayMiss.MinValue, value); } }
        public long MaxMiss { get { return _PlayMiss.MaxValue; } set { SetProperty(ref _PlayMiss.MaxValue, value); } }
        public long MinBad { get { return _PlayBad.MinValue; } set { SetProperty(ref _PlayBad.MinValue, value); } }
        public long MaxBad { get { return _PlayBad.MaxValue; } set { SetProperty(ref _PlayBad.MaxValue, value); } }
        public bool ModifierAny { get { return _PlayModifiers.Any; } set { SetProperty(ref _PlayModifiers.Any, value); } }
        public bool ModifierNone { get { return _PlayModifiers.None; } set { SetProperty(ref _PlayModifiers.None, value); } }
        public bool ModifierBE { get { return _PlayModifiers.Flag.HasFlag(ModifiersFlag.BE); } set { SetModifiersFlag(ModifiersFlag.BE, value); } }
        public bool ModifierDA { get { return _PlayModifiers.Flag.HasFlag(ModifiersFlag.DA); } set { SetModifiersFlag(ModifiersFlag.DA, value); } }
        public bool ModifierFS { get { return _PlayModifiers.Flag.HasFlag(ModifiersFlag.FS); } set { SetModifiersFlag(ModifiersFlag.FS, value); } }
        public bool ModifierGN { get { return _PlayModifiers.Flag.HasFlag(ModifiersFlag.GN); } set { SetModifiersFlag(ModifiersFlag.GN, value); } }
        public bool ModifierIF { get { return _PlayModifiers.Flag.HasFlag(ModifiersFlag.IF); } set { SetModifiersFlag(ModifiersFlag.IF, value); } }
        public bool ModifierNA { get { return _PlayModifiers.Flag.HasFlag(ModifiersFlag.NA); } set { SetModifiersFlag(ModifiersFlag.NA, value); } }
        public bool ModifierNB { get { return _PlayModifiers.Flag.HasFlag(ModifiersFlag.NB); } set { SetModifiersFlag(ModifiersFlag.NB, value); } }
        public bool ModifierNF { get { return _PlayModifiers.Flag.HasFlag(ModifiersFlag.NF); } set { SetModifiersFlag(ModifiersFlag.NF, value); } }
        public bool ModifierNO { get { return _PlayModifiers.Flag.HasFlag(ModifiersFlag.NO); } set { SetModifiersFlag(ModifiersFlag.NO, value); } }
        public bool ModifierOD { get { return _PlayModifiers.Flag.HasFlag(ModifiersFlag.OD); } set { SetModifiersFlag(ModifiersFlag.OD, value); } }
        public bool ModifierOP { get { return _PlayModifiers.Flag.HasFlag(ModifiersFlag.OP); } set { SetModifiersFlag(ModifiersFlag.OP, value); } }
        public bool ModifierPM { get { return _PlayModifiers.Flag.HasFlag(ModifiersFlag.PM); } set { SetModifiersFlag(ModifiersFlag.PM, value); } }
        public bool ModifierSC { get { return _PlayModifiers.Flag.HasFlag(ModifiersFlag.SC); } set { SetModifiersFlag(ModifiersFlag.SC, value); } }
        public bool ModifierSF { get { return _PlayModifiers.Flag.HasFlag(ModifiersFlag.SF); } set { SetModifiersFlag(ModifiersFlag.SF, value); } }
        public bool ModifierSS { get { return _PlayModifiers.Flag.HasFlag(ModifiersFlag.SS); } set { SetModifiersFlag(ModifiersFlag.SS, value); } }

        // Etc
        public bool IsShowCheckedOnly { get { return _EtcCheckedOnly.ShowChecked; } set { SetProperty(ref _EtcCheckedOnly.ShowChecked, value); } }

        void SetModifiersFlag(ModifiersFlag flag, bool on, [CallerMemberName] string? name = null)
        {
            if (on)
            {
                _PlayModifiers.Flag |= flag;
            }
            else
            {
                _PlayModifiers.Flag &= ~flag;
            }
            OnPropertyChanged(name);
        }
    }
}
