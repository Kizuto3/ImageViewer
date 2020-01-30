using Prism.Mvvm;
using System.Collections.Generic;
using System.IO;

namespace ImageViewer.Models
{
    public class ImageModel : BindableBase
    {
        #region Private Members

        /// <summary>
        /// Full path to an image 
        /// </summary>
        private string _fullpath;

        /// <summary>
        /// Scale of an image
        /// </summary>
        private double _scaleX;

        /// <summary>
        /// Scale of an image
        /// </summary>
        private double _scaleY;

        /// <summary>
        /// Rotation angle of an image
        /// </summary>
        private double _angle;

        #endregion

        #region Public Properties

        public int ID { get; set; }

        /// <summary>
        /// Full path to image
        /// </summary>
        public string Fullpath
        {
            get
            {
                return _fullpath;
            }
            set
            {
                SetProperty(ref _fullpath, value);
            }
        }

        /// <summary>
        /// Scale of image
        /// </summary>
        public double ScaleX
        {
            get
            {
                return _scaleX;
            }
            set
            {
                SetProperty(ref _scaleX, value);
            }
        }

        /// <summary>
        /// Scale of image
        /// </summary>
        public double ScaleY
        {
            get
            {
                return _scaleY;
            }
            set
            {
                SetProperty(ref _scaleY, value);
            }
        }

        /// <summary>
        /// Rotation angle of image
        /// </summary>
        public double Angle
        {
            get
            {
                return _angle;
            }
            set
            {
                SetProperty(ref _angle, value);
            }
        }

        /// <summary>
        /// File name of the image
        /// </summary>
        public string FileName
        {
            get
            {
                return Path.GetFileName(_fullpath);
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Default Constructor
        /// </summary>
        public ImageModel()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="path">Full path to image</param>
        public ImageModel(string path)
        {
            _fullpath = path;
            _scaleX = 1;
            _scaleY = 1;
            _angle = 0;
        }

        #endregion

        #region Overrided Methods

        /// <summary>
        /// Compare Image models by full paths of two images
        /// </summary>
        /// <param name="obj">Object to compare</param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if(obj is ImageModel)
            {
                // cast object to Image model
                var image1 = obj as ImageModel;

                // And compare their paths
                return Fullpath.Equals(image1.Fullpath);
            }
            return base.Equals(obj);
        }

        /// <summary>
        /// Generate HashCode
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            var hashCode = 1295127587;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(_fullpath);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Fullpath);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(FileName);
            return hashCode;
        }

        #endregion
    }
}
