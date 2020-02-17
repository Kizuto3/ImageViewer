using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace ImageViewer.Views.UserControls
{
    /// <summary>
    /// Interaction logic for EditBarControl.xaml
    /// </summary>
    public partial class EditBarControl : UserControl
    {
        #region Properties

        /// <summary>
        /// Dependency property to set up a command to draw a rectangle
        /// </summary>
        public DependencyProperty DrawRectangleProperty = DependencyProperty.Register(nameof(DrawRectangle), typeof(ICommand), typeof(EditBarControl), new PropertyMetadata(null));

        /// <summary>
        /// Dependency property to set up a command to draw a circle
        /// </summary>
        public DependencyProperty DrawCircleProperty = DependencyProperty.Register(nameof(DrawCircle), typeof(ICommand), typeof(EditBarControl), new PropertyMetadata(null));

        /// <summary>
        /// Dependency property to set up a command to draw a line
        /// </summary>
        public DependencyProperty DrawLineProperty = DependencyProperty.Register(nameof(DrawLine), typeof(ICommand), typeof(EditBarControl), new PropertyMetadata(null));

        /// <summary>
        /// Dependency property to set up a command to draw a polyline
        /// </summary>
        public DependencyProperty DrawPolylineProperty = DependencyProperty.Register(nameof(DrawPolyline), typeof(ICommand), typeof(EditBarControl), new PropertyMetadata(null));

        /// <summary>
        /// Dependency property to set up a main color of a shape to draw
        /// </summary>
        public DependencyProperty MainColorProperty = DependencyProperty.Register(nameof(MainColor), typeof(Color), typeof(EditBarControl), new PropertyMetadata(Colors.White));

        /// <summary>
        /// Dependency property to set up a border color of a shape to draw
        /// </summary>
        public DependencyProperty BorderColorProperty = DependencyProperty.Register(nameof(BorderColor), typeof(Color), typeof(EditBarControl), new PropertyMetadata(Colors.White));

        /// <summary>
        /// Dependency property to set up a background color opacity of a shape to draw
        /// </summary>
        public DependencyProperty BackgroundOpacityProperty = DependencyProperty.Register(nameof(BackgroundOpacity), typeof(double), typeof(EditBarControl), new PropertyMetadata(1d));

        /// <summary>
        /// Command to draw a rectangle
        /// </summary>
        public ICommand DrawRectangle
        {
            get => (ICommand)GetValue(DrawRectangleProperty);
            set => SetValue(DrawRectangleProperty, value);
        }

        /// <summary>
        /// Command to draw a circle
        /// </summary>
        public ICommand DrawCircle
        {
            get => (ICommand)GetValue(DrawCircleProperty);
            set => SetValue(DrawCircleProperty, value);
        }

        /// <summary>
        /// Command to draw a line
        /// </summary>
        public ICommand DrawLine
        {
            get => (ICommand)GetValue(DrawLineProperty);
            set => SetValue(DrawLineProperty, value);
        }

        /// <summary>
        /// Command to draw a polyline
        /// </summary>
        public ICommand DrawPolyline
        {
            get => (ICommand)GetValue(DrawPolylineProperty);
            set => SetValue(DrawPolylineProperty, value);
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

        #endregion

        public EditBarControl()
        {
            InitializeComponent();
        }
    }
}
