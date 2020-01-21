using ImageViewer.Adorners;
using ImageViewer.ViewModels;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
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
        /// Defines if mouse is clicked and moving at the same time
        /// </summary>
        private bool _isMouseMove;

        /// <summary>
        /// Defines the ability to select an area of an image to crop
        /// </summary>
        private bool _canSelectArea;

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

        /// <summary>
        /// Allows to copy an image to user`s clipboard
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CopyButton_Click(object sender, RoutedEventArgs e)
        {
            double width = CurrentImage.ActualWidth;
            double height = CurrentImage.ActualHeight;
            if(width > 0 && height > 0)
            {
                RenderTargetBitmap bmpCopied = new RenderTargetBitmap((int)Math.Round(width), (int)Math.Round(height), 96, 96, PixelFormats.Default);
                DrawingVisual dv = new DrawingVisual();
                using (DrawingContext dc = dv.RenderOpen())
                {
                    VisualBrush vb = new VisualBrush(CurrentImage);
                    dc.DrawRectangle(vb, null, new Rect(new Point(), new Size(width, height)));
                }
                bmpCopied.Render(dv);
                Clipboard.SetImage(bmpCopied);
                MessageBox.Show("Sended to clipboard " + width + " " + height);
            }           
        }

        #region Crop Methods

        /// <summary>
        /// Allows to crop an image
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CropButton_Click(object sender, RoutedEventArgs e)
        {   if(_adorner != null)
            {
                //Clip an image using RectangleGeometry. 
                CurrentImage.Clip = new RectangleGeometry(_adorner.Rect);

                //Remove the adorner on image
                _layer.Remove(_adorner);

                //And set adorner to null
                _adorner = null;
            }
        }

        /// <summary>
        /// Restores an image after croping
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CropButton_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            //Set Clip value to null to restore an actual image
            CurrentImage.Clip = null;
        }

        #endregion

        #region Methods to select region to crop

        /// <summary>
        /// Start drawing an adorner`s rectangle
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ImageView_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //Mouse is clicked
            _isMouseMove = true;

            //Set the start point of a adorner`s rectangle 
            _startPoint = e.GetPosition(CurrentImage);
        }

        /// <summary>
        /// Finish drawing and adorner`s rectangle 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ImageView_MouseUp(object sender, MouseButtonEventArgs e)
        {
            //Mouse is no longer clicked
            _isMouseMove = false;
        }
        
        /// <summary>
        /// Draw an adorner`s rectangle on image
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ImageView_MouseMove(object sender, MouseEventArgs e)
        {
            //If mouse is clicked and moving at the same time
            if (_isMouseMove && _canSelectArea)
            {
                //Set the end point of adorner`s rectangle 
                _endPoint = e.GetPosition(CurrentImage);

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
                    _layer = AdornerLayer.GetAdornerLayer(CurrentImage);

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
                    _adorner = new RectAdorner(CurrentImage, rect);

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

        /// <summary>
        /// if mouse leaves the scrollViewer area disable ability to select area to crop
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ScrollViewer_MouseLeave(object sender, MouseEventArgs e)
        {
            _isMouseMove = false;
        }

        /// <summary>
        /// Adds or removes ability to select an area of an image to crop and changes the cursor
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SelectAreaButton_Click(object sender, RoutedEventArgs e)
        {
            _canSelectArea = !_canSelectArea;
            CurrentImage.Cursor = CurrentImage.Cursor == Cursors.Hand ? Cursors.Arrow : Cursors.Hand;
            if(_adorner != null)
            {
                _layer.Remove(_adorner);
                _adorner = null;
            }
        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_adorner != null && _layer != null)
            {
                _layer.Remove(_adorner);
                _adorner = null;
            }
            CurrentImage.Clip = null;
        }
    }
}