using System;
using System.Globalization;

namespace ChatAssistant.Converter
{
    /// <summary>
    /// A converter that takes in date and converts it to a user friendly time
    /// </summary>
    public class TimeToDisplayTimeConverter : BaseValueConverter<TimeToDisplayTimeConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //get the time
            var time = (DateTimeOffset)value;

            //if it's today
            if (time.Date == DateTimeOffset.Now.Date)
            {
                return time.ToLocalTime().ToString("HH:mm");
            }
            //otherwise
            return time.ToLocalTime().ToString("HH:mm, MMM yyyy");
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
