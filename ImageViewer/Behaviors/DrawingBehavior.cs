using System.Windows;
using System;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;
using System.Windows.Media;
using ImageViewer.Adorners;
using System.Windows.Documents;
using System.Collections.Generic;
using Prism.Commands;
using Unity;
using Prism.Events;
using CommonServiceLocator;
using ImageViewer.EventAggregators;
using ImageViewer.DatabaseContext;
using System.Linq;
using ImageViewer.Models;

namespace ImageViewer.Behaviors
{
    class DrawingBehavior : Behavior<Image>
    {
        #region Properties

        public DependencyProperty SolidColorBrushProperty = DependencyProperty.Register(nameof(SolidColorBrush), typeof(SolidColorBrush), typeof(DrawingBehavior), new PropertyMetadata(Brushes.Fuchsia));

        public DependencyProperty ThicknessProperty = DependencyProperty.Register(nameof(Thickness), typeof(double), typeof(DrawingBehavior), new PropertyMetadata(1d));

        public DependencyProperty OpacityProperty = DependencyProperty.Register(nameof(Opacity), typeof(double), typeof(DrawingBehavior), new PropertyMetadata(1d));

        public DependencyProperty BackgroundProperty = DependencyProperty.Register(nameof(Background), typeof(Color), typeof(DrawingBehavior), new PropertyMetadata(Colors.Transparent));

        public SolidColorBrush SolidColorBrush
        {
            get => (SolidColorBrush)GetValue(SolidColorBrushProperty);
            set => SetValue(SolidColorBrushProperty, value);
        }

        public double Thickness
        {
            get => (double)GetValue(ThicknessProperty);
            set => SetValue(ThicknessProperty, value);
        }

        public double Opacity
        {
            get => (double)GetValue(OpacityProperty);
            set => SetValue(OpacityProperty, value);
        }

        public Color Background
        {
            get => (Color)GetValue(BackgroundProperty);
            set => SetValue(BackgroundProperty, value);
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
        /// Flag that indicates if user can draw a rectangle
        /// </summary>
        private bool _drawRectangle;

        /// <summary>
        /// Flag that indicate if user can draw a circle
        /// </summary>
        private bool _drawCircle;

        /// <summary>
        /// Flag that indicate if user can draw a line
        /// </summary>
        private bool _drawLine;

        /// <summary>
        /// ID of current image
        /// </summary>
        private int _imageModelID;

        /// <summary>
        /// Adorners that will be added to image adorner layer
        /// </summary>
        private readonly List<GeometryAdorner> _adorners = new List<GeometryAdorner>();

        /// <summary>
        /// Image adorner layer
        /// </summary>
        private AdornerLayer _layer;

        /// <summary>
        /// Data context to manage database
        /// </summary>
        private readonly ApplicationContext _db = new ApplicationContext();

        /// <summary>
        /// Event Aggregator to subscribe to <see cref="IdSentEvent"/> event
        /// </summary>
        private readonly IEventAggregator _ea;

        #endregion

        /// <summary>
        /// Constructor to subscribe to <see cref="IdSentEvent"/> event aggregator
        /// </summary>
        public DrawingBehavior()
        {
            var container = ServiceLocator.Current.GetInstance<IUnityContainer>();
            _ea = container.Resolve<IEventAggregator>();
            _ea.GetEvent<IdSentEvent>().Subscribe(DrawGeometries);

            var page = _db.GetPageModel();
            _imageModelID = _db.GetImageModel(page.ImageModelID).ID;
        }

        #region Commands

        /// <summary>
        /// Command to set flags to draw a rectangle or nothing
        /// </summary>
        public ICommand DrawRectangleCommand => new DelegateCommand(OnDrawRectangle);

        /// <summary>
        /// Command to set flags to draw a circle or nothing
        /// </summary>
        public ICommand DrawCircleCommand => new DelegateCommand(OnDrawCircle);

        /// <summary>
        /// Command to set flags to draw a line or nothing
        /// </summary>
        public ICommand DrawLineCommand => new DelegateCommand(OnDrawLine);

        #endregion

        #region Methods

        protected override void OnAttached()
        {
            base.OnAttached();

            AssociatedObject.MouseLeftButtonDown += OnMouseDown;
            AssociatedObject.MouseMove += OnMouseMove;
            AssociatedObject.MouseUp += OnMouseUp;

            AssociatedObject.Loaded += AssociatedObject_Loaded;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();

            AssociatedObject.MouseLeftButtonDown -= OnMouseDown;
            AssociatedObject.MouseMove -= OnMouseMove;
            AssociatedObject.MouseUp -= OnMouseUp;

            AssociatedObject.Loaded -= AssociatedObject_Loaded;
        }

        /// <summary>
        /// Set start points of the rectangle to crop
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            _startPoint = e.GetPosition(AssociatedObject);

            var adorner = new GeometryAdorner(AssociatedObject, null)
            {
                Cursor = Cursors.Hand,
                BorderBrush = SolidColorBrush,
                BorderThickness = Thickness,
                BackgroundOpacity = Opacity,
                BackgroundColor = Background
            };

            if (_layer == null) _layer = AdornerLayer.GetAdornerLayer(AssociatedObject);

            _layer.Add(adorner);
            _adorners.Add(adorner);

            adorner.MouseMove += OnMouseMove;
            adorner.MouseLeftButtonDown += OnMouseDown;
            adorner.MouseUp += OnMouseUp;
        }

