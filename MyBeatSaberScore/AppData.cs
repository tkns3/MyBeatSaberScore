using MyBeatSaberScore.Model;
using Newtonsoft.Json.Linq;
using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using System.Xml.Linq;

namespace MyBeatSaberScore
{
    internal static class AppData
    {
        public static bool IsFirst { get; set; } = true;

        public static UserData SelectedUser { get; set; } = new();

        public static MainPageFilterViewModel MainPageFilter { get; set; } = new();

        public static ViewTarget ViewTarget { get => Config.ViewTarget; set => Config.ViewTarget = value; }
    }

    [Flags]
    public enum ViewTarget
    {
        None = 0x00,
        BeatLeader = 0x01,
        ScoreSaber = 0x02,
    }

    public class MainPageFilterValue
    {
        public string FilterName = "No filtering";

        // 譜面情報
        public FilterStringSearch MapFullName = new(IntegrationScore.GetFilterTargetMapName);
        public FilterStringSearch MapBsr = new(IntegrationScore.GetFilterTargetMapBsr);
        public FilterStringSearch MapHash = new(IntegrationScore.GetFilterTargetMapHash);
        public FilterMapRankStatus MapRankStatus = new(IntegrationScore.GetFilterTargetMapTypeIsRanked);
        public FilterMapMode MapMode = new(IntegrationScore.GetFilterTargetMapMode);
        public FilterMapDifficulty MapDifficulty = new(IntegrationScore.GetFilterTargetMapDifficulty);
        public FilterDoubleRange MapStar = new(0, 20, IntegrationScore.GetFilterTargetMapStar);
        public FilterDoubleRange MapDuration = new(double.MinValue, double.MaxValue, IntegrationScore.GetFilterTargetMapDuration);
        public FilterDoubleRange MapBpm = new(double.MinValue, double.MaxValue, IntegrationScore.GetFilterTargetMapBpm);
        public FilterLongRange MapNotes = new(long.MinValue, long.MaxValue, IntegrationScore.GetFilterTargetMapNotes);
        public FilterLongRange MapBombs = new(long.MinValue, long.MaxValue, IntegrationScore.GetFilterTargetMapBombs);
        public FilterLongRange MapWalls = new(long.MinValue, long.MaxValue, IntegrationScore.GetFilterTargetMapWalls);
        public FilterDoubleRange MapNps = new(double.MinValue, double.MaxValue, IntegrationScore.GetFilterTargetMapNps);
        public FilterDoubleRange MapNjs = new(double.MinValue, double.MaxValue, IntegrationScore.GetFilterTargetMapNjs);
        public FilterDateTimeRange MapRankedDate = new(null, null, IntegrationScore.GetFilterTargetMapRankedDate);

        // プレイ結果
        public FilterDateTimeRange PlayUpdateDate = new(null, null, IntegrationScore.GetFilterTargetPlayUpdateDate);
        public FilterPlayResult PlayResult = new(IntegrationScore.GetFilterTargetPlayResultType);
        public FilterPlayFullCombo PlayFullCombo = new(IntegrationScore.GetFilterTargetPlayIsFullCombo);
        public FilterDoubleRange PlayPp = new(double.MinValue, double.MaxValue, IntegrationScore.GetFilterTargetPlayPp);
        public FilterDoubleRange PlayAcc = new(double.MinValue, double.MaxValue, IntegrationScore.GetFilterTargetPlayAcc);
        public FilterLongRange PlayWorldRank = new(long.MinValue, long.MaxValue, IntegrationScore.GetFilterTargetPlayWorldRank);
        public FilterLongRange PlayMissPlusBad = new(long.MinValue, long.MaxValue, IntegrationScore.GetFilterTargetPlayMissPlusBad);
        public FilterLongRange PlayMiss = new(long.MinValue, long.MaxValue, IntegrationScore.GetFilterTargetPlayMiss);
        public FilterLongRange PlayBad = new(long.MinValue, long.MaxValue, IntegrationScore.GetFilterTargetPlayBad);
        public FilterPlayModifiers PlayModifiers = new(IntegrationScore.GetFilterTargetPlayModifiers);

        // Etc
        public FilterEtcCheckedOnly EtcCheckedOnly = new(IntegrationScore.GetFilterTargetEtcCheckedOnly);

