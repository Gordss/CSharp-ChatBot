using System;
using System.Globalization;
using System.Windows;

namespace ChatAssistant.Converter
{
    /// <summary>
    /// A converter that takes in bool and returns user or api response message horizontal alignment
    /// </summary>
    public class SentByUserAlignmentConverter : BaseValueConverter<SentByUserAlignmentConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value ? HorizontalAlignment.Right : HorizontalAlignment.Left;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
