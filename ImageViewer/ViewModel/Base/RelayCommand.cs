using System;
using System.Windows.Input;

namespace ImageViewer
{
    /// <summary>
    /// A basic command that runs Action
    /// </summary>
    public class RelayCommand : ICommand
    {
        #region Private members

        /// <summary>
        /// The action to run
        /// </summary>
        private Action _action;

        #endregion

        #region Public Events
        /// <summary>
        /// The event thats fires when the <see cref="CanExecute(object)"/> value has changed 
        /// </summary>
        public event EventHandler CanExecuteChanged = (sender, e) => { };

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>

        public RelayCommand(Action action)
        {
            _action = action;
        }
        #endregion

        #region Command Methods

        /// <summary>
        /// A relay command can can always execute
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>

        public bool CanExecute(object parameter) => true;

        /// <summary>
        /// Executes a commands Action
        /// </summary>
        /// <param name="parameter"></param>
        public void Execute(object parameter)
        {
            _action();
        }

        #endregion
    }
}
