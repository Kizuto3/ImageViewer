namespace ImageViewer.Models
{
    class EditModel
    {
        #region Public Properties

        /// <summary>
        /// ID of image in database
        /// </summary>
        public int ImageModelID { get; set; }

        /// <summary>
        /// Geometry path
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// ID of <see cref="EditModel"/>
        /// </summary>
        public int ID { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public EditModel() { }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="imageModelID">ID of image in database</param>
        /// <param name="path">Geometry path</param>
        public EditModel(int imageModelID, string path)
        {
            ImageModelID = imageModelID;
            Path = path;
        }

        #endregion
    }
}
