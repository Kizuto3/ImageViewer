using ImageViewer.Models;
using Microsoft.Win32;
using Prism.Commands;
using Prism.Mvvm;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ImageViewer.ViewModels
{
    /// <summary>
    /// The view model for the main page
    /// </summary>
    class MainPageViewModel : BindableBase
    {
        private const double ScaleIn = 1.1;

        private const double ScaleOut = 0.9;

        private const double RotationAngle = 90;

        #region Private Members

        /// <summary>
        /// Current image that user sees
        /// </summary>
        private ImageModel _currentImage;

        /// <summary>
        /// Defines if list is visible to user or not
        /// </summary>
        private bool _isListVisible;

        /// <summary>
        /// Defines if edit bar is visible to user or not
        /// </summary>
        private bool _isEditBarVisible;

        private double _cropStartX;

        private double _cropStartY;

        private double _cropWidth;

        private double _cropHeight;

        #endregion

        #region Public Properties

        /// <summary>
        /// Current image that user sees
        /// </summary>
        public ImageModel CurrentImage
        {
            get
            {
                return _currentImage;
            }
            set
            {
                SetProperty(ref _currentImage, value);
            }
        }

        /// <summary>
        /// Defines if list is visible to user or not
        /// </summary>
        public bool IsListVisible
        {
            get
            {
                return _isListVisible;
            }
            set
            {
                SetProperty(ref _isListVisible, value);
            }
        }

        /// <summary>
        /// Defines if edit bar is visible to user or not
        /// </summary>
        public bool IsEditBarVisible
        {
            get
            {
                return _isEditBarVisible;
            }
            set
            {
                SetProperty(ref _isEditBarVisible, value);
            }
        }

        public double CropStartX
        {
            get
            {
                return _cropStartX;
            }
            set
            {
                SetProperty(ref _cropStartX, value);
            }
        }

        public double CropStartY
        {
            get
            {
                return _cropStartY;
            }
            set
            {
                SetProperty(ref _cropStartY, value);
            }
        }

        public double CropWidth
        {
            get
            {
                return _cropWidth;
            }
            set
            {
                SetProperty(ref _cropWidth, value);
            }
        }

        public double CropHeight
        {
            get
            {
                return _cropHeight;
            }
            set
            {
                SetProperty(ref _cropHeight, value);
            }
        }

        /// <summary>
        /// All images that user wants to watch
        /// </summary>
        public ObservableCollection<ImageModel> Images { get; set;}

        #endregion

        #region Commands

        /// <summary>
        /// The command to add images to collection
        /// </summary>
        public DelegateCommand AddImagesCommand { get; set; }

        /// <summary>
        /// The command that switches the visibility of the image list
        /// </summary>
        public DelegateCommand SwitchListVisibilityCommand { get; set; }

        /// <summary>
        /// The command that switches the visibility of the image list
        /// </summary>
        public DelegateCommand SwitchEditBarVisibilityCommand { get; set; }

        /// <summary>
        /// The command to zoom image in
        /// </summary>
        public DelegateCommand ZoomInCommand { get; set; }

        /// <summary>
        /// The command to zoom image out
        /// </summary>
        public DelegateCommand ZoomOutCommand { get; set; }

        /// <summary>
        /// The command to rotate image left
        /// </summary>
        public DelegateCommand RotateLeftCommand { get; set; }

        /// <summary>
        /// The command to rotate image right
        /// </summary>
        public DelegateCommand RotateRightCommand { get; set; }

        /// <summary>
        /// The command to rotate image right
        /// </summary>
        public DelegateCommand PopCommand { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        public MainPageViewModel()
        {
            _currentImage = new ImageModel();

            AddImagesCommand = new DelegateCommand(AddImages);
            SwitchListVisibilityCommand = new DelegateCommand(SwitchListVisibility);
            SwitchEditBarVisibilityCommand = new DelegateCommand(SwitchEditBarVisibility);
            ZoomInCommand = new DelegateCommand(ZoomIn);
            ZoomOutCommand = new DelegateCommand(ZoomOut);
            RotateLeftCommand = new DelegateCommand(RotateLeft);
            RotateRightCommand = new DelegateCommand(RotateRight);
            PopCommand = new DelegateCommand(Pop);

            Images = new ObservableCollection<ImageModel>();
        }

        #endregion

        /// <summary>
        /// Add images to observable collection
        /// </summary>
        public void AddImages()
        {
            OpenFileDialog open = new OpenFileDialog()
            {
                Filter = "Image Files(*.jpg; *.jpeg; *.gif; *.bmp; *png)|*.jpg; *.jpeg; *.gif; *.bmp; *png|All files (*.*)|*.*",
                Multiselect = true
            };
            if (open.ShowDialog() == true)
            {
                foreach(var file in open.FileNames)
                {
                    var image = new ImageModel(file);
                    if (Images.Contains(image)) continue;
                    Images.Add(image);
                }
            }
        }

        /// <summary>
        /// Swithes IsListVisible value to show or hide image list on UI
        /// </summary>
        public void SwitchListVisibility()
        {
            IsListVisible = !IsListVisible;
        }

        /// <summary>
        /// Switches IsEditBarVisible value to show or hide edit bar on UI
        /// </summary>
        public void SwitchEditBarVisibility()
        {
            IsEditBarVisible = !IsEditBarVisible;
        }

        /// <summary>
        /// Increases image scale by <see cref="ScaleIn"/> (1.1)
        /// </summary>
        public void ZoomIn()
        {
            CurrentImage.ScaleX *= ScaleIn;
            CurrentImage.ScaleY *= ScaleIn;
        }

        /// <summary>
        /// Increases image scale by <see cref="ScaleOut"/> (0.9)
        /// </summary>
        public void ZoomOut()
        {
            CurrentImage.ScaleX *= ScaleOut;
            CurrentImage.ScaleY *= ScaleOut;
        }

        /// <summary>
        /// Decreases image rotation angle by <see cref="RotationAngle"/> (90)
        /// </summary>
        public void RotateLeft()
        {
            CurrentImage.Angle -= RotationAngle;
        }

        /// <summary>
        /// Increases image rotation angle by <see cref="RotationAngle"/> (90)
        /// </summary>
        public void RotateRight()
        {
            CurrentImage.Angle += RotationAngle;
        }

        public void Pop()
        {
            MessageBox.Show("Lalala");
        }
    }
}
