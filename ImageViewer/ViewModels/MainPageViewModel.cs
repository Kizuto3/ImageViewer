using CommonServiceLocator;
using ImageViewer.DatabaseContext;
using ImageViewer.EventAggregators;
using ImageViewer.Models;
using Microsoft.Win32;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Unity;

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
        /// Current page settings
        /// </summary>
        private PageModel _currentPage;

        /// <summary>
        /// DataContext to manage database
        /// </summary>
        private readonly ApplicationContext db;

        /// <summary>
        /// Event aggregator to publish <see cref="IdSentEvent"/> event
        /// </summary>
        private IEventAggregator _ea;

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
        /// Gets or set`s current page
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

        /// <summary>
        /// All images that user wants to watch
        /// </summary>
        public IEnumerable<ImageModel> Images { get; set;} 

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

            db = new ApplicationContext();
            db.ImageModels.LoadAsync();
            db.PageModels.LoadAsync();

            for (var i = 0; i < db.ImageModels.Local.Count; i++)
            {
                if (!File.Exists(db.ImageModels.Local[i].Fullpath)) db.ImageModels.Local.RemoveAt(i--);
            }

            var page = db.PageModels.Local.FirstOrDefault();

            if (page == null)
            {
                _currentPage = new PageModel(false, false, 0);
                db.PageModels.Local.Add(_currentPage);
            }
            else
            {
                _currentPage = page;
            }

            _currentImage = db.ImageModels.Local.FirstOrDefault(img => img.ID == CurrentPage.ImageModelID);
            Images = db.ImageModels.Local.ToBindingList();

            db.SaveChangesAsync();

            var container = ServiceLocator.Current.GetInstance<IUnityContainer>();
            _ea = container.Resolve<IEventAggregator>();
            _ea.GetEvent<IdSentEvent>().Publish(CurrentPage.ImageModelID);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Add images to database
        /// </summary>
        private void AddImages()
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
                        if (db.ImageModels.Local.Contains(image)) continue;
                        db.ImageModels.Add(image);
                    }
                }
                db.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Removes ImageModel from list
        /// </summary>
        /// <param name="imageModel">ImageModel to remove</param>
        private void RemoveImage(ImageModel imageModel)
        {
            db.ImageModels.Local.Remove(imageModel);
            db.SaveChangesAsync();
        }

        /// <summary>
        /// Swithes IsListVisible value to show or hide image list on UI
        /// </summary>
        private void SwitchListVisibility()
        {
            CurrentPage.IsListVisible = !CurrentPage.IsListVisible;

            db.Entry(CurrentPage).State = EntityState.Modified;
            db.SaveChanges();
        }

        /// <summary>
        /// Switches IsEditBarVisible value to show or hide edit bar on UI
        /// </summary>
        private void SwitchEditBarVisibility()
        {
            CurrentPage.IsEditBarVisible = !CurrentPage.IsEditBarVisible;

            db.Entry(CurrentPage).State = EntityState.Modified;
            db.SaveChanges();
        }

        /// <summary>
        /// Increases image scale by <see cref="ScaleIn"/> (1.1)
        /// </summary>
        private void ZoomIn()
        {
            CurrentImage.ScaleX *= ScaleIn;
            CurrentImage.ScaleY *= ScaleIn;

            db.Entry(CurrentImage).State = EntityState.Modified;
            db.SaveChangesAsync();
        }

        /// <summary>
        /// Increases image scale by <see cref="ScaleOut"/> (0.9)
        /// </summary>
        private void ZoomOut()
        {
            CurrentImage.ScaleX *= ScaleOut;
            CurrentImage.ScaleY *= ScaleOut;

            db.Entry(CurrentImage).State = EntityState.Modified;
            db.SaveChangesAsync();
        }

        /// <summary>
        /// Decreases image rotation angle by <see cref="RotationAngle"/> (90)
        /// </summary>
        private void RotateLeft()
        {
            CurrentImage.Angle -= RotationAngle;

            db.Entry(CurrentImage).State = EntityState.Modified;
            db.SaveChangesAsync();
        }

        /// <summary>
        /// Increases image rotation angle by <see cref="RotationAngle"/> (90)
        /// </summary>
        private void RotateRight()
        {
            CurrentImage.Angle += RotationAngle;

            db.Entry(CurrentImage).State = EntityState.Modified;
            db.SaveChangesAsync();
        }

        #endregion

        /// <summary>
        /// Changes current image
        /// </summary>
        private void SelectionChanged()
        {
            CurrentPage.ImageModelID = CurrentImage != null ? CurrentImage.ID : CurrentPage.ImageModelID - 1;

            _ea.GetEvent<IdSentEvent>().Publish(CurrentPage.ImageModelID);

            db.Entry(CurrentPage).State = EntityState.Modified;
            db.SaveChangesAsync();
        }
    }
}
