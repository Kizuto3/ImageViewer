using ImageViewer.Abstractions;
using ImageViewer.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
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
        /// Dependency property to set up a shape to be drawn
        /// </summary>
        public static DependencyProperty DrawingShapeProperty = DependencyProperty.Register(nameof(DrawingShape), typeof(ShapeType), typeof(ImagePresenterControl), new PropertyMetadata(ShapeType.None));

        /// <summary>
        /// Dependency property to set up a shape to be drawn
        /// </summary>
        public static DependencyProperty BehaviorTypeProperty = DependencyProperty.Register(nameof(BehaviorType), typeof(BehaviorType), typeof(ImagePresenterControl), new PropertyMetadata(BehaviorType.None));

        /// <summary>
        /// Dependency property to set up a main color of shape to draw
        /// </summary>
        public DependencyProperty MainColorProperty = DependencyProperty.Register(nameof(MainColor), typeof(Color), typeof(ImagePresenterControl), new PropertyMetadata(Colors.White));

        /// <summary>
        /// Dependency property to set up a border color of shape to draw
        /// </summary>
        public DependencyProperty BorderColorProperty = DependencyProperty.Register(nameof(BorderColor), typeof(Color), typeof(ImagePresenterControl), new PropertyMetadata(Colors.White));

        /// <summary>
        /// Dependency property to set up a background color opacity of a shape to draw
        /// </summary>
        public DependencyProperty BackgroundOpacityProperty = DependencyProperty.Register(nameof(BackgroundOpacity), typeof(double), typeof(ImagePresenterControl), new PropertyMetadata(1d));

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

        /// <summary>
        /// Shape to be drawn
        /// </summary>
        public ShapeType DrawingShape
        {
            get => (ShapeType)GetValue(DrawingShapeProperty);
            set => SetValue(DrawingShapeProperty, value);
        }

        /// <summary>
        /// Behavior to use
        /// </summary>
        public BehaviorType BehaviorType
        {
            get => (BehaviorType)GetValue(BehaviorTypeProperty);
            set => SetValue(BehaviorTypeProperty, value);
        }

        /// <summary>
        /// Main color of shape to draw
        /// </summary>
        public Color MainColor
        {
            get => (Color)GetValue(MainColorProperty);
            set => SetValue(MainColorProperty, value);
        }

        /// <summary>
        /// Border color of shape to draw
        /// </summary>
        public Color BorderColor
        {
            get => (Color)GetValue(BorderColorProperty);
            set => SetValue(BorderColorProperty, value);
        }

        /// <summary>
        /// Border color of shape to draw
        /// </summary>
        public double BackgroundOpacity
        {
            get => (double)GetValue(BackgroundOpacityProperty);
            set => SetValue(BackgroundOpacityProperty, value);
        }

        /// <summary>
        /// ScrollViewer that holds image
        /// </summary>
        ScrollViewer scrollViewer;

        static ImagePresenterControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ImagePresenterControl), new FrameworkPropertyMetadata(typeof(ImagePresenterControl)));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            scrollViewer = GetTemplateChild("ScrollElement") as ScrollViewer;

            scrollViewer.PreviewMouseWheel += ScrollViewer_PreviewMouseWheel;
        }

        private void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            var context = DataContext as MainPageViewModel;

            if (Keyboard.Modifiers == ModifierKeys.Control)
            {
                if (e.Delta > 0)
                {
                    context.ZoomIn();
                }
                else
                {
                    context.ZoomOut();
                }
                e.Handled = true;
            }
            if (Keyboard.Modifiers == ModifierKeys.Alt)
            {
                if (e.Delta > 0)
                {
                    scrollViewer.LineRight();
                }
                else
                {
                    scrollViewer.LineLeft();
                }
                e.Handled = true;
            }
        }
    }
}
