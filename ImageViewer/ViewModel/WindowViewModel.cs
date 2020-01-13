using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ImageViewer
{
    class WindowViewModel : BaseViewModel
    {
        #region Private Members

        /// <summary>
        /// The window this View Model Controls
        /// </summary>
        private Window mWindow;

        /// <summary>
        /// The margin around the window to allow a drop shadow
        /// </summary>
        private int mOutermarginSize = 10;
        private int mWindowRadius = 10;

        #endregion

        #region Public Properties

        /// <summary>
        /// The size of the resize border around the window
        /// </summary>
        public int ResizeBorder { get; set; } = 6;

        public Thickness ResizeBorderThickness { get { return new Thickness(ResizeBorder); } }

        #endregion

        #region Constructor
        public WindowViewModel(Window window)
        {
            mWindow = window;
        }

        #endregion
    }
}
