using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace MyBeatSaberScore.Convertes
{
    public class EnumToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var ParameterString = parameter as string;
            if (ParameterString == null)
            {
                return Binding.DoNothing;
            }

            if (Enum.IsDefined(value.GetType(), value) == false)
            {
                return Binding.DoNothing;
            }

            object paramvalue = Enum.Parse(value.GetType(), ParameterString);

            return (int)paramvalue == (int)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var parameterString = parameter as string;
            if (null == parameterString)
                return Binding.DoNothing;

            if (true.Equals(value))
                return Enum.Parse(targetType, parameterString);
            else
                return Binding.DoNothing;
        }
    }
}