        public MainPageFilterValue CopyFrom(MainPageFilterValue other)
        {
            this.FilterName = other.FilterName;
            this.MapFullName.CopyFrom(other.MapFullName);
            this.MapBsr.CopyFrom(other.MapBsr);
            this.MapHash.CopyFrom(other.MapHash);
            this.MapRankStatus.CopyFrom(other.MapRankStatus);
            this.MapMode.CopyFrom(other.MapMode);
            this.MapDifficulty.CopyFrom(other.MapDifficulty);
            this.MapStar.CopyFrom(other.MapStar);
            this.MapDuration.CopyFrom(other.MapDuration);
            this.MapBpm.CopyFrom(other.MapBpm);
            this.MapNotes.CopyFrom(other.MapNotes);
            this.MapBombs.CopyFrom(other.MapBombs);
            this.MapWalls.CopyFrom(other.MapWalls);
            this.MapNps.CopyFrom(other.MapNps);
            this.MapNjs.CopyFrom(other.MapNjs);
            this.MapRankedDate.CopyFrom(other.MapRankedDate);
            this.PlayUpdateDate.CopyFrom(other.PlayUpdateDate);
            this.PlayResult.CopyFrom(other.PlayResult);
            this.PlayFullCombo.CopyFrom(other.PlayFullCombo);
            this.PlayPp.CopyFrom(other.PlayPp);
            this.PlayAcc.CopyFrom(other.PlayAcc);
            this.PlayWorldRank.CopyFrom(other.PlayWorldRank);
            this.PlayMissPlusBad.CopyFrom(other.PlayMissPlusBad);
            this.PlayMiss.CopyFrom(other.PlayMiss);
            this.PlayBad.CopyFrom(other.PlayBad);
            this.PlayModifiers.CopyFrom(other.PlayModifiers);
            this.EtcCheckedOnly.CopyFrom(other.EtcCheckedOnly);
            return this;
        }

