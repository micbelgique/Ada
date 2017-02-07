using System;
using Windows.UI.Xaml.Data;

namespace MartineobotIOTMvvm.Converters
{
    public class LongToDateStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
          
            return new DateTime(1970, 1, 1).Add(TimeSpan.FromMilliseconds((long)value)).AddHours(2).ToString("dd/MM/yyyy hh:mm:ss tt");
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}