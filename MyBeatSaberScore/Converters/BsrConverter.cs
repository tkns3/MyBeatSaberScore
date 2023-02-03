using System;
using System.Globalization;
using System.Windows.Data;
using static MyBeatSaberScore.PageMain;

namespace MyBeatSaberScore.Convertes
{
    public class BsrConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            NumOfKey v = (NumOfKey)value;
            return (v.Key > 0) ? (v.IsDeleted) ? $"({v.Key:x})" : $"{v.Key:x}" : "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
