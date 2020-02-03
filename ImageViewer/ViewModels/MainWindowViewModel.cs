using ImageViewer.Abstractions;
using ImageViewer.DatabaseContext;
using ImageViewer.Models;
using Prism.Commands;
using Prism.Mvvm;
using System.Linq;
using System.Windows;

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

        #endregion

        #region Commands

        /// <summary>
        /// Command to close window and save changes
        /// </summary>
        public DelegateCommand<Window> CloseCommand { get; }

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        public MainWindowViewModel()
        {
            CloseCommand = new DelegateCommand<Window>(CloseWindow);

            _db = new ApplicationContext();

            var window = _db.GetWindowModels().FirstOrDefault();

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
        /// <param name="window"></param>
        private void CloseWindow(Window window)
        {
            _db.UpdateWindowModel(CurrentWindow);

            window.Close();
        }

        #endregion
    }
}
