using System;
using System.Globalization;
using System.Windows;

namespace ChatAssistant.Converter
{
    /// <summary>
    /// A converter that takes in bool and converts the message corner radius either for  user or for api response
    /// </summary>
    public class SentByUserMessageCornerRadiusConverter : BaseValueConverter<SentByUserMessageCornerRadiusConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value ? new CornerRadius(15, 15, 0, 15) : new CornerRadius(15, 15, 15, 0);
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
