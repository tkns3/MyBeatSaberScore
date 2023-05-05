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
                    if ((long)value == long.MinValue) return "Unlimited";
                    break;
                case "LongMax":
                    if ((long)value == long.MaxValue) return "Unlimited";
                    break;
                case "DoubleMin":
                    if ((double)value == double.MinValue) return "Unlimited";
                    break;
                case "DoubleMax":
                    if ((double)value == double.MaxValue) return "Unlimited";
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
                }
            }
            return value;
        }
    }
}
