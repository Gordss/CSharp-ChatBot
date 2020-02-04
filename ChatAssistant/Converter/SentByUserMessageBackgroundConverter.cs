using System;
using System.Globalization;
using System.Windows.Media;

namespace ChatAssistant.Converter
{
    /// <summary>
    /// A converter that takes in bool and converts the message color either for user or for api response
    /// </summary>
    public class SentByUserMessageBackgroundConverter : BaseValueConverter<SentByUserMessageBackgroundConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value ? new BrushConverter().ConvertFrom("#388E3C") : new BrushConverter().ConvertFrom("#7f7f7f");
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
