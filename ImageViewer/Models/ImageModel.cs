using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.IO;

namespace ImageViewer.Models
{
    public class ImageModel : BindableBase
    {
        /// <summary>
        /// Full path to image 
        /// </summary>
        private string _fullPath;

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
        /// File name of the image
        /// </summary>
        public string FileName
        {
            get
            {
                return Path.GetFileName(_fullPath);
            }
        }

        /// <summary>
        /// Default Constructor
        /// </summary>
        public ImageModel()
        {
            _fullPath = "";
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="path">Full path to image</param>
        public ImageModel(string path)
        {
            _fullPath = path;
        }

        /// <summary>
        /// Compare Image models by full paths of two images
        /// </summary>
        /// <param name="obj">Object to compare</param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            try
            {
                // Try to cast object to Image model
                var image1 = (ImageModel)obj;

                // And compare their paths
                return FullPath.Equals(image1.FullPath);
            }
            catch(Exception)
            {
                //Otherwise compare by references
                return base.Equals(obj);
            }
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
