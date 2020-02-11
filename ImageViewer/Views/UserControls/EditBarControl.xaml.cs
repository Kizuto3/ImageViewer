using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ImageViewer.Views.UserControls
{
    /// <summary>
    /// Interaction logic for EditBarControl.xaml
    /// </summary>
    public partial class EditBarControl : UserControl
    {
        /// <summary>
        /// Dependency property to set up command to draw a rectangle
        /// </summary>
        public DependencyProperty DrawRectangleProperty = DependencyProperty.Register(nameof(DrawRectangle), typeof(ICommand), typeof(EditBarControl), new PropertyMetadata(null));

        /// <summary>
        /// Dependency property to set up command to draw a circle
        /// </summary>
        public DependencyProperty DrawCircleProperty = DependencyProperty.Register(nameof(DrawCircle), typeof(ICommand), typeof(EditBarControl), new PropertyMetadata(null));

        /// <summary>
        /// Dependency property to set up command to draw a line
        /// </summary>
        public DependencyProperty DrawLineProperty = DependencyProperty.Register(nameof(DrawLine), typeof(ICommand), typeof(EditBarControl), new PropertyMetadata(null));

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

        public EditBarControl()
        {
            InitializeComponent();
        }
    }
}
