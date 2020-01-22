using ImageViewer.Abstractions;
using Prism.Mvvm;

namespace ImageViewer.ViewModels
{
    /// <summary>
    /// The View Model for the custom window
    /// </summary>
    public class MainWindowViewModel : BindableBase
    {
        /// <summary>
        /// Page to display on main window
        /// </summary>
        public ApplicationPage CurrentPage { get; set; } = ApplicationPage.MainPage;
    }
}
