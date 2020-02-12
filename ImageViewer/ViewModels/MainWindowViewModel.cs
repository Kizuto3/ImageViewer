using ImageViewer.Abstractions;
using ImageViewer.DatabaseContext;
using ImageViewer.Models;
using Prism.Mvvm;

namespace ImageViewer.ViewModels
{
    /// <summary>
    /// The View Model for the custom window
    /// </summary>
    public class MainWindowViewModel : BindableBase
    {
        #region Private Members

        /// <summary>
        /// The current window
        /// </summary>
        private WindowModel _currentWindow;

        /// <summary>
        /// Data context to manage database
        /// </summary>
        private readonly ApplicationContext _db;

        /// <summary>
        /// Corner`s radius of the window
        /// </summary>
        private readonly int _windowCornerRadius = 10;

        /// <summary>
        /// Padding of the window
        /// </summary>
        private readonly double _windowPadding = 5d;

        #endregion

        #region Public Properties

        /// <summary>
        /// Page to display on main window
        /// </summary>
        public ApplicationPage CurrentPage { get; set; } = ApplicationPage.MainPage;

        /// <summary>
        /// Gets or sets the current window
        /// </summary>
        public WindowModel CurrentWindow
        {
            get
            {
                return _currentWindow;
            }
            set
            {
                SetProperty(ref _currentWindow, value);
            }
        }

        /// <summary>
        /// Corner`s radius of the window
        /// </summary>
        public int WindowCornerRadius
        {
            get
            {
                return CurrentWindow.State == 0 ? _windowCornerRadius : 0;
            }
        }

        /// <summary>
        /// Padding of the window
        /// </summary>
        public double WindowPadding
        {
            get
            {
                return CurrentWindow.State == 0 ? _windowPadding : 0d;
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        public MainWindowViewModel()
        {
            _db = new ApplicationContext();

            var window = _db.GetWindowModel();

            if (window == null)
            {
                CurrentWindow = new WindowModel(200, 200, 1200, 900, 0);
                _db.InsertWindowModel(CurrentWindow);
            }
            else
            {
                CurrentWindow = window;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Close window and save changes
        /// </summary>
        public void SaveChanges()
        {
            _db.UpdateWindowModel(CurrentWindow);
            _db.RemoveObsoleteEditModels();
        }

        #endregion
    }
}
