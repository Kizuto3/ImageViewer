using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ImageViewer.Views.CustomControls
{
    public class ImagePresenterControl : Control
    {
        /// <summary>
        /// Dependency property to set up an image source
        /// </summary>
        public static DependencyProperty ImageSourceProperty = DependencyProperty.Register(nameof(ImageSource), typeof(ImageSource), typeof(ImagePresenterControl), new PropertyMetadata());

        /// <summary>
        /// Dependency property to set up an image scale 
        /// </summary>
        public static DependencyProperty ScaleXProperty = DependencyProperty.Register(nameof(ScaleX), typeof(double), typeof(ImagePresenterControl), new PropertyMetadata(1d));

        /// <summary>
        /// Dependency property to set up an image scale
        /// </summary>
        public static DependencyProperty ScaleYProperty = DependencyProperty.Register(nameof(ScaleY), typeof(double), typeof(ImagePresenterControl), new PropertyMetadata(1d));

        /// <summary>
        /// Dependency property to set up a rotation angle of the image
        /// </summary>
        public static DependencyProperty RotationAngleProperty = DependencyProperty.Register(nameof(RotationAngle), typeof(double), typeof(ImagePresenterControl), new PropertyMetadata(0d));

        /// <summary>
        /// Dependency property to set up a line thickness
        /// </summary>
        public static DependencyProperty LineThicknessProperty = DependencyProperty.Register(nameof(LineThickness), typeof(double), typeof(ImagePresenterControl), new PropertyMetadata(1d));

        /// <summary>
        /// An image source
        /// </summary>
        public ImageSource ImageSource
        {
            get => (ImageSource)GetValue(ImageSourceProperty);
            set => SetValue(ImageSourceProperty, value);
        }

        /// <summary>
        /// An image scale
        /// </summary>
        public double ScaleX
        {
            get => (double)GetValue(ScaleXProperty);
            set => SetValue(ScaleXProperty, value);
        }

        /// <summary>
        /// An image scale
        /// </summary>
        public double ScaleY
        {
            get => (double)GetValue(ScaleYProperty);
            set => SetValue(ScaleYProperty, value);
        }

        /// <summary>
        /// An image rotation angle
        /// </summary>
        public double RotationAngle
        {
            get => (double)GetValue(RotationAngleProperty);
            set => SetValue(RotationAngleProperty, value);
        }

        /// <summary>
        /// A line thickness
        /// </summary>
        public double LineThickness
        {
            get => (double)GetValue(LineThicknessProperty);
            set => SetValue(LineThicknessProperty, value);
        }

        static ImagePresenterControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ImagePresenterControl), new FrameworkPropertyMetadata(typeof(ImagePresenterControl)));
        }
    }
}
