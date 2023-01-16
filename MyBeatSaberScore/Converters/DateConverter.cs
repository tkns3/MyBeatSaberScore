using System;
using System.Globalization;
using System.Windows.Data;

namespace MyBeatSaberScore.Convertes
{
    public class DateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var v = (DateTime?)value;
            if (v != null)
            {
                return v?.ToLocalTime().ToString("yyyy/MM/dd HH:mm:ss (ddd)") ?? "";
            }
            else
            {
                return "";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
