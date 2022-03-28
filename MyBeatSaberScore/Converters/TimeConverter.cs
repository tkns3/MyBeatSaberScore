using System;
using System.Globalization;
using System.Windows.Data;

namespace MyBeatSaberScore.Convertes
{
    public class TimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double v = (double)value;
            int sec = System.Convert.ToInt32(v);
            var span = new TimeSpan(0, 0, (int)sec);
            if (sec < 3600)
            {
                return span.ToString(@"m\:ss");
            }
            return span.ToString(@"h\:mm\:ss");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