        /// <summary>
        /// Draw rectangle above image to crop
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                _endPoint = e.GetPosition(AssociatedObject);

                if (_drawRectangle)
                    DrawRectangle(_adorners.Last());
                if (_drawCircle)
                    DrawCircle(_adorners.Last());
                if (_drawLine)
                    DrawLine(_adorners.Last());
            }
        }

        /// <summary>
        /// Insert <see cref="EditModel"/> into database
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (_adorners.Last().Geometry == null) return;

            var path = _adorners.Last().Geometry.GetFlattenedPathGeometry().ToString();
            _db.InsertEditModel(new EditModel(_imageModelID, path));
        }

        /// <summary>
        /// Draw all geometries above image after it`s first load
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AssociatedObject_Loaded(object sender, RoutedEventArgs e)
        {
            DrawGeometries(_imageModelID);
        }

        /// <summary>
        /// Draw a rectangle
        /// </summary>
        /// <param name="adorner"></param>
        private void DrawRectangle(GeometryAdorner adorner)
        {
            //Create a new instance of Rect object
            var rect = new Rect()
            {
                X = Math.Min(_startPoint.X, _endPoint.X),
                Y = Math.Min(_startPoint.Y, _endPoint.Y),
                Width = Math.Abs(_startPoint.X - _endPoint.X),
                Height = Math.Abs(_startPoint.Y - _endPoint.Y)
            };

            var rectangleGeometry = new RectangleGeometry(rect);

            //Set it`s Rect object to new a rectangle geometry
            adorner.Geometry = rectangleGeometry;

            //And invalidate it`s visual
            adorner.InvalidateVisual();
        }

        /// <summary>
        /// Draw a circle
        /// </summary>
        /// <param name="adorner"></param>
        private void DrawCircle(GeometryAdorner adorner)
        {
            //Create a new instance of Ellipse geometry
            var ellipseGeometry = new EllipseGeometry
            {
                Center = new Point((_startPoint.X + _endPoint.X) / 2, (_startPoint.Y + _endPoint.Y) / 2),
                RadiusX = Math.Sqrt(Math.Pow(_endPoint.X - _startPoint.X, 2) / 2),
                RadiusY = Math.Sqrt(Math.Pow(_endPoint.Y - _startPoint.Y, 2) / 2)
            };

            //Set adorner`s Geometry property to a new Ellipse geometry
            adorner.Geometry = ellipseGeometry;

            //And invalidate adorner`s visual
            adorner.InvalidateVisual();
        }

        /// <summary>
        /// Draw a line
        /// </summary>
        /// <param name="adorner"></param>
        private void DrawLine(GeometryAdorner adorner)
        {
            //Create a new instance of line geometry
            var lineGeometry = new LineGeometry
            {
                StartPoint = _startPoint,
                EndPoint = _endPoint,
            };

            //Set adorner`s Geometry property to a new Line geometry
            adorner.Geometry = lineGeometry;

            //And invalidate adorner`s visual
            adorner.InvalidateVisual();
        }

        /// <summary>
        /// Set flag to draw a rectangle or nothing
        /// </summary>
        private void OnDrawRectangle()
        {
            _drawCircle = false;
            _drawLine = false;
            _drawRectangle = !_drawRectangle;
        }

        /// <summary>
        /// Set flag to draw a circle or nothing
        /// </summary>
        private void OnDrawCircle()
        {
            _drawRectangle = false;
            _drawLine = false;
            _drawCircle = !_drawCircle;
        }

        /// <summary>
        /// Set flag to draw a line or nothing
        /// </summary>
        private void OnDrawLine()
        {
            _drawRectangle = false;
            _drawCircle = false;
            _drawLine = !_drawLine;
        }

        /// <summary>
        /// Gets all geometries related to image with <paramref name="imageModelID"/> from database and draws them
        /// </summary>
        /// <param name="imageModelID">ID of image</param>
        private void DrawGeometries(int imageModelID)
        {
            if (_layer == null)
            {
                _layer = AdornerLayer.GetAdornerLayer(AssociatedObject);
            }

            foreach (var adorner in _adorners)
            {
                _layer.Remove(adorner);
            }

            _imageModelID = imageModelID;

            var shapes = _db.GetEditModels(imageModelID);

            foreach (var shape in shapes)
            {
                var adorner = new GeometryAdorner(AssociatedObject, Geometry.Parse(shape.Path))
                {
                    Cursor = Cursors.Hand,
                    BorderBrush = SolidColorBrush,
                    BorderThickness = Thickness,
                    BackgroundOpacity = Opacity,
                    BackgroundColor = Background
                };

                adorner.MouseMove += OnMouseMove;
                adorner.MouseLeftButtonDown += OnMouseDown;
                adorner.MouseUp += OnMouseUp;

                _adorners.Add(adorner);
                _layer.Add(adorner);
            }
        }

        #endregion
    }
}