        public void Parse(JObject? node)
        {
            if (node != null)
            {
                FilterName = GetValue(node, "FilterName", ParseToString, "");
                MapFullName.SearchValue = "";
                MapBsr.SearchValue = "";
                MapHash.SearchValue = "";
                MapRankStatus.ShowRanked = GetValue(node, "MapRankStatus_ShowRanked", ParseToBool, true);
                MapRankStatus.ShowUnRanked = GetValue(node, "MapRankStatus_ShowUnRanked", ParseToBool, true);
                MapMode.ShowStandard = GetValue(node, "MapMode_ShowStandard", ParseToBool, true);
                MapMode.ShowLawless = GetValue(node, "MapMode_ShowLawless", ParseToBool, true);
                MapMode.ShowOneSaber = GetValue(node, "MapMode_ShowOneSaber", ParseToBool, true);
                MapMode.ShowLightShow = GetValue(node, "MapMode_ShowLightShow", ParseToBool, true);
                MapMode.ShowDegree90 = GetValue(node, "MapMode_ShowDegree90", ParseToBool, true);
                MapMode.ShowDegree360 = GetValue(node, "MapMode_ShowDegree360", ParseToBool, true);
                MapMode.ShowNoArrows = GetValue(node, "MapMode_ShowNoArrows", ParseToBool, true);
                MapDifficulty.ShowEasy = GetValue(node, "MapDifficulty_ShowEasy", ParseToBool, true);
                MapDifficulty.ShowNormal = GetValue(node, "MapDifficulty_ShowNormal", ParseToBool, true);
                MapDifficulty.ShowHard = GetValue(node, "MapDifficulty_ShowHard", ParseToBool, true);
                MapDifficulty.ShowExpert = GetValue(node, "MapDifficulty_ShowExpert", ParseToBool, true);
                MapDifficulty.ShowExpertPlus = GetValue(node, "MapDifficulty_ShowExpertPlus", ParseToBool, true);
                MapStar.MinValue = GetValue(node, "MapStar_MinValue", ParseToDouble, double.MinValue);
                MapStar.MaxValue = GetValue(node, "MapStar_MaxValue", ParseToDouble, double.MaxValue);
                MapDuration.MinValue = GetValue(node, "MapDuration_MinValue", ParseToDouble, double.MinValue);
                MapDuration.MaxValue = GetValue(node, "MapDuration_MaxValue", ParseToDouble, double.MaxValue);
                MapBpm.MinValue = GetValue(node, "MapBpm_MinValue", ParseToDouble, double.MinValue);
                MapBpm.MaxValue = GetValue(node, "MapBpm_MaxValue", ParseToDouble, double.MaxValue);
                MapNotes.MinValue = GetValue(node, "MapNotes_MinValue", ParseToLong, long.MinValue);
                MapNotes.MaxValue = GetValue(node, "MapNotes_MaxValue", ParseToLong, long.MaxValue);
                MapBombs.MinValue = GetValue(node, "MapBombs_MinValue", ParseToLong, long.MinValue);
                MapBombs.MaxValue = GetValue(node, "MapBombs_MaxValue", ParseToLong, long.MaxValue);
                MapWalls.MinValue = GetValue(node, "MapWalls_MinValue", ParseToLong, long.MinValue);
                MapWalls.MaxValue = GetValue(node, "MapWalls_MaxValue", ParseToLong, long.MaxValue);
                MapNps.MinValue = GetValue(node, "MapNps_MinValue", ParseToDouble, double.MinValue);
                MapNps.MaxValue = GetValue(node, "MapNps_MaxValue", ParseToDouble, double.MaxValue);
                MapNjs.MinValue = GetValue(node, "MapNjs_MinValue", ParseToDouble, double.MinValue);
                MapNjs.MaxValue = GetValue(node, "MapNjs_MaxValue", ParseToDouble, double.MaxValue);
                MapRankedDate.MinValue = GetValue<DateTime?>(node, "MapRankedDate_MinValue", ParseToDateTime, null);
                MapRankedDate.MaxValue = GetValue<DateTime?>(node, "MapRankedDate_MaxValue", ParseToDateTime, null);
                PlayUpdateDate.MinValue = GetValue<DateTime?>(node, "PlayUpdateDate_MinValue", ParseToDateTime, null);
                PlayUpdateDate.MaxValue = GetValue<DateTime?>(node, "PlayUpdateDate_MaxValue", ParseToDateTime, null);
                PlayResult.ShowClear = GetValue(node, "PlayResult_ShowClear", ParseToBool, true);
                PlayResult.ShowFailure = GetValue(node, "PlayResult_ShowFailure", ParseToBool, true);
                PlayResult.ShowNotPlay = GetValue(node, "PlayResult_ShowNotPlay", ParseToBool, true);
                PlayFullCombo.ShowFullCombo = GetValue(node, "PlayFullCombo_ShowFullCombo", ParseToBool, true);
                PlayFullCombo.ShowNotFullCombo = GetValue(node, "PlayFullCombo_ShowNotFullCombo", ParseToBool, true);
                PlayPp.MinValue = GetValue(node, "PlayPp_MinValue", ParseToDouble, double.MinValue);
                PlayPp.MaxValue = GetValue(node, "PlayPp_MaxValue", ParseToDouble, double.MaxValue);
                PlayAcc.MinValue = GetValue(node, "PlayAcc_MinValue", ParseToDouble, double.MinValue);
                PlayAcc.MaxValue = GetValue(node, "PlayAcc_MaxValue", ParseToDouble, double.MaxValue);
                PlayWorldRank.MinValue = GetValue(node, "PlayWorldRank_MinValue", ParseToLong, long.MinValue);
                PlayWorldRank.MaxValue = GetValue(node, "PlayWorldRank_MaxValue", ParseToLong, long.MaxValue);
                PlayMissPlusBad.MinValue = GetValue(node, "PlayMissPlusBad_MinValue", ParseToLong, long.MinValue);
                PlayMissPlusBad.MaxValue = GetValue(node, "PlayMissPlusBad_MaxValue", ParseToLong, long.MaxValue);
                PlayMiss.MinValue = GetValue(node, "PlayMiss_MinValue", ParseToLong, long.MinValue);
                PlayMiss.MaxValue = GetValue(node, "PlayMiss_MaxValue", ParseToLong, long.MaxValue);
                PlayBad.MinValue = GetValue(node, "PlayBad_MinValue", ParseToLong, long.MinValue);
                PlayBad.MaxValue = GetValue(node, "PlayBad_MaxValue", ParseToLong, long.MaxValue);
                PlayModifiers.Any = GetValue(node, "PlayModifiers_Any", ParseToBool, true);
                PlayModifiers.None = GetValue(node, "PlayModifiers_None", ParseToBool, false);
                PlayModifiers.Flag |= GetValue(node, "PlayModifiers_BE", ParseToBool, false) ? ModifiersFlag.BE : 0;
                PlayModifiers.Flag |= GetValue(node, "PlayModifiers_DA", ParseToBool, false) ? ModifiersFlag.DA : 0;
                PlayModifiers.Flag |= GetValue(node, "PlayModifiers_FS", ParseToBool, false) ? ModifiersFlag.FS : 0;
                PlayModifiers.Flag |= GetValue(node, "PlayModifiers_GN", ParseToBool, false) ? ModifiersFlag.GN : 0;
                PlayModifiers.Flag |= GetValue(node, "PlayModifiers_IF", ParseToBool, false) ? ModifiersFlag.IF : 0;
                PlayModifiers.Flag |= GetValue(node, "PlayModifiers_NA", ParseToBool, false) ? ModifiersFlag.NA : 0;
                PlayModifiers.Flag |= GetValue(node, "PlayModifiers_NB", ParseToBool, false) ? ModifiersFlag.NB : 0;
                PlayModifiers.Flag |= GetValue(node, "PlayModifiers_NF", ParseToBool, false) ? ModifiersFlag.NF : 0;
                PlayModifiers.Flag |= GetValue(node, "PlayModifiers_NO", ParseToBool, false) ? ModifiersFlag.NO : 0;
                PlayModifiers.Flag |= GetValue(node, "PlayModifiers_OD", ParseToBool, false) ? ModifiersFlag.OD : 0;
                PlayModifiers.Flag |= GetValue(node, "PlayModifiers_OP", ParseToBool, false) ? ModifiersFlag.OP : 0;
                PlayModifiers.Flag |= GetValue(node, "PlayModifiers_PM", ParseToBool, false) ? ModifiersFlag.PM : 0;
                PlayModifiers.Flag |= GetValue(node, "PlayModifiers_SC", ParseToBool, false) ? ModifiersFlag.SC : 0;
                PlayModifiers.Flag |= GetValue(node, "PlayModifiers_SF", ParseToBool, false) ? ModifiersFlag.SF : 0;
                PlayModifiers.Flag |= GetValue(node, "PlayModifiers_SS", ParseToBool, false) ? ModifiersFlag.SS : 0;
                EtcCheckedOnly.ShowCheckedOnly = GetValue(node, "EtcCheckedOnly_ShowCheckedOnly", ParseToBool, true);
            }
        }

