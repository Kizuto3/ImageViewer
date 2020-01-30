using Prism.Mvvm;

namespace ImageViewer.Models
{
    public class WindowModel : BindableBase
    {
        #region Private Members

        /// <summary>
        /// The position of the window`s left edge in relation to desktop
        /// </summary>
        private double _left;

        /// <summary>
        /// The position of the window`s top edge in relation to desktop
        /// </summary>
        private double _top;

        /// <summary>
        /// The window`s width
        /// </summary>
        private double _width;

        /// <summary>
        /// The window`s height
        /// </summary>
        private double _height;

        /// <summary>
        /// The window`s state (minimized, maximized or normal)
        /// </summary>
        private int _state;

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the position of the window`s left edge in relation to desktop
        /// </summary>
        public double Left
        {
            get
            {
                return _left;
            }
            set
            {
                SetProperty(ref _left, value);
            }
        }

        /// <summary>
        /// Gets or sets the position of the window`s top edge in relation to desktop
        /// </summary>
        public double Top
        {
            get
            {
                return _top;
            }
            set
            {
                SetProperty(ref _top, value);
            }
        }

        /// <summary>
        /// Gets or sets the window`s width
        /// </summary>
        public double Width
        {
            get
            {
                return _width;
            }
            set
            {
                SetProperty(ref _width, value);
            }
        }

        /// <summary>
        /// Gets or sets the window`s height
        /// </summary>
        public double Height
        {
            get
            {
                return _height;
            }
            set
            {
                SetProperty(ref _height, value);
            }
        }

        /// <summary>
        /// Gets or sets the window`s state (minimized, maximized or normal)
        /// </summary>
        public int State
        {
            get
            {
                return _state;
            }
            set
            {
                SetProperty(ref _state, value);
            }
        }

        /// <summary>
        /// Gets or sets the window`s id
        /// </summary>
        public int ID { get; set; }

        #endregion

        /// <summary>
        /// Default constructor
        /// </summary>
        public WindowModel() {}

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="left">The position of the window`s left edge in relation to desktop</param>
        /// <param name="top">The position of the window`s top edge in relation to desktop</param>
        /// <param name="width">The window`s width</param>
        /// <param name="height">The window`s height</param>
        /// <param name="state">The window`s state (normal, minimized, maximized)</param>
        /// <param name="isListVisible">Flag that defines if image list is visible to user or not</param>
        /// <param name="isEditBarVisible">Flag that defines if edit bar is visible to user or not</param>
        public WindowModel(double left, double top, double width, double height, int state)
        {
            _left = left;
            _top = top;
            _width = width;
            _height = height;
            _state = state;
        }
    }
}
