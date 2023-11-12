using MyBeatSaberScore.BeatMap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBeatSaberScore.Model
{
    public enum PlayResultType
    {
        Clear,
        Failure,
        NotPlay,
    }

    public abstract class FilterItemBase
    {
        public virtual bool IsShow(object obj)
        {
            return false;
        }
    }

    public class FilterLongRange : FilterItemBase
    {
        public long MinValue;
        public long MaxValue;
        public Func<object, long> GetFilterTargetValue;

        public FilterLongRange(long min, long max, Func<object, long> getFilterTargetValue)
        {
            MinValue = min;
            MaxValue = max;
            this.GetFilterTargetValue = getFilterTargetValue;
        }

        public override bool IsShow(object obj)
        {
            long value = GetFilterTargetValue(obj);
            return MinValue <= value && value < MaxValue;
        }

        public FilterLongRange CopyFrom(FilterLongRange other)
        {
            this.MinValue = other.MinValue;
            this.MaxValue = other.MaxValue;
            this.GetFilterTargetValue = other.GetFilterTargetValue;
            return this;
        }
    }

    public class FilterDoubleRange : FilterItemBase
    {
        public double MinValue;
        public double MaxValue;
        public Func<object, double> GetFilterTargetValue;

        public FilterDoubleRange(double min, double max, Func<object, double> getFilterTargetValue)
        {
            MinValue = min;
            MaxValue = max;
            this.GetFilterTargetValue = getFilterTargetValue;
        }

        public override bool IsShow(object obj)
        {
            double value = GetFilterTargetValue(obj);
            return MinValue <= value && value < MaxValue;
        }

        public FilterDoubleRange CopyFrom(FilterDoubleRange other)
        {
            this.MinValue = other.MinValue;
            this.MaxValue = other.MaxValue;
            this.GetFilterTargetValue = other.GetFilterTargetValue;
            return this;
        }
    }

    public class FilterDateTimeRange : FilterItemBase
    {
        public DateTimeOffset? MinValue;
        public DateTimeOffset? MaxValue;
        public Func<object, DateTimeOffset?> GetFilterTargetValue;

        public FilterDateTimeRange(DateTimeOffset? min, DateTimeOffset? max, Func<object, DateTimeOffset?> getFilterTargetValue)
        {
            MinValue = min;
            MaxValue = max;
            this.GetFilterTargetValue = getFilterTargetValue;
        }

        public override bool IsShow(object obj)
        {
            DateTimeOffset? value = GetFilterTargetValue(obj);
            if (value != null)
            {
                if (MinValue != null)
                {
                    if (value < MinValue)
                    {
                        return false;
                    }
                }

                if (MaxValue != null)
                {
                    if (value >= MaxValue)
                    {
                        return false;
                    }
                }
            }
            else
            {
                return (MinValue == null) && (MaxValue == null);
            }
            return true;
        }

        public FilterDateTimeRange CopyFrom(FilterDateTimeRange other)
        {
            this.MinValue = other.MinValue;
            this.MaxValue = other.MaxValue;
            this.GetFilterTargetValue = other.GetFilterTargetValue;
            return this;
        }
    }

    public class FilterStringSearch : FilterItemBase
    {
        public string SearchValue;
        public Func<object, string> GetFilterTargetValue;

        public FilterStringSearch(Func<object, string> getFilterTargetValue)
        {
            this.SearchValue = "";
            this.GetFilterTargetValue = getFilterTargetValue;
        }

        public override bool IsShow(object obj)
        {
            if (SearchValue.Length > 0)
            {
                string value = GetFilterTargetValue(obj);
                return value.Contains(SearchValue, StringComparison.OrdinalIgnoreCase);
            }
            return true;
        }

        public FilterStringSearch CopyFrom(FilterStringSearch other)
        {
            this.SearchValue = other.SearchValue;
            this.GetFilterTargetValue = other.GetFilterTargetValue;
            return this;
        }
    }

    public class FilterMapRankStatus : FilterItemBase
    {
        public bool ShowRanked = true;
        public bool ShowUnRanked = true;
        public Func<object, bool> GetFilterTargetMapIsRanked;

        public FilterMapRankStatus(Func<object, bool> getFilterTargetMapIsRanked)
        {
            this.GetFilterTargetMapIsRanked = getFilterTargetMapIsRanked;
        }

        public override bool IsShow(object obj)
        {
            bool value = GetFilterTargetMapIsRanked(obj);
            return (value && ShowRanked) || (!value && ShowUnRanked);
        }

        public FilterMapRankStatus CopyFrom(FilterMapRankStatus other)
        {
            this.ShowRanked = other.ShowRanked;
            this.ShowUnRanked = other.ShowUnRanked;
            this.GetFilterTargetMapIsRanked = other.GetFilterTargetMapIsRanked;
            return this;
        }
    }

    public class FilterMapMode : FilterItemBase
    {
        public bool ShowStandard = true;
        public bool ShowLawless = true;
        public bool ShowOneSaber = true;
        public bool ShowLightShow = true;
        public bool ShowDegree90 = true;
        public bool ShowDegree360 = true;
        public bool ShowNoArrows = true;
        public Func<object, BeatMapMode> GetFilterTargetValue;

        public FilterMapMode(Func<object, BeatMapMode> getFilterTargetValue)
        {
            this.GetFilterTargetValue = getFilterTargetValue;
        }

        public override bool IsShow(object obj)
        {
            BeatMapMode value = GetFilterTargetValue(obj);
            bool isShow = value switch
            {
                BeatMapMode.Standard => ShowStandard,
                BeatMapMode.Lawless => ShowLawless,
                BeatMapMode.OneSaber => ShowOneSaber,
                BeatMapMode.Lightshow => ShowLightShow,
                BeatMapMode.Degree90 => ShowDegree90,
                BeatMapMode.Degree360 => ShowDegree360,
                BeatMapMode.NoArrows => ShowNoArrows,
                _ => true,
            };
            return isShow;
        }

        public FilterMapMode CopyFrom(FilterMapMode other)
        {
            this.ShowStandard = other.ShowStandard;
            this.ShowLawless = other.ShowLawless;
            this.ShowOneSaber = other.ShowOneSaber;
            this.ShowLightShow = other.ShowLightShow;
            this.ShowDegree90 = other.ShowDegree90;
            this.ShowDegree360 = other.ShowDegree360;
            this.ShowNoArrows = other.ShowNoArrows;
            this.GetFilterTargetValue = other.GetFilterTargetValue;
            return this;
        }
    }

    public class FilterMapDifficulty : FilterItemBase
    {
        public bool ShowEasy = true;
        public bool ShowNormal = true;
        public bool ShowHard = true;
        public bool ShowExpert = true;
        public bool ShowExpertPlus = true;
        public Func<object, BeatMapDifficulty> GetFilterTargetValue;

        public FilterMapDifficulty(Func<object, BeatMapDifficulty> getFilterTargetValue)
        {
            this.GetFilterTargetValue = getFilterTargetValue;
        }

        public override bool IsShow(object obj)
        {
            BeatMapDifficulty value = GetFilterTargetValue(obj);
            bool isShow = value switch
            {
                BeatMapDifficulty.Easy => ShowEasy,
                BeatMapDifficulty.Normal => ShowNormal,
                BeatMapDifficulty.Hard => ShowHard,
                BeatMapDifficulty.Expert => ShowExpert,
                BeatMapDifficulty.ExpertPlus => ShowExpertPlus,
                _ => true,
            };
            return isShow;
        }

        public FilterMapDifficulty CopyFrom(FilterMapDifficulty other)
        {
            this.ShowEasy = other.ShowEasy;
            this.ShowNormal = other.ShowNormal;
            this.ShowHard = other.ShowHard;
            this.ShowExpert = other.ShowExpert;
            this.ShowExpertPlus = other.ShowExpertPlus;
            this.GetFilterTargetValue = other.GetFilterTargetValue;
            return this;
        }
    }

    public class FilterPlayResult : FilterItemBase
    {
        public bool ShowClear = true;
        public bool ShowFailure = true;
        public bool ShowNotPlay = true;
        public Func<object, PlayResultType> GetFilterTargetValue;

        public FilterPlayResult(Func<object, PlayResultType> getFilterTargetValue)
        {
            this.GetFilterTargetValue = getFilterTargetValue;
        }

        public override bool IsShow(object obj)
        {
            PlayResultType value = GetFilterTargetValue(obj);
            bool isShow = value switch
            {
                PlayResultType.Clear => ShowClear,
                PlayResultType.Failure => ShowFailure,
                PlayResultType.NotPlay => ShowNotPlay,
                _ => true,
            };
            return isShow;
        }

        public FilterPlayResult CopyFrom(FilterPlayResult other)
        {
            this.ShowClear = other.ShowClear;
            this.ShowFailure = other.ShowFailure;
            this.ShowNotPlay = other.ShowNotPlay;
            this.GetFilterTargetValue = other.GetFilterTargetValue;
            return this;
        }
    }

    public class FilterPlayFullCombo : FilterItemBase
    {
        public bool ShowFullCombo = true;
        public bool ShowNotFullCombo = true;
        public Func<object, bool> GetFilterTargetPlayIsFullCombo;

        public FilterPlayFullCombo(Func<object, bool> getFilterTargetPlayIsFullCombo)
        {
            this.GetFilterTargetPlayIsFullCombo = getFilterTargetPlayIsFullCombo;
        }

        public override bool IsShow(object obj)
        {
            bool value = GetFilterTargetPlayIsFullCombo(obj);
            return (value && ShowFullCombo) || (!value && ShowNotFullCombo);
        }

        public FilterPlayFullCombo CopyFrom(FilterPlayFullCombo other)
        {
            this.ShowFullCombo = other.ShowFullCombo;
            this.ShowNotFullCombo = other.ShowNotFullCombo;
            return this;
        }
    }

    public class FilterPlayModifiers : FilterItemBase
    {
        public bool Any = true;
        public bool None = false;
        public ModifiersFlag Flag;
        public Func<object, ModifiersFlag> GetFilterTargetValue;

        public FilterPlayModifiers(Func<object, ModifiersFlag> getFilterTargetValue)
        {
            this.GetFilterTargetValue = getFilterTargetValue;
        }

        public override bool IsShow(object obj)
        {
            ModifiersFlag modifiers = GetFilterTargetValue(obj);
            if (modifiers == Flag)
            {
                if (Flag == 0)
                {
                    if (None && Any) // フィルター選択状況は「None + Any」がオン
                    {
                        return true;
                    }
                    else if (Any) // フィルター選択状況は「Any」がオン
                    {
                        return true;
                    }
                    else if (None) // フィルター選択状況は「None」がオン
                    {
                        return true;
                    }
                    else // フィルター選択状況はすべてオフ
                    {
                        return false;
                    }
                }
                else
                {
                    if (None && Any) // フィルター選択状況は「None + Any + Modifiers1つ以上」がオン
                    {
                        return false;
                    }
                    else if (Any) // フィルター選択状況は「Any + Modifiers1つ以上」がオン
                    {
                        return true;
                    }
                    else if (None) // フィルター選択状況は「None + Modifiers1つ以上」がオン
                    {
                        return false;
                    }
                    else // フィルター選択状況は「Modifiers1つ以上」がオン
                    {
                        return true;
                    }
                }
            }
            else if (modifiers.HasFlag(Flag))
            {
                if (None && Any) // フィルター選択状況は「None + Any + Modifiers1つ以上」がオン
                {
                    return false;
                }
                else if (Any) // フィルター選択状況は「Any + Modifiers1つ以上」がオン
                {
                    return true;
                }
                else if (None) // フィルター選択状況は「None + Modifiers1つ以上」がオン
                {
                    return false;
                }
                else // フィルター選択状況は「Modifiers1つ以上」がオン
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public FilterPlayModifiers CopyFrom(FilterPlayModifiers other)
        {
            this.Any = other.Any;
            this.None = other.None;
            this.Flag = other.Flag;
            this.GetFilterTargetValue = other.GetFilterTargetValue;
            return this;
        }
    }

    public class FilterEtcCheckedOnly : FilterItemBase
    {
        public bool ShowCheckedOnly = true;
        public Func<object, bool> GetFilterTargetPlayIsFullCombo;

        public FilterEtcCheckedOnly(Func<object, bool> getFilterTargetPlayIsFullCombo)
        {
            this.GetFilterTargetPlayIsFullCombo = getFilterTargetPlayIsFullCombo;
        }

        public override bool IsShow(object obj)
        {
            bool value = GetFilterTargetPlayIsFullCombo(obj);
            return ShowCheckedOnly ? value : true;
        }

        public FilterEtcCheckedOnly CopyFrom(FilterEtcCheckedOnly other)
        {
            this.ShowCheckedOnly = other.ShowCheckedOnly;
            this.GetFilterTargetPlayIsFullCombo = other.GetFilterTargetPlayIsFullCombo;
            return this;
        }
    }
}
