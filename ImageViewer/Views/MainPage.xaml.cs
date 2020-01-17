using ImageViewer.Adorners;
using ImageViewer.ViewModels;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ImageViewer.Views
{
    /// <summary>
    /// Interaction logic for MainPage.xaml
    /// </summary>
    public partial class MainPage : Page
    {
        #region Private Members

        /// <summary>
        /// Scale coefficient to multiply by when zoom in image
        /// </summary>
        private const double ZoomInCoef = 1.1;

        /// <summary>
        /// Scale coefficient to divide by when zoom in image
        /// </summary>
        private const double ZoomOutCoef = 0.9;

        /// <summary>
        /// Rotation Angle
        /// </summary>
        private const int RotationAngle = 90;

        /// <summary>
        /// Defines if mouse is clicked and moving at the same time
        /// </summary>
        private bool _isMouseMove;

        /// <summary>
        /// Start point to draw a rectangle on image
        /// </summary>
        private Point _startPoint;

        /// <summary>
        /// End point to draw a rectangle on image
        /// </summary>
        private Point _endPoint;

        /// <summary>
        /// Adorner that will be added to image adorner layer
        /// </summary>
        private RectAdorner _adorner;

        /// <summary>
        /// Image adorner layer
        /// </summary>
        private AdornerLayer _layer;

        #endregion

        public MainPage()
        {
            InitializeComponent();
            DataContext = new MainPageViewModel();
        }

        #region Zoom methods

        /// <summary>
        /// Zoom in an image transform group and scale transform
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ZoomInButton_Click(object sender, RoutedEventArgs e)
        {
            // Create an instance of image transform group object
            var transformGroup = ImageView.LayoutTransform as TransformGroup;

            // Find first ScaleTransform child in transform group
            var scaleTransform = transformGroup.Children.FirstOrDefault(child => child is ScaleTransform) as ScaleTransform;

            // If there is not a ScaleTransform child
            if (scaleTransform == null)
            {
                // Create an instance of ScaleTransform object
                var imageScaleTransform = new ScaleTransform
                {
                    // Zoom in an image
                    ScaleX = 1.0 * ZoomInCoef,
                    ScaleY = 1.0 * ZoomInCoef
                };

                // Add ScaleTransform object to transform group
                transformGroup.Children.Add(imageScaleTransform);
            }

            //If there is a ScaleTransform child
            else
            {
                // Change it`s X and Y values to zoom in an image
                scaleTransform.ScaleX *= ZoomInCoef;
                scaleTransform.ScaleY *= ZoomInCoef;
            }
        }

        /// <summary>
        /// Zoom out an image using transform group and scale transform
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ZoomOutButton_Click(object sender, RoutedEventArgs e)
        {
            // Create an instance of image transform group object
            var transformGroup = ImageView.LayoutTransform as TransformGroup;

            // Find first ScaleTransform child in transform group
            var scaleTransform = transformGroup.Children.FirstOrDefault(child => child is ScaleTransform) as ScaleTransform;

            // If there is not a ScaleTransform child
            if (scaleTransform == null)
            {
                // Create an instance of ScaleTransform object
                var imageScaleTransform = new ScaleTransform
                {
                    // Zoom out an image
                    ScaleX = 1.0 * ZoomOutCoef,
                    ScaleY = 1.0 * ZoomOutCoef
                };

                // Add ScaleTransform object to transform group
                transformGroup.Children.Add(imageScaleTransform);
            }

            //If there is a ScaleTransform child
            else
            {
                // Change it`s X and Y values to zoom out an image
                scaleTransform.ScaleX *= ZoomOutCoef;
                scaleTransform.ScaleY *= ZoomOutCoef;
            }
        }

        #endregion

        #region Rotate methods

        /// <summary>
        /// Rotate left an image using transform group and layout transform
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RotateLeftButton_Click(object sender, RoutedEventArgs e)
        {
            // Create an instance of image transform group object
            var transformGroup = ImageView.LayoutTransform as TransformGroup;

            // Find first RotateTransform child in transform group
            var rotateTransform = transformGroup.Children.FirstOrDefault(child => child is RotateTransform) as RotateTransform;

            // If there is not a RotateTransform child
            if (rotateTransform == null)
            {
                // Create an instance of RotateTransform object
                var imageRotateTransform = new RotateTransform
                {
                    //Rotate an image to the left
                    Angle = -RotationAngle
                };

                // Add RotateTransform object to transform group
                transformGroup.Children.Add(imageRotateTransform);
            }

            // If there is a RotateTransform child
            else
            {
                // Change it`s Angle value to rotate an image to the left
                rotateTransform.Angle -= RotationAngle;
            }
        }

        /// <summary>
        /// Rotate right an image using transform group and layout transform
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RotateRightButton_Click(object sender, RoutedEventArgs e)
        {
            // Create an instance of image transform group object
            var transformGroup = ImageView.LayoutTransform as TransformGroup;

            // Find first RotateTransform child in transform group
            var rotateTransform = transformGroup.Children.FirstOrDefault(child => child is RotateTransform) as RotateTransform;

            // If there is not a RotateTransform child
            if (rotateTransform == null)
            {
                // Create an instance of RotateTransform object
                var imageRotateTransform = new RotateTransform
                {
                    //Rotate an image to the right
                    Angle = RotationAngle
                };

                // Add RotateTransform object to transform group
                transformGroup.Children.Add(imageRotateTransform);
            }

            // If there is a RotateTransform child
            else
            {
                // Change it`s Angle value to rotate an image to the right
                rotateTransform.Angle += RotationAngle;
            }
        }

        #endregion

        /// <summary>
        /// Allows to copy an image to user`s clipboard
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CopyButton_Click(object sender, RoutedEventArgs e)
        {
            double width = ImageView.ActualWidth;
            double height = ImageView.ActualHeight;
            RenderTargetBitmap bmpCopied = new RenderTargetBitmap((int)Math.Round(width), (int)Math.Round(height), 96, 96, PixelFormats.Default);
            DrawingVisual dv = new DrawingVisual();
            using (DrawingContext dc = dv.RenderOpen())
            {
                VisualBrush vb = new VisualBrush(ImageView);
                dc.DrawRectangle(vb, null, new Rect(new Point(), new Size(width, height)));
            }
            bmpCopied.Render(dv);
            Clipboard.SetImage(bmpCopied);
            MessageBox.Show("Sended to clipboard");
        }

        #region Crop Methods

        /// <summary>
        /// Allows to crop an image
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CropButton_Click(object sender, RoutedEventArgs e)
        {   
            //Clip an image using RectangleGeometry. 
            ImageView.Clip = new RectangleGeometry(_adorner.Rect);

            //Remove the adorner on image
            _layer.Remove(_adorner);

            //And set adorner to null
            _adorner = null;
        }

        /// <summary>
        /// Restores an image after croping
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CropButton_MouseRightButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            //Set Clip value to null to restore an actual image
            ImageView.Clip = null;
        }

        #endregion

        #region Methods to select region to crop

        /// <summary>
        /// Start drawing an adorner`s rectangle
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ImageView_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            //Mouse is clicked
            _isMouseMove = true;

            //Set the start point of a adorner`s rectangle 
            _startPoint = e.GetPosition(ImageView);
        }

        /// <summary>
        /// Finish drawing and adorner`s rectangle 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ImageView_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            //Mouse is no longer clicked
            _isMouseMove = false;
        }
        
        /// <summary>
        /// Draw an adorner`s rectangle on image
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ImageView_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            //If mouse is clicked and moving at the same time
            if (_isMouseMove)
            {
                //Set the end point of adorner`s rectangle 
                _endPoint = e.GetPosition(ImageView);

                //Create a new instance of Rect object
                var rect = new Rect()
                {
                    X = Math.Min(_startPoint.X, _endPoint.X),
                    Y = Math.Min(_startPoint.Y, _endPoint.Y),
                    Width = Math.Abs(_startPoint.X - _endPoint.X),
                    Height = Math.Abs(_startPoint.Y - _endPoint.Y)
                };

                //If adorner layer wasn`t created
                if(_layer == null)

                    //Get the image`s adorner layer
                    _layer = AdornerLayer.GetAdornerLayer(ImageView);

                //If adorner was created
                if (_adorner != null)
                {
                    //Set it`s Rect object to new rectangle
                    _adorner.Rect = rect;

                    //And invalidate it`s visual
                    _adorner.InvalidateVisual();
                }

                //If adorner was not created
                else
                {
                    //Create a new instance of RectAdorner object
                    _adorner = new RectAdorner(ImageView, rect);

                    //Set it`s mouse event handlers
                    _adorner.MouseUp += ImageView_MouseUp;
                    _adorner.MouseMove += ImageView_MouseMove;
                    _adorner.MouseDown += ImageView_MouseDown;

                    //And add it to the adorner layer
                    _layer.Add(_adorner);
                }
            }
        }

        #endregion
    }
}
