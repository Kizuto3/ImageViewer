namespace ImageViewer.Models
{
    public class EditModel
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
        /// Background color of a model
        /// </summary>
        public string BackgroundColor { get; set; }

        /// <summary>
        /// Border brush of a model
        /// </summary>
        public string BorderBrush { get; set; }

        /// <summary>
        /// Opacity of background of a model
        /// </summary>
        public double BackgroundOpacity { get; set; }


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
        /// <param name="backgroundColor">Background color</param>
        /// <param name="backgroundOpacity">Background opacity</param>
        /// <param name="borderBrush">Border brush</param>
        public EditModel(int imageModelID, string path, string backgroundColor, double backgroundOpacity, string borderBrush)
        {
            ImageModelID = imageModelID;
            Path = path;
            BackgroundColor = backgroundColor;
            BorderBrush = borderBrush;
            BackgroundOpacity = backgroundOpacity;
        }

        #endregion
    }
}
