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
using System.Linq;

namespace ImageViewer.Behaviors
{
    class CropCopySaveBehavior : Behavior<Image>
    {
        #region Properties

        /// <summary>
        /// Dependency property to set up a behavior to use
        /// </summary>
        public static DependencyProperty BehaviorTypeProperty = DependencyProperty.Register(nameof(BehaviorType), typeof(BehaviorType), typeof(CropCopySaveBehavior), 
            new PropertyMetadata(BehaviorType.None, new PropertyChangedCallback(BehaviorTypeValueUpdate)));

        /// <summary>
        /// Dependency property to set up a command to invoke
        /// </summary>
        public static DependencyProperty CommandTypeProperty = DependencyProperty.Register(nameof(CommandType), typeof(CopyCropSaveBehaviorCommandType), typeof(CropCopySaveBehavior), 
            new PropertyMetadata(CopyCropSaveBehaviorCommandType.None, new PropertyChangedCallback(CommandTypeValueUpdate)));

        /// <summary>
        /// Behavior to use
        /// </summary>
        public BehaviorType BehaviorType
        {
            get => (BehaviorType)GetValue(BehaviorTypeProperty);
            set => SetValue(BehaviorTypeProperty, value);
        }

        /// <summary>
        /// Command to invoke
        /// </summary>
        public CopyCropSaveBehaviorCommandType CommandType
        {
            get => (CopyCropSaveBehaviorCommandType)GetValue(CommandTypeProperty);
            set => SetValue(CommandTypeProperty, value);
        }

        #endregion

        #region Property changed callbacks

        /// <summary>
        /// Removes rectangle area from image adorner layer if <see cref="BehaviorType"/> does not equal to <see cref="BehaviorType.CropCopySave"/>
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        private static void BehaviorTypeValueUpdate(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var behavior = d as CropCopySaveBehavior;

            if (behavior.BehaviorType != BehaviorType.CropCopySave)
            {
                if (behavior._layer != null && behavior._adorner != null)
                {
                    behavior._layer.Remove(behavior._adorner);
                    behavior._adorner = null;
                }
            }
        }

        /// <summary>
        /// Calls method requested by <see cref="CopyCropSaveBehaviorCommandType"/>
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        private static void CommandTypeValueUpdate(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var behavior = d as CropCopySaveBehavior;

            switch (behavior.CommandType)
            {
                case CopyCropSaveBehaviorCommandType.Save:
                    behavior.OnSave();
                    break;
                case CopyCropSaveBehaviorCommandType.Copy:
                    behavior.OnCopy();
                    break;
                case CopyCropSaveBehaviorCommandType.Crop:
                    behavior.OnCrop();
                    break;
                case CopyCropSaveBehaviorCommandType.RemoveCrop:
                    behavior.OnRemoveCrop();
                    break;
            }
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
            if (BehaviorType != BehaviorType.CropCopySave || _adorner == null) return;

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

                //Set adorner`s Rect object to new rectangle
                _adorner.Rect = rect;
                _adorner.Geometry = new RectangleGeometry(rect);

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
            }
            if (!_layer.GetAdorners(AssociatedObject).Contains(_adorner))
            {
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
                AssociatedObject.Clip = _adorner.Geometry as RectangleGeometry;

                //Remove the adorner on image
                _layer.Remove(_adorner);
            }
        }

        /// <summary>
        /// Copy image to clipboard
        /// </summary>
        private void OnCopy()
        {
            var bmpCopied = ImageToCroppedBitmap(AssociatedObject);
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
            var bmpCopied = ImageToCroppedBitmap(AssociatedObject);

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
        private CroppedBitmap ImageToCroppedBitmap(Image image)
        {
            var layer = AdornerLayer.GetAdornerLayer(image);

            layer.UpdateLayout();

            var container = VisualTreeHelper.GetParent(layer) as Visual;

            Rect rect = _adorner == null ? new Rect(image.RenderSize) : _adorner.Rect;

            var relativeElementBounds = image.TransformToAncestor(container).TransformBounds(rect);

            int width = (int)relativeElementBounds.Width;
            int height = (int)relativeElementBounds.Height;
            int x = (int)relativeElementBounds.X;
            int y = (int)relativeElementBounds.Y;

            var bmpCopied = new RenderTargetBitmap((int)image.RenderSize.Width, (int)image.RenderSize.Height, 96, 96, PixelFormats.Default);

            bmpCopied.Render(container);

            var croppedBitmap = new CroppedBitmap(bmpCopied, new Int32Rect(x, y, width, height));

            return croppedBitmap;
        }

        #endregion
    }
}
