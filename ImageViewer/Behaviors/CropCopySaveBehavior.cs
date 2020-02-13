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
using ImageViewer.Abstractions;
using CommonServiceLocator;
using Prism.Events;
using Unity;
using ImageViewer.EventAggregators;

namespace ImageViewer.Behaviors
{
    class CropCopySaveBehavior : Behavior<Image>
    {
        #region Properties

        /// <summary>
        /// Dependency property to set up a shape to be drawn
        /// </summary>
        public static DependencyProperty BehaviorTypeProperty = DependencyProperty.Register(nameof(BehaviorType), typeof(BehaviorType), typeof(CropCopySaveBehavior), new PropertyMetadata(BehaviorType.None));

        /// <summary>
        /// Behavior to use
        /// </summary>
        public BehaviorType BehaviorType
        {
            get => (BehaviorType)GetValue(BehaviorTypeProperty);
            set => SetValue(BehaviorTypeProperty, value);
        }

        #endregion

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
        /// Adorner that will be added to image adorner layer
        /// </summary>
        private GeometryAdorner _adorner;

        /// <summary>
        /// Image adorner layer
        /// </summary>
        private AdornerLayer _layer;

        #endregion

        /// <summary>
        /// Creates new instance of <see cref="CropCopySaveBehavior"/> and subscribes to events
        /// </summary>
        public CropCopySaveBehavior()
        {
            var container = ServiceLocator.Current.GetInstance<IUnityContainer>();
            var ea = container.Resolve<IEventAggregator>();

            ea.GetEvent<SaveImageEvent>().Subscribe(OnSave);
            ea.GetEvent<CropImageEvent>().Subscribe(OnCrop);
            ea.GetEvent<RemoveCropEvent>().Subscribe(OnRemoveCrop);
            ea.GetEvent<CopyImageEvent>().Subscribe(OnCopy);
        }

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
            if (BehaviorType != BehaviorType.CropCopySave) return;

            if (e.LeftButton == MouseButtonState.Pressed)
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

                //Set adorner`s Rect object to new rectangle
                _adorner.Rect = rect;
                _adorner.Geometry = rectangleGeometry;

                //And invalidate it`s visual
                _adorner.InvalidateVisual();
            }
        }

        /// <summary>
        /// Set start points of the rectangle to crop
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (BehaviorType != BehaviorType.CropCopySave) return;

            _startPoint = e.GetPosition(AssociatedObject);

            //If adorner layer wasn`t created
            if (_layer == null)
            {
                //Get the image`s adorner layer
                _layer = AdornerLayer.GetAdornerLayer(AssociatedObject);
            }

            //If adorner was not created
            if (_adorner == null)
            {
                //Create a new instance of RectAdorner object
                _adorner = new GeometryAdorner(AssociatedObject, null)
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
                MessageBox.Show("Sended to clipboard", "Image viewer");
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
