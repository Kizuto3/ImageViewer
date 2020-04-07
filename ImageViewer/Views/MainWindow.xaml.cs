using ImageViewer.ViewModels;
using System.Windows;
using System.Windows.Input;

namespace ImageViewer.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Close the window and save changes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            var viewModel = DataContext as MainWindowViewModel;
            viewModel.SaveChanges();
            Close();
        }

        /// <summary>
        /// Maximize the window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MaximizeButton_Click(object sender, RoutedEventArgs e)
        {
            WindowState ^= WindowState.Maximized;
        }

        /// <summary>
        /// Minimize the window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        /// <summary>
        /// Show system menu bar 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuButton_MouseUp(object sender, MouseButtonEventArgs e)
        {
            SystemCommands.ShowSystemMenu(this, PointToScreen(e.GetPosition(this)));
        }
    }
}
