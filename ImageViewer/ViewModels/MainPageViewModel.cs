using ImageViewer.Models;
using Microsoft.Win32;
using Prism.Commands;
using Prism.Mvvm;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using ImageViewer.DatabaseContext;
using System.IO;
using System.Linq;
using Prism.Events;
using ImageViewer.EventAggregators;
using CommonServiceLocator;
using Unity;
using System;

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
        /// Current page
        /// </summary>
        private PageModel _currentPage;

        /// <summary>
        /// Database context to manage database
        /// </summary>
        private readonly ApplicationContext _db;

        /// <summary>
        /// Event aggregator to publish <see cref="IdSentEvent"/> event
        /// </summary>
        private readonly IEventAggregator _ea;

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
        /// Current page
        /// </summary>
        public PageModel CurrentPage
        {
            get
            {
                return _currentPage;
            }
            set
            {
                SetProperty(ref _currentPage, value);
            }
        }

        public double LineThickness
        {
            get
            {
                return 5 / CurrentImage.ScaleY;
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
        public DelegateCommand AddImagesCommand { get; }

        /// <summary>
        /// The command to remove image from collection
        /// </summary>
        public DelegateCommand<ImageModel> RemoveImageCommand { get; }

        /// <summary>
        /// The command that switches the visibility of the image list
        /// </summary>
        public DelegateCommand SwitchListVisibilityCommand { get; }

        /// <summary>
        /// The command that switches the visibility of the image list
        /// </summary>
        public DelegateCommand SwitchEditBarVisibilityCommand { get; }

        /// <summary>
        /// The command to zoom image in
        /// </summary>
        public DelegateCommand ZoomInCommand { get; }

        /// <summary>
        /// The command to zoom image out
        /// </summary>
        public DelegateCommand ZoomOutCommand { get; }

        /// <summary>
        /// The command to rotate image left
        /// </summary>
        public DelegateCommand RotateLeftCommand { get; }

        /// <summary>
        /// The command to rotate image right
        /// </summary>
        public DelegateCommand RotateRightCommand { get; }

        /// <summary>
        /// The command to change current image
        /// </summary>
        public DelegateCommand SelectionChangedCommand { get; }

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        public MainPageViewModel()
        {
            AddImagesCommand = new DelegateCommand(AddImages);
            RemoveImageCommand = new DelegateCommand<ImageModel>(RemoveImage);
            SwitchListVisibilityCommand = new DelegateCommand(SwitchListVisibility);
            SwitchEditBarVisibilityCommand = new DelegateCommand(SwitchEditBarVisibility);
            ZoomInCommand = new DelegateCommand(ZoomIn);
            ZoomOutCommand = new DelegateCommand(ZoomOut);
            RotateLeftCommand = new DelegateCommand(RotateLeft);
            RotateRightCommand = new DelegateCommand(RotateRight);
            SelectionChangedCommand = new DelegateCommand(SelectionChanged);

            Images = new ObservableCollection<ImageModel>();

            _db = new ApplicationContext();

            foreach(var model in _db.GetImageModels().Result)
            {
                if (!File.Exists(model.FullPath)) continue;
                Images.Add(model);
            }

            var page = _db.GetPageModel();

            if(page == null)
            {
                CurrentPage = new PageModel(false, false, 0);
                _db.InsertPageModel(CurrentPage);
            }
            else
            {
                CurrentPage = page;
            }

            CurrentImage = _db.GetImageModel(CurrentPage.ImageModelID);

            var container = ServiceLocator.Current.GetInstance<IUnityContainer>();
            _ea = container.Resolve<IEventAggregator>();
        }

        #endregion

        #region Methods

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

            var regex = new Regex(@"([^\s]+(\.(?i)(jpg|jpeg|gif|tiff|bat|ico|png|heif|exif|bmp))$)", RegexOptions.IgnoreCase);

            if (open.ShowDialog() == true)
            {
                foreach(var file in open.FileNames)
                {
                    if (regex.IsMatch(file))
                    {
                        var image = new ImageModel(file);
                        if (Images.Contains(image)) continue;
                        _db.InsertImageModel(image);
                        Images.Add(_db.GetImageModels().Result.Last());
                    }
                }
            }
        }

        /// <summary>
        /// Swithes IsListVisible value to show or hide image list on UI
        /// </summary>
        public void SwitchListVisibility()
        {
            CurrentPage.IsListVisible = !CurrentPage.IsListVisible;

            _db.UpdatePageModel(CurrentPage);
        }

        /// <summary>
        /// Switches IsEditBarVisible value to show or hide edit bar on UI
        /// </summary>
        public void SwitchEditBarVisibility()
        {
            CurrentPage.IsEditBarVisible = !CurrentPage.IsEditBarVisible;

            _db.UpdatePageModel(CurrentPage);
        }

        /// <summary>
        /// Increases image scale by <see cref="ScaleIn"/> (1.1)
        /// </summary>
        public void ZoomIn()
        {
            CurrentImage.ScaleX *= ScaleIn;
            CurrentImage.ScaleY *= ScaleIn;

            _db.UpdateImageModel(CurrentImage);
        }

        /// <summary>
        /// Increases image scale by <see cref="ScaleOut"/> (0.9)
        /// </summary>
        public void ZoomOut()
        {
            CurrentImage.ScaleX *= ScaleOut;
            CurrentImage.ScaleY *= ScaleOut;

            _db.UpdateImageModel(CurrentImage);
        }

        /// <summary>
        /// Decreases image rotation angle by <see cref="RotationAngle"/> (90)
        /// </summary>
        public void RotateLeft()
        {
            CurrentImage.Angle -= RotationAngle;

            _db.UpdateImageModel(CurrentImage);
        }

        /// <summary>
        /// Increases image rotation angle by <see cref="RotationAngle"/> (90)
        /// </summary>
        public void RotateRight()
        {
            CurrentImage.Angle += RotationAngle;

            _db.UpdateImageModel(CurrentImage);
        }

        /// <summary>
        /// Removes ImageModel from list
        /// </summary>
        /// <param name="imageModel">ImageModel to remove</param>
        private void RemoveImage(ImageModel imageModel)
        {
            if(Images.Count == 0)
            {
                return;
            }
            var index = (Images.IndexOf(imageModel) - 1) >= 0 ? Images.IndexOf(imageModel) - 1 : 1;
            CurrentImage = Images.ElementAt(index >= Images.Count ? 0 : index);
            Images.Remove(imageModel);
            _db.RemoveImageModel(imageModel.ID);
        }

        /// <summary>
        /// Changes current image
        /// </summary>
        private void SelectionChanged()
        {
            CurrentPage.ImageModelID = CurrentImage != null ? CurrentImage.ID : CurrentPage.ImageModelID - 1;

            _ea.GetEvent<IdSentEvent>().Publish(CurrentPage.ImageModelID);

            _db.UpdatePageModel(CurrentPage);
        }

        #endregion
    }
}