        public JObject GetJObject()
        {
            JObject obj = new JObject
            {
                new JProperty("FilterName", FilterName),
                new JProperty("MapRankStatus_ShowRanked", MapRankStatus.ShowRanked.ToString()),
                new JProperty("MapRankStatus_ShowUnRanked", MapRankStatus.ShowUnRanked.ToString()),
                new JProperty("MapMode_ShowStandard", MapMode.ShowStandard.ToString()),
                new JProperty("MapMode_ShowLawless", MapMode.ShowLawless.ToString()),
                new JProperty("MapMode_ShowOneSaber", MapMode.ShowOneSaber.ToString()),
                new JProperty("MapMode_ShowLightShow", MapMode.ShowLightShow.ToString()),
                new JProperty("MapMode_ShowDegree90", MapMode.ShowDegree90.ToString()),
                new JProperty("MapMode_ShowDegree360", MapMode.ShowDegree360.ToString()),
                new JProperty("MapMode_ShowNoArrows", MapMode.ShowNoArrows.ToString()),
                new JProperty("MapDifficulty_ShowEasy", MapDifficulty.ShowEasy.ToString()),
                new JProperty("MapDifficulty_ShowNormal", MapDifficulty.ShowNormal.ToString()),
                new JProperty("MapDifficulty_ShowHard", MapDifficulty.ShowHard.ToString()),
                new JProperty("MapDifficulty_ShowExpert", MapDifficulty.ShowExpert.ToString()),
                new JProperty("MapDifficulty_ShowExpertPlus", MapDifficulty.ShowExpertPlus.ToString()),
                new JProperty("MapStar_MinValue", (MapStar.MinValue == double.MinValue) ? "unlimited" : MapStar.MinValue.ToString()),
                new JProperty("MapStar_MaxValue", (MapStar.MaxValue == double.MaxValue) ? "unlimited" : MapStar.MaxValue.ToString()),
                new JProperty("MapDuration_MinValue", (MapDuration.MinValue == double.MinValue) ? "unlimited" : MapDuration.MinValue.ToString()),
                new JProperty("MapDuration_MaxValue", (MapDuration.MaxValue == double.MaxValue) ? "unlimited" : MapDuration.MaxValue.ToString()),
                new JProperty("MapBpm_MinValue", (MapBpm.MinValue == double.MinValue) ? "unlimited" : MapBpm.MinValue.ToString()),
                new JProperty("MapBpm_MaxValue", (MapBpm.MaxValue == double.MaxValue) ? "unlimited" : MapBpm.MaxValue.ToString()),
                new JProperty("MapNotes_MinValue", (MapNotes.MinValue == long.MinValue) ? "unlimited" : MapNotes.MinValue.ToString()),
                new JProperty("MapNotes_MaxValue", (MapNotes.MaxValue == long.MaxValue) ? "unlimited" : MapNotes.MaxValue.ToString()),
                new JProperty("MapBombs_MinValue", (MapBombs.MinValue == long.MinValue) ? "unlimited" : MapBombs.MinValue.ToString()),
                new JProperty("MapBombs_MaxValue", (MapBombs.MaxValue == long.MaxValue) ? "unlimited" : MapBombs.MaxValue.ToString()),
                new JProperty("MapWalls_MinValue", (MapWalls.MinValue == long.MinValue) ? "unlimited" : MapWalls.MinValue.ToString()),
                new JProperty("MapWalls_MaxValue", (MapWalls.MaxValue == long.MaxValue) ? "unlimited" : MapWalls.MaxValue.ToString()),
                new JProperty("MapNps_MinValue", (MapNps.MinValue == double.MinValue) ? "unlimited" : MapNps.MinValue.ToString()),
                new JProperty("MapNps_MaxValue", (MapNps.MaxValue == double.MaxValue) ? "unlimited" : MapNps.MaxValue.ToString()),
                new JProperty("MapNjs_MinValue", (MapNjs.MinValue == double.MinValue) ? "unlimited" : MapNjs.MinValue.ToString()),
                new JProperty("MapNjs_MaxValue", (MapNjs.MaxValue == double.MaxValue) ? "unlimited" : MapNjs.MaxValue.ToString()),
                new JProperty("MapRankedDate_MinValue", MapRankedDate.MinValue.ToString()),
                new JProperty("MapRankedDate_MaxValue", MapRankedDate.MaxValue.ToString()),
                new JProperty("PlayUpdateDate_MinValue", PlayUpdateDate.MinValue.ToString()),
                new JProperty("PlayUpdateDate_MaxValue", PlayUpdateDate.MaxValue.ToString()),
                new JProperty("PlayResult_ShowClear", PlayResult.ShowClear.ToString()),
                new JProperty("PlayResult_ShowFailure", PlayResult.ShowFailure.ToString()),
                new JProperty("PlayResult_ShowNotPlay", PlayResult.ShowNotPlay.ToString()),
                new JProperty("PlayFullCombo_ShowFullCombo", PlayFullCombo.ShowFullCombo.ToString()),
                new JProperty("PlayFullCombo_ShowNotFullCombo", PlayFullCombo.ShowNotFullCombo.ToString()),
                new JProperty("PlayPp_MinValue", (PlayPp.MinValue == double.MinValue) ? "unlimited" : PlayPp.MinValue.ToString()),
                new JProperty("PlayPp_MaxValue", (PlayPp.MaxValue == double.MaxValue) ? "unlimited" : PlayPp.MaxValue.ToString()),
                new JProperty("PlayAcc_MinValue", (PlayAcc.MinValue == double.MinValue) ? "unlimited" : PlayAcc.MinValue.ToString()),
                new JProperty("PlayAcc_MaxValue", (PlayAcc.MaxValue == double.MaxValue) ? "unlimited" : PlayAcc.MaxValue.ToString()),
                new JProperty("PlayWorldRank_MinValue", (PlayWorldRank.MinValue == long.MinValue) ? "unlimited" : PlayWorldRank.MinValue.ToString()),
                new JProperty("PlayWorldRank_MaxValue", (PlayWorldRank.MaxValue == long.MaxValue) ? "unlimited" : PlayWorldRank.MaxValue.ToString()),
                new JProperty("PlayMissPlusBad_MinValue", (PlayMissPlusBad.MinValue == long.MinValue) ? "unlimited" : PlayMissPlusBad.MinValue.ToString()),
                new JProperty("PlayMissPlusBad_MaxValue", (PlayMissPlusBad.MaxValue == long.MaxValue) ? "unlimited" : PlayMissPlusBad.MaxValue.ToString()),
                new JProperty("PlayMiss_MinValue", (PlayMiss.MinValue == long.MinValue) ? "unlimited" : PlayMiss.MinValue.ToString()),
                new JProperty("PlayMiss_MaxValue", (PlayMiss.MaxValue == long.MaxValue) ? "unlimited" : PlayMiss.MaxValue.ToString()),
                new JProperty("PlayBad_MinValue", (PlayBad.MinValue == long.MinValue) ? "unlimited" : PlayBad.MinValue.ToString()),
                new JProperty("PlayBad_MaxValue", (PlayBad.MaxValue == long.MaxValue) ? "unlimited" : PlayBad.MaxValue.ToString()),
                new JProperty("PlayModifiers_Any", PlayModifiers.Any.ToString()),
                new JProperty("PlayModifiers_None", PlayModifiers.None.ToString()),
                new JProperty("PlayModifiers_BE", PlayModifiers.Flag.HasFlag(ModifiersFlag.BE).ToString()),
                new JProperty("PlayModifiers_DA", PlayModifiers.Flag.HasFlag(ModifiersFlag.DA).ToString()),
                new JProperty("PlayModifiers_FS", PlayModifiers.Flag.HasFlag(ModifiersFlag.FS).ToString()),
                new JProperty("PlayModifiers_GN", PlayModifiers.Flag.HasFlag(ModifiersFlag.GN).ToString()),
                new JProperty("PlayModifiers_IF", PlayModifiers.Flag.HasFlag(ModifiersFlag.IF).ToString()),
                new JProperty("PlayModifiers_NA", PlayModifiers.Flag.HasFlag(ModifiersFlag.NA).ToString()),
                new JProperty("PlayModifiers_NB", PlayModifiers.Flag.HasFlag(ModifiersFlag.NB).ToString()),
                new JProperty("PlayModifiers_NF", PlayModifiers.Flag.HasFlag(ModifiersFlag.NF).ToString()),
                new JProperty("PlayModifiers_NO", PlayModifiers.Flag.HasFlag(ModifiersFlag.NO).ToString()),
                new JProperty("PlayModifiers_OD", PlayModifiers.Flag.HasFlag(ModifiersFlag.OD).ToString()),
                new JProperty("PlayModifiers_OP", PlayModifiers.Flag.HasFlag(ModifiersFlag.OP).ToString()),
                new JProperty("PlayModifiers_PM", PlayModifiers.Flag.HasFlag(ModifiersFlag.PM).ToString()),
                new JProperty("PlayModifiers_SC", PlayModifiers.Flag.HasFlag(ModifiersFlag.SC).ToString()),
                new JProperty("PlayModifiers_SF", PlayModifiers.Flag.HasFlag(ModifiersFlag.SF).ToString()),
                new JProperty("PlayModifiers_SS", PlayModifiers.Flag.HasFlag(ModifiersFlag.SS).ToString()),
                new JProperty("EtcCheckedOnly_ShowCheckedOnly", EtcCheckedOnly.ShowCheckedOnly.ToString())
            };
            return obj;
        }

