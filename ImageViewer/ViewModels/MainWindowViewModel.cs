using ImageViewer.Abstractions;
using ImageViewer.DatabaseContext;
using ImageViewer.Models;
using Prism.Commands;
using Prism.Mvvm;
using System.Data.Entity;
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
        private readonly ApplicationContext db;

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

        /// <summary>
        /// Default constructor
        /// </summary>
        public MainWindowViewModel()
        {
            CloseCommand = new DelegateCommand<Window>(CloseWindow);

            db = new ApplicationContext();
            db.WindowModels.Load();

            var window = db.WindowModels.Local.FirstOrDefault(w => w is WindowModel);
            if (window == null)
            {
                CurrentWindow = new WindowModel(200, 200, 1200, 900, 0);
                db.WindowModels.Local.Add(CurrentWindow);
                db.SaveChangesAsync();
            }
            else
            {
                CurrentWindow = window;
            }
        }

        #region Methods

        /// <summary>
        /// Close window and save changes
        /// </summary>
        /// <param name="window"></param>
        private void CloseWindow(Window window)
        {
            db.Entry(CurrentWindow).State = EntityState.Modified;
            db.SaveChanges();

            window.Close();
        }

        #endregion
    }
}
