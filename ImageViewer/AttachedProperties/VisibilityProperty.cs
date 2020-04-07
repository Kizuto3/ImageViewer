using System.Windows;

namespace ImageViewer.AttachedProperties
{
    public class VisibilityProperty
    {
        /// <summary>
        /// Dependency property with boolean value. Flags the visibility of element
        /// </summary>
        public static readonly DependencyProperty ListVisibilityProperty = DependencyProperty.RegisterAttached(
            "ListVisibility",
            typeof(bool),
            typeof(VisibilityProperty),
            new FrameworkPropertyMetadata(false));

        public static bool GetListVisibility(FrameworkElement frameworkElement)
        {
            return (bool)frameworkElement.GetValue(ListVisibilityProperty);
        }

        public static void SetListVisibility(FrameworkElement frameworkElement, bool observe)
        {
            frameworkElement.SetValue(ListVisibilityProperty, observe);
        }
    }
}
