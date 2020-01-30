using Prism.Commands;
using System.IO;
using System.Windows;
using System;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using ImageViewer.Adorners;
using System.Windows.Documents;
using Microsoft.Win32;

namespace ImageViewer.Behaviors
{
    class CropCopySaveBehavior : Behavior<Image>
    {
        #region Private Members

        /// <summary>
        /// Start point of the rectagle to crop
        /// </summary>
        private Point _startPoint;

        /// <summary>
        /// End point of the rectagle to crop
        /// </summary>
        private Point _endPoint;

        /// <summary>
        /// Flag that indicates if user can select area to crop
        /// </summary>
        private bool _canSelectArea;

        /// <summary>
        /// Adorner that will be added to image adorner layer
        /// </summary>
        private GeometryAdorner _adorner;

        /// <summary>
        /// Image adorner layer
        /// </summary>
        private AdornerLayer _layer;

        #endregion

        #region Commands

        /// <summary>
        /// Command to save image
        /// </summary>
        public ICommand SaveCommand => new DelegateCommand(OnSave);

        /// <summary>
        /// Command to crop image
        /// </summary>
        public ICommand CropCommand => new DelegateCommand(OnCrop);

        /// <summary>
        /// Command to remove crop from image
        /// </summary>
        public ICommand RemoveCropCommand => new DelegateCommand(OnRemoveCrop);

        /// <summary>
        /// Command to copy image to clipboard
        /// </summary>
        public ICommand CopyCommand => new DelegateCommand(OnCopy);

        /// <summary>
        /// Command to select area to crop
        /// </summary>
        public ICommand SelectAreaCommand => new DelegateCommand(OnSelectArea);

        #endregion

        #region Methods

        protected override void OnAttached()
        {
            base.OnAttached();

            AssociatedObject.MouseDown += OnMouseDown;
            AssociatedObject.MouseMove += OnMouseMove;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();

            AssociatedObject.MouseLeftButtonDown -= OnMouseDown;
            AssociatedObject.MouseMove -= OnMouseMove;
        }

        /// <summary>
        /// Draw rectangle above image to crop
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && _canSelectArea)
            {
                _endPoint = e.GetPosition(AssociatedObject);

                //Create a new instance of Rect object
                var rect = new Rect()
                {
                    X = Math.Min(_startPoint.X, _endPoint.X),
                    Y = Math.Min(_startPoint.Y, _endPoint.Y),
                    Width = Math.Abs(_startPoint.X - _endPoint.X),
                    Height = Math.Abs(_startPoint.Y - _endPoint.Y)
                };

                var rectangleGeometry = new RectangleGeometry(rect);

                //If adorner layer wasn`t created
                if (_layer == null)

                    //Get the image`s adorner layer
                    _layer = AdornerLayer.GetAdornerLayer(AssociatedObject);

                //If adorner was created
                if (_adorner != null)
                {
                    //Set it`s Rect object to new rectangle
                    _adorner.Rect = rect;
                    _adorner.Geometry = rectangleGeometry;

                    //And invalidate it`s visual
                    _adorner.InvalidateVisual();
                }

                //If adorner was not created
                else
                {
                    //Create a new instance of RectAdorner object
                    _adorner = new GeometryAdorner(AssociatedObject, rectangleGeometry)
                    {
                        Cursor = Cursors.Hand
                    };

                    //Set it`s mouse event handlers
                    _adorner.MouseMove += OnMouseMove;
                    _adorner.MouseDown += OnMouseDown;

                    //And add it to the adorner layer
                    _layer.Add(_adorner);
                }
            }
        }

        /// <summary>
        /// Set start points of the rectangle to crop
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            _startPoint = e.GetPosition(AssociatedObject);
        }

        /// <summary>
        /// Crop the image 
        /// </summary>
        private void OnCrop()
        {
            if (_adorner != null)
            {
                //Clip an image using RectangleGeometry. 
                AssociatedObject.Clip = new RectangleGeometry(_adorner.Rect);

                //Remove the adorner on image
                _layer.Remove(_adorner);

                _adorner = null;
            }
        }

        /// <summary>
        /// Copy image to clipboard
        /// </summary>
        private void OnCopy()
        {
            var bmpCopied = ImageToRTB(AssociatedObject);
            if (bmpCopied != null)
            {
                Clipboard.SetImage(bmpCopied);
                MessageBox.Show("Sended to clipboard");
            }
        }

        /// <summary>
        /// Open save dialog to save image
        /// </summary>
        private void OnSave()
        {
            var bmpCopied = ImageToRTB(AssociatedObject);

            if (bmpCopied != null)
            {
                var saveFileDialog = new SaveFileDialog()
                {
                    Filter = "Image File (*.jpg; *.jpeg; *.gif; *.bmp; *png)| *.jpg; *.jpeg; *.gif; *.bmp; *png"
                };
                if (saveFileDialog.ShowDialog() == true)
                {
                    var encoder = new PngBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(bmpCopied));
                    using (FileStream stream = new FileStream(saveFileDialog.FileName, FileMode.Create))
                        encoder.Save(stream);
                }
            }
        }

        /// <summary>
        /// Removes crop from image
        /// </summary>
        private void OnRemoveCrop()
        {
            AssociatedObject.Clip = null;
        }

        /// <summary>
        /// Set flag to indicates if user can select area to crop
        /// </summary>
        private void OnSelectArea()
        {
            _canSelectArea = !_canSelectArea;
            AssociatedObject.Cursor = AssociatedObject.Cursor == Cursors.Hand ? Cursors.Arrow : Cursors.Hand;
            if (_adorner != null)
            {
                _layer.Remove(_adorner);
                _adorner = null;
            }
        }

        /// <summary>
        /// Converts image to RenderTargetBitmap
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        private RenderTargetBitmap ImageToRTB(Image image)
        {
            double width = image.ActualWidth;
            double height = image.ActualHeight;
            RenderTargetBitmap bmpCopied = null;

            if (width > 0 && height > 0)
            {
                bmpCopied = new RenderTargetBitmap((int)Math.Round(width), (int)Math.Round(height), 96, 96, PixelFormats.Default);
                DrawingVisual dv = new DrawingVisual();
                using (DrawingContext dc = dv.RenderOpen())
                {
                    VisualBrush vb = new VisualBrush(image);
                    dc.DrawRectangle(vb, null, new Rect(new Point(), new Size(width, height)));
                }
                bmpCopied.Render(dv);
            }
            return bmpCopied;
        }

        #endregion
    }
}
