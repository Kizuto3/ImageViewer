using System;
using System.Globalization;
using System.Windows.Media;

namespace ImageViewer.ValueConverters
{
    class StringToColorConverter : BaseValueConverter<StringToColorConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var color = value as string;
            return (Color)ColorConverter.ConvertFromString(color);
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value.ToString();
        }
    }
}
