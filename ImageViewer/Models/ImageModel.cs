using Prism.Mvvm;
using System;
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
        private string _fullPath;

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

        /// <summary>
        /// Actual width of an image
        /// </summary>
        private double _width;

        /// <summary>
        /// Actual height of an image
        /// </summary>
        private double _height;

        #endregion

        #region Public Properties

        /// <summary>
        /// Full path to image
        /// </summary>
        public string FullPath
        {
            get
            {
                return _fullPath;
            }
            set
            {
                SetProperty(ref _fullPath, value);
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
        /// Actual width of an image
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
        /// Actual height of an image
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
        /// File name of the image
        /// </summary>
        public string FileName
        {
            get
            {
                return Path.GetFileName(_fullPath);
            }
        }

        #endregion

        /// <summary>
        /// Default Constructor
        /// </summary>
        public ImageModel()
        {
            _fullPath = "";
            _scaleX = 1;
            _scaleY = 1;
            _angle = 0;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="path">Full path to image</param>
        public ImageModel(string path)
        {
            _fullPath = path;
            _scaleX = 1;
            _scaleY = 1;
            _angle = 0;
        }

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
                return FullPath.Equals(image1.FullPath);
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
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(_fullPath);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(FullPath);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(FileName);
            return hashCode;
        }
    }
}
