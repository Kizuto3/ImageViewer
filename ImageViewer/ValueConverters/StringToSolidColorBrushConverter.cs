using System;
using System.Globalization;
using System.Windows.Media;

namespace ImageViewer.ValueConverters
{
    class StringToSolidColorBrushConverter : BaseValueConverter<StringToSolidColorBrushConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var colorStr = value as string;
            var color = (Color)ColorConverter.ConvertFromString(colorStr);
            return new SolidColorBrush(color);
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value.ToString();
        }
    }
}
