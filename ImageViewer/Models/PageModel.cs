using Prism.Mvvm;

namespace ImageViewer.Models
{
    public class PageModel : BindableBase
    {
        #region Private Members

        /// <summary>
        /// Flag that defines if image list is visible to user or not
        /// </summary>
        private bool _isListVisible;

        /// <summary>
        /// Flag that defines if edit bar is visible to user or not
        /// </summary>
        private bool _isEditBarVisible;

        /// <summary>
        /// The id of image to display
        /// </summary>
        private int _imageModelID;

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the flag that defines if image list is visible to user or not
        /// </summary>
        public bool IsListVisible
        {
            get
            {
                return _isListVisible;
            }
            set
            {
                SetProperty(ref _isListVisible, value);
            }
        }

        /// <summary>
        /// Gets or sets Flag that defines if edit bar is visible to user or not
        /// </summary>
        public bool IsEditBarVisible
        {
            get
            {
                return _isEditBarVisible;
            }
            set
            {
                SetProperty(ref _isEditBarVisible, value);
            }
        }

        /// <summary>
        /// Gets or sets the id of image to display
        /// </summary>
        public int ImageModelID
        {
            get
            {
                return _imageModelID;
            }
            set
            {
                SetProperty(ref _imageModelID, value);
            }
        }

        /// <summary>
        /// ID of page
        /// </summary>
        public int ID { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="isListVisible"></param>
        /// <param name="isEditBarVisible"></param>
        public PageModel()
        {

        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="isListVisible"></param>
        /// <param name="isEditBarVisible"></param>
        public PageModel(int id, bool isListVisible, bool isEditBarVisible, int imageModelID)
        {
            ID = id;
            _isListVisible = isListVisible;
            _isEditBarVisible = isEditBarVisible;
            _imageModelID = imageModelID;
        }

        #endregion
    }
}