        private T GetValue<T>(JObject node, string name, Func<string, T, T> parse, T defaultValue)
        {
            var value = node[name]?.ToString();
            if (value?.Length > 0)
            {
                return parse(value, defaultValue);
            }
            return defaultValue;
        }

        private string ParseToString(string value, string defaultValue)
        {
            return value;
        }

        private bool ParseToBool(string value, bool defaultValue)
        {
            if (bool.TryParse(value, out var result))
            {
                return result;
            }
            return defaultValue;
        }

        private long ParseToLong(string value, long defaultValue)
        {
            if (long.TryParse(value, out var result))
            {
                return result;
            }
            return defaultValue;
        }

        private double ParseToDouble(string value, double defaultValue)
        {
            if (double.TryParse(value, out var result))
            {
                return result;
            }
            return defaultValue;
        }

        private DateTime? ParseToDateTime(string value, DateTime? defaultValue)
        {
            if (DateTime.TryParse(value, out var result))
            {
                return result;
            }
            return defaultValue;
        }
    }

    public class MainPageFilterViewModel : ObservableBase
    {
        public MainPageFilterValue Value = new MainPageFilterValue();

        public string FilterName { get { return Value.FilterName; } set { SetProperty(ref Value.FilterName, value); } }

        // 譜面情報
        public string SongName { get { return Value.MapFullName.SearchValue; } set { SetProperty(ref Value.MapFullName.SearchValue, value); } }
        public string Bsr { get { return Value.MapBsr.SearchValue; } set { SetProperty(ref Value.MapBsr.SearchValue, value); } }
        public string Hash { get { return Value.MapHash.SearchValue; } set { SetProperty(ref Value.MapHash.SearchValue, value); } }
        public bool IsShowRank { get { return Value.MapRankStatus.ShowRanked; } set { SetProperty(ref Value.MapRankStatus.ShowRanked, value); } }
        public bool IsShowUnRank { get { return Value.MapRankStatus.ShowUnRanked; } set { SetProperty(ref Value.MapRankStatus.ShowUnRanked, value); } }
        public bool IsShowStandard { get { return Value.MapMode.ShowStandard; } set { SetProperty(ref Value.MapMode.ShowStandard, value); } }
        public bool IsShowLawless { get { return Value.MapMode.ShowLawless; } set { SetProperty(ref Value.MapMode.ShowLawless, value); } }
        public bool IsShowOneSaber { get { return Value.MapMode.ShowOneSaber; } set { SetProperty(ref Value.MapMode.ShowOneSaber, value); } }
        public bool IsShowLightShow { get { return Value.MapMode.ShowLightShow; } set { SetProperty(ref Value.MapMode.ShowLightShow, value); } }
        public bool IsShow90Degree { get { return Value.MapMode.ShowDegree90; } set { SetProperty(ref Value.MapMode.ShowDegree90, value); } }
        public bool IsShow360Degree { get { return Value.MapMode.ShowDegree360; } set { SetProperty(ref Value.MapMode.ShowDegree360, value); } }
        public bool IsShowNoArrows { get { return Value.MapMode.ShowNoArrows; } set { SetProperty(ref Value.MapMode.ShowNoArrows, value); } }
        public bool IsShowEasy { get { return Value.MapDifficulty.ShowEasy; } set { SetProperty(ref Value.MapDifficulty.ShowEasy, value); } }
        public bool IsShowNormal { get { return Value.MapDifficulty.ShowNormal; } set { SetProperty(ref Value.MapDifficulty.ShowNormal, value); } }
        public bool IsShowHard { get { return Value.MapDifficulty.ShowHard; } set { SetProperty(ref Value.MapDifficulty.ShowHard, value); } }
        public bool IsShowExpert { get { return Value.MapDifficulty.ShowExpert; } set { SetProperty(ref Value.MapDifficulty.ShowExpert, value); } }
        public bool IsShowExpertPlus { get { return Value.MapDifficulty.ShowExpertPlus; } set { SetProperty(ref Value.MapDifficulty.ShowExpertPlus, value); } }
        public double MinStar { get { return Value.MapStar.MinValue; } set { SetProperty(ref Value.MapStar.MinValue, value); } }
        public double MaxStar { get { return Value.MapStar.MaxValue; } set { SetProperty(ref Value.MapStar.MaxValue, value); } }
        public double MinDuration { get { return Value.MapDuration.MinValue; } set { SetProperty(ref Value.MapDuration.MinValue, value); } }
        public double MaxDuration { get { return Value.MapDuration.MaxValue; } set { SetProperty(ref Value.MapDuration.MaxValue, value); } }
        public double MinBpm { get { return Value.MapBpm.MinValue; } set { SetProperty(ref Value.MapBpm.MinValue, value); } }
        public double MaxBpm { get { return Value.MapBpm.MaxValue; } set { SetProperty(ref Value.MapBpm.MaxValue, value); } }
        public long MinNote { get { return Value.MapNotes.MinValue; } set { SetProperty(ref Value.MapNotes.MinValue, value); } }
        public long MaxNote { get { return Value.MapNotes.MaxValue; } set { SetProperty(ref Value.MapNotes.MaxValue, value); } }
        public long MinBomb { get { return Value.MapBombs.MinValue; } set { SetProperty(ref Value.MapBombs.MinValue, value); } }
        public long MaxBomb { get { return Value.MapBombs.MaxValue; } set { SetProperty(ref Value.MapBombs.MaxValue, value); } }
        public long MinWall { get { return Value.MapWalls.MinValue; } set { SetProperty(ref Value.MapWalls.MinValue, value); } }
        public long MaxWall { get { return Value.MapWalls.MaxValue; } set { SetProperty(ref Value.MapWalls.MaxValue, value); } }
        public double MinNps { get { return Value.MapNps.MinValue; } set { SetProperty(ref Value.MapNps.MinValue, value); } }
        public double MaxNps { get { return Value.MapNps.MaxValue; } set { SetProperty(ref Value.MapNps.MaxValue, value); } }
        public double MinNjs { get { return Value.MapNjs.MinValue; } set { SetProperty(ref Value.MapNjs.MinValue, value); } }
        public double MaxNjs { get { return Value.MapNjs.MaxValue; } set { SetProperty(ref Value.MapNjs.MaxValue, value); } }
        public DateTime? RankedDateStart { get { return Value.MapRankedDate.MinValue; } set { SetProperty(ref Value.MapRankedDate.MinValue, value); } }
        public DateTime? RankedDateEnd { get { return Value.MapRankedDate.MaxValue; } set { SetProperty(ref Value.MapRankedDate.MaxValue, value); } }

