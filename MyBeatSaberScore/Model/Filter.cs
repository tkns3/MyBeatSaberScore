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

    internal abstract class FilterItemBase
    {
        public virtual bool IsShow(object obj)
        {
            return false;
        }
    }

    internal class FilterLongRange : FilterItemBase
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
    }

    internal class FilterDoubleRange : FilterItemBase
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
    }

    internal class FilterDateTimeRange : FilterItemBase
    {
        public DateTime? MinValue;
        public DateTime? MaxValue;
        public Func<object, DateTime?> GetFilterTargetValue;

        public FilterDateTimeRange(DateTime? min, DateTime? max,Func<object, DateTime?> getFilterTargetValue)
        {
            MinValue = min;
            MaxValue = max;
            this.GetFilterTargetValue = getFilterTargetValue;
        }

        public override bool IsShow(object obj)
        {
            DateTime? value = GetFilterTargetValue(obj);
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
                    if (value > MaxValue)
                    {
                        return false;
                    }
                }
            }
            return true;
        }
    }

    internal class FilterStringSearch : FilterItemBase
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
    }

    internal class FilterMapRankStatus : FilterItemBase
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
    }

    internal class FilterMapMode : FilterItemBase
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
    }

    internal class FilterMapDifficulty : FilterItemBase
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
    }

    internal class FilterPlayResult : FilterItemBase
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
    }

    internal class FilterPlayFullCombo : FilterItemBase
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
    }

    internal class FilterPlayModifiers : FilterItemBase
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
    }

    internal class FilterEtcCheckedOnly : FilterItemBase
    {
        public bool ShowChecked = true;
        public Func<object, bool> GetFilterTargetPlayIsFullCombo;

        public FilterEtcCheckedOnly(Func<object, bool> getFilterTargetPlayIsFullCombo)
        {
            this.GetFilterTargetPlayIsFullCombo = getFilterTargetPlayIsFullCombo;
        }

        public override bool IsShow(object obj)
        {
            bool value = GetFilterTargetPlayIsFullCombo(obj);
            return ShowChecked ? value : true;
        }
    }
}
