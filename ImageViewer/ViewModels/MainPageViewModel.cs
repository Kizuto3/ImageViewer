using ImageViewer.Models;
using Microsoft.Win32;
using Prism.Commands;
using Prism.Mvvm;
using System.Collections.ObjectModel;

namespace ImageViewer.ViewModels
{
    /// <summary>
    /// The view model for the main page
    /// </summary>
    class MainPageViewModel : BindableBase
    {
        #region Private Members

        /// <summary>
        /// Current image that user sees
        /// </summary>
        private ImageModel _imageModel;

        /// <summary>
        /// Defines if list is visible to user or not
        /// </summary>
        private bool _isListVisible;

        /// <summary>
        /// Defines if edit bar is visible to user or not
        /// </summary>
        private bool _isEditBarVisible;

        #endregion

        #region Public Properties

        /// <summary>
        /// Current image that user sees
        /// </summary>
        public ImageModel ImageModel
        {
            get
            {
                return _imageModel;
            }
            set
            {
                SetProperty(ref _imageModel, value);
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
        public DelegateCommand SwitchListVisibilityComand { get; set; }

        /// <summary>
        /// The command that switches the visibility of the image list
        /// </summary>
        public DelegateCommand SwitchEditBarVisibilityComand { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        public MainPageViewModel()
        {
            _imageModel = new ImageModel();
            AddImagesCommand = new DelegateCommand(AddImages);
            SwitchListVisibilityComand = new DelegateCommand(SwitchListVisibility);
            SwitchEditBarVisibilityComand = new DelegateCommand(SwitchEditBarVisibility);
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
    }
}