        // プレイ結果
        public DateTime? DateStart { get { return Value.PlayUpdateDate.MinValue; } set { SetProperty(ref Value.PlayUpdateDate.MinValue, value); } }
        public DateTime? DateEnd { get { return Value.PlayUpdateDate.MaxValue; } set { SetProperty(ref Value.PlayUpdateDate.MaxValue, value); } }
        public bool IsShowClear { get { return Value.PlayResult.ShowClear; } set { SetProperty(ref Value.PlayResult.ShowClear, value); } }
        public bool IsShowFailure { get { return Value.PlayResult.ShowFailure; } set { SetProperty(ref Value.PlayResult.ShowFailure, value); } }
        public bool IsShowNotPlay { get { return Value.PlayResult.ShowNotPlay; } set { SetProperty(ref Value.PlayResult.ShowNotPlay, value); } }
        public bool IsShowFullCombo { get { return Value.PlayFullCombo.ShowFullCombo; } set { SetProperty(ref Value.PlayFullCombo.ShowFullCombo, value); } }
        public bool IsShowNotFullCombo { get { return Value.PlayFullCombo.ShowNotFullCombo; } set { SetProperty(ref Value.PlayFullCombo.ShowNotFullCombo, value); } }
        public double MinPp { get { return Value.PlayPp.MinValue; } set { SetProperty(ref Value.PlayPp.MinValue, value); } }
        public double MaxPp { get { return Value.PlayPp.MaxValue; } set { SetProperty(ref Value.PlayPp.MaxValue, value); } }
        public double MinAcc { get { return Value.PlayAcc.MinValue; } set { SetProperty(ref Value.PlayAcc.MinValue, value); } }
        public double MaxAcc { get { return Value.PlayAcc.MaxValue; } set { SetProperty(ref Value.PlayAcc.MaxValue, value); } }
        public long MinScoreRank { get { return Value.PlayWorldRank.MinValue; } set { SetProperty(ref Value.PlayWorldRank.MinValue, value); } }
        public long MaxScoreRank { get { return Value.PlayWorldRank.MaxValue; } set { SetProperty(ref Value.PlayWorldRank.MaxValue, value); } }
        public long MinMissPlusBad { get { return Value.PlayMissPlusBad.MinValue; } set { SetProperty(ref Value.PlayMissPlusBad.MinValue, value); } }
        public long MaxMissPusBad { get { return Value.PlayMissPlusBad.MaxValue; } set { SetProperty(ref Value.PlayMissPlusBad.MaxValue, value); } }
        public long MinMiss { get { return Value.PlayMiss.MinValue; } set { SetProperty(ref Value.PlayMiss.MinValue, value); } }
        public long MaxMiss { get { return Value.PlayMiss.MaxValue; } set { SetProperty(ref Value.PlayMiss.MaxValue, value); } }
        public long MinBad { get { return Value.PlayBad.MinValue; } set { SetProperty(ref Value.PlayBad.MinValue, value); } }
        public long MaxBad { get { return Value.PlayBad.MaxValue; } set { SetProperty(ref Value.PlayBad.MaxValue, value); } }
        public bool ModifierAny { get { return Value.PlayModifiers.Any; } set { SetProperty(ref Value.PlayModifiers.Any, value); } }
        public bool ModifierNone { get { return Value.PlayModifiers.None; } set { SetProperty(ref Value.PlayModifiers.None, value); } }
        public bool ModifierBE { get { return Value.PlayModifiers.Flag.HasFlag(ModifiersFlag.BE); } set { SetModifiersFlag(ModifiersFlag.BE, value); } }
        public bool ModifierDA { get { return Value.PlayModifiers.Flag.HasFlag(ModifiersFlag.DA); } set { SetModifiersFlag(ModifiersFlag.DA, value); } }
        public bool ModifierFS { get { return Value.PlayModifiers.Flag.HasFlag(ModifiersFlag.FS); } set { SetModifiersFlag(ModifiersFlag.FS, value); } }
        public bool ModifierGN { get { return Value.PlayModifiers.Flag.HasFlag(ModifiersFlag.GN); } set { SetModifiersFlag(ModifiersFlag.GN, value); } }
        public bool ModifierIF { get { return Value.PlayModifiers.Flag.HasFlag(ModifiersFlag.IF); } set { SetModifiersFlag(ModifiersFlag.IF, value); } }
        public bool ModifierNA { get { return Value.PlayModifiers.Flag.HasFlag(ModifiersFlag.NA); } set { SetModifiersFlag(ModifiersFlag.NA, value); } }
        public bool ModifierNB { get { return Value.PlayModifiers.Flag.HasFlag(ModifiersFlag.NB); } set { SetModifiersFlag(ModifiersFlag.NB, value); } }
        public bool ModifierNF { get { return Value.PlayModifiers.Flag.HasFlag(ModifiersFlag.NF); } set { SetModifiersFlag(ModifiersFlag.NF, value); } }
        public bool ModifierNO { get { return Value.PlayModifiers.Flag.HasFlag(ModifiersFlag.NO); } set { SetModifiersFlag(ModifiersFlag.NO, value); } }
        public bool ModifierOD { get { return Value.PlayModifiers.Flag.HasFlag(ModifiersFlag.OD); } set { SetModifiersFlag(ModifiersFlag.OD, value); } }
        public bool ModifierOP { get { return Value.PlayModifiers.Flag.HasFlag(ModifiersFlag.OP); } set { SetModifiersFlag(ModifiersFlag.OP, value); } }
        public bool ModifierPM { get { return Value.PlayModifiers.Flag.HasFlag(ModifiersFlag.PM); } set { SetModifiersFlag(ModifiersFlag.PM, value); } }
        public bool ModifierSC { get { return Value.PlayModifiers.Flag.HasFlag(ModifiersFlag.SC); } set { SetModifiersFlag(ModifiersFlag.SC, value); } }
        public bool ModifierSF { get { return Value.PlayModifiers.Flag.HasFlag(ModifiersFlag.SF); } set { SetModifiersFlag(ModifiersFlag.SF, value); } }
        public bool ModifierSS { get { return Value.PlayModifiers.Flag.HasFlag(ModifiersFlag.SS); } set { SetModifiersFlag(ModifiersFlag.SS, value); } }

        // Etc
        public bool IsShowCheckedOnly { get { return Value.EtcCheckedOnly.ShowCheckedOnly; } set { SetProperty(ref Value.EtcCheckedOnly.ShowCheckedOnly, value); } }

        void SetModifiersFlag(ModifiersFlag flag, bool on, [CallerMemberName] string? name = null)
        {
            if (on)
            {
                Value.PlayModifiers.Flag |= flag;
            }
            else
            {
                Value.PlayModifiers.Flag &= ~flag;
            }
            OnPropertyChanged(name);
        }

        public void UpdateValue(MainPageFilterValue other)
        {
            Value.CopyFrom(other);
            var type = typeof(MainPageFilterViewModel);
            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var prop in properties)
            {
                OnPropertyChanged(prop.Name);
            }
        }
    }
}
