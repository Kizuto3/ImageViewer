using System;
using System.Globalization;
using System.Windows.Media.Imaging;

namespace ImageViewer.ValueConverters
{
    /// <summary>
    /// Converts the <see cref="string"/> to <see cref="BitmapImage"/>
    /// </summary>
    class StringPathToImageConverter : BaseValueConverter<StringPathToImageConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var fullPath = (string)value;

            if (string.IsNullOrEmpty(fullPath)) return null;

            try
            {
                BitmapImage image = new BitmapImage();
                image.BeginInit();
                image.UriSource = new Uri(fullPath);
                image.EndInit();

                return image;
            }
            catch (Exception)
            {
                return null;
            }
            
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
