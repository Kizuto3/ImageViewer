using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ImageViewer.Views.UserControls
{
    /// <summary>
    /// Interaction logic for BottomBarControl.xaml
    /// </summary>
    public partial class BottomBarControl : UserControl
    {
        #region Properties

        /// <summary>
        /// Dependency property to set up command to save an image
        /// </summary>
        public DependencyProperty SaveImageProperty = DependencyProperty.Register(nameof(SaveImage), typeof(ICommand), typeof(BottomBarControl), new PropertyMetadata(null));

        /// <summary>
        /// Dependency property to set up command to copy an image to clipboard
        /// </summary>
        public DependencyProperty CopyImageProperty = DependencyProperty.Register(nameof(CopyImage), typeof(ICommand), typeof(BottomBarControl), new PropertyMetadata(null));

        /// <summary>
        /// Dependency property to set up command to crop an image
        /// </summary>
        public DependencyProperty CropImageProperty = DependencyProperty.Register(nameof(CropImage), typeof(ICommand), typeof(BottomBarControl), new PropertyMetadata(null));

        /// <summary>
        /// Dependency property to set up command to remove crop from an image
        /// </summary>
        public DependencyProperty RemoveCropProperty = DependencyProperty.Register(nameof(RemoveCrop), typeof(ICommand), typeof(BottomBarControl), new PropertyMetadata(null));

        /// <summary>
        /// Dependency property to set up command to select an area to crop
        /// </summary>
        public DependencyProperty SelectAreaProperty = DependencyProperty.Register(nameof(SelectArea), typeof(ICommand), typeof(BottomBarControl), new PropertyMetadata(null));

        /// <summary>
        /// Command to save an image
        /// </summary>
        public ICommand SaveImage
        {
            get => (ICommand)GetValue(SaveImageProperty);
            set => SetValue(SaveImageProperty, value);
        }

        /// <summary>
        /// Command to copy an image
        /// </summary>
        public ICommand CopyImage
        {
            get => (ICommand)GetValue(CopyImageProperty);
            set => SetValue(CopyImageProperty, value);
        }

        /// <summary>
        /// Command to crop an image
        /// </summary>
        public ICommand CropImage
        {
            get => (ICommand)GetValue(CropImageProperty);
            set => SetValue(CropImageProperty, value);
        }

        /// <summary>
        /// Command to remove crop from an image
        /// </summary>
        public ICommand RemoveCrop
        {
            get => (ICommand)GetValue(RemoveCropProperty);
            set => SetValue(RemoveCropProperty, value);
        }

        /// <summary>
        /// Command to select area to crop
        /// </summary>
        public ICommand SelectArea
        {
            get => (ICommand)GetValue(SelectAreaProperty);
            set => SetValue(SelectAreaProperty, value);
        }

        #endregion

        public BottomBarControl()
        {
            InitializeComponent();
        }
    }
}
