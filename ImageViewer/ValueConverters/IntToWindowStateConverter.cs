using System;
using System.Globalization;
using System.Windows;

namespace ImageViewer.ValueConverters
{
    class IntToWindowStateConverter : BaseValueConverter<IntToWindowStateConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var windowState = (int)value;

            switch (windowState)
            {
                case 0: return WindowState.Normal;
                case 1: return WindowState.Minimized;
                case 2: return WindowState.Maximized;

                default: return WindowState.Normal;
            }
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            WindowState windowState = (WindowState)value;

            switch (windowState)
            {
                case WindowState.Normal: return 0;
                case WindowState.Minimized: return 1;
                case WindowState.Maximized: return 2;

                default: return 0;
            }
        }
    }
}
