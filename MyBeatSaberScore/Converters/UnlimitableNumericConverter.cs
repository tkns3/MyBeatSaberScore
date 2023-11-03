using System;
using System.CodeDom;
using System.Globalization;
using System.Windows.Data;

namespace MyBeatSaberScore.Convertes
{
    public class UnlimitableNumericConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch (parameter.ToString())
            {
                case "LongMin":
                    if ((long)value == long.MinValue) return "";
                    break;
                case "LongMax":
                    if ((long)value == long.MaxValue) return "";
                    break;
                case "DoubleMin":
                    if ((double)value == double.MinValue) return "";
                    break;
                case "DoubleMax":
                    if ((double)value == double.MaxValue) return "";
                    break;
                case "StarMin":
                    if ((double)value == double.MinValue || (double)value == 0d) return "";
                    break;
                case "StarMax":
                    if ((double)value == double.MaxValue || (double)value == 20d) return "";
                    break;
            }
            return $"{value}";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string s)
            {
                switch (parameter.ToString())
                {
                    case "LongMin":
                        if (!long.TryParse(s, out long _)) return long.MinValue;
                        break;
                    case "LongMax":
                        if (!long.TryParse(s, out long _)) return long.MaxValue;
                        break;
                    case "DoubleMin":
                        if (!double.TryParse(s, out double _)) return double.MinValue;
                        break;
                    case "DoubleMax":
                        if (!double.TryParse(s, out double _)) return double.MaxValue;
                        break;
                    case "StarMin":
                        if (!double.TryParse(s, out double _)) return 0d;
                        break;
                    case "StarMax":
                        if (!double.TryParse(s, out double _)) return 20d;
                        break;
                }
            }
            return value;
        }
    }
}
