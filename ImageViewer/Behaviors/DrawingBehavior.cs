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
using ImageViewer.Abstractions;

namespace ImageViewer.Behaviors
{
    class DrawingBehavior : Behavior<Image>
    {
        #region Properties

        /// <summary>
        /// Dependency property to set up a geometry`s border color
        /// </summary>
        public DependencyProperty BorderColorBrushProperty = DependencyProperty.Register(nameof(BorderColorBrush), typeof(SolidColorBrush), typeof(DrawingBehavior), new PropertyMetadata(Brushes.Fuchsia));

        /// <summary>
        /// Dependency property to set up a thickness of geometry`s border
        /// </summary>
        public DependencyProperty BorderThicknessProperty = DependencyProperty.Register(nameof(BorderThickness), typeof(double), typeof(DrawingBehavior), new PropertyMetadata(1d));

        /// <summary>
        /// Dependency property to set up an opacity of a geometry
        /// </summary>
        public DependencyProperty BackgroundOpacityProperty = DependencyProperty.Register(nameof(BackgroundOpacity), typeof(double), typeof(DrawingBehavior), new PropertyMetadata(1d));

        /// <summary>
        /// Dependency property to set up a background color of a geometry
        /// </summary>
        public DependencyProperty BackgroundColorProperty = DependencyProperty.Register(nameof(BackgroundColor), typeof(Color), typeof(DrawingBehavior), new PropertyMetadata(Colors.Transparent));

        /// <summary>
        /// A geometry`s border color
        /// </summary>
        public SolidColorBrush BorderColorBrush
        {
            get => (SolidColorBrush)GetValue(BorderColorBrushProperty);
            set => SetValue(BorderColorBrushProperty, value);
        }

        /// <summary>
        /// A thickness of geometry`s border
        /// </summary>
        public double BorderThickness
        {
            get => (double)GetValue(BorderThicknessProperty);
            set => SetValue(BorderThicknessProperty, value);
        }

        /// <summary>
        /// An opacity of a geometry
        /// </summary>
        public double BackgroundOpacity
        {
            get => (double)GetValue(BackgroundOpacityProperty);
            set => SetValue(BackgroundOpacityProperty, value);
        }

        /// <summary>
        /// A background color of a geometry
        /// </summary>
        public Color BackgroundColor
        {
            get => (Color)GetValue(BackgroundColorProperty);
            set => SetValue(BackgroundColorProperty, value);
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
        /// Indicates what geometry user wants to draw
        /// </summary>
        private DrawingGeometry _drawingGeometry = DrawingGeometry.None; 

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
                BorderBrush = BorderColorBrush,
                BorderThickness = BorderThickness,
                BackgroundOpacity = BackgroundOpacity,
                BackgroundColor = BackgroundColor
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

                switch (_drawingGeometry)
                {
                    case DrawingGeometry.Rectangle: DrawRectangle(_adorners.Last());
                        break;
                    case DrawingGeometry.Circle: DrawCircle(_adorners.Last());
                        break;
                    case DrawingGeometry.Line: DrawLine(_adorners.Last());
                        break;
                }
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
            _drawingGeometry = _drawingGeometry == DrawingGeometry.Rectangle ? DrawingGeometry.None : DrawingGeometry.Rectangle;
        }

        /// <summary>
        /// Set flag to draw a circle or nothing
        /// </summary>
        private void OnDrawCircle()
        {
            _drawingGeometry = _drawingGeometry == DrawingGeometry.Circle ? DrawingGeometry.None : DrawingGeometry.Circle;
        }

        /// <summary>
        /// Set flag to draw a line or nothing
        /// </summary>
        private void OnDrawLine()
        {
            _drawingGeometry = _drawingGeometry == DrawingGeometry.Line ? DrawingGeometry.None : DrawingGeometry.Line;
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
                    BorderBrush = BorderColorBrush,
                    BorderThickness = BorderThickness,
                    BackgroundOpacity = BackgroundOpacity,
                    BackgroundColor = BackgroundColor
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
