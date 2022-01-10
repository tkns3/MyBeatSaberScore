using System;
using System.Globalization;
using System.Windows.Data;

namespace MyBeatSaberScore.Convertes
{
    public class BsrConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            long v = (long)value;
            return (v > 0) ? $"{v:x}" : "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
