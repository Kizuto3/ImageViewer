using System.Windows;
using System;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;
using System.Windows.Media;
using ImageViewer.Adorners;
using System.Windows.Documents;
using System.Collections.Generic;
using ImageViewer.DatabaseContext;
using System.Linq;
using ImageViewer.Models;
using ImageViewer.Abstractions;
using System.Collections.ObjectModel;

namespace ImageViewer.Behaviors
{
    class DrawingBehavior : Behavior<Image>
    {
        #region Properties

        /// <summary>
        /// Dependency property to set up a geometry`s border color
        /// </summary>
        public static DependencyProperty BorderColorProperty = DependencyProperty.Register(nameof(BorderColor), typeof(Color), typeof(DrawingBehavior), new PropertyMetadata(Colors.Fuchsia));

        /// <summary>
        /// Dependency property to set up a thickness of geometry`s border
        /// </summary>
        public static DependencyProperty BorderThicknessProperty = DependencyProperty.Register(nameof(BorderThickness), typeof(double), typeof(DrawingBehavior), 
            new PropertyMetadata(1d, new PropertyChangedCallback(BorderThicknessChanged)));

        /// <summary>
        /// Dependency property to set up an opacity of a geometry
        /// </summary>
        public static DependencyProperty BackgroundOpacityProperty = DependencyProperty.Register(nameof(BackgroundOpacity), typeof(double), typeof(DrawingBehavior), new PropertyMetadata(1d));

        /// <summary>
        /// Dependency property to set up a background color of a geometry
        /// </summary>
        public static DependencyProperty BackgroundColorProperty = DependencyProperty.Register(nameof(BackgroundColor), typeof(Color), typeof(DrawingBehavior), new PropertyMetadata(Colors.Transparent));

        /// <summary>
        /// Dependency property to set up a shape to be drawn
        /// </summary>
        public static DependencyProperty ShapeProperty = DependencyProperty.Register(nameof(Shape), typeof(ShapeType), typeof(DrawingBehavior), new PropertyMetadata(ShapeType.None));

        /// <summary>
        /// Dependency property to set up a behavior to use
        /// </summary>
        public static DependencyProperty BehaviorTypeProperty = DependencyProperty.Register(nameof(BehaviorType), typeof(BehaviorType), typeof(DrawingBehavior), new PropertyMetadata(BehaviorType.None));

        /// <summary>
        /// Dependency property to set up all drawn shapes 
        /// </summary>
        public static DependencyProperty EditModelsProperty
            = DependencyProperty.Register(nameof(EditModels), typeof(ObservableCollection<EditModel>), typeof(DrawingBehavior), 
                new PropertyMetadata(new ObservableCollection<EditModel>(), new PropertyChangedCallback(EditModelsUpdate)));

        /// <summary>
        /// Dependency property to set up a ID of current image
        /// </summary>
        public static DependencyProperty CurrentImageIDProperty = DependencyProperty.Register(nameof(CurrentImageID), typeof(int), typeof(DrawingBehavior), new PropertyMetadata(1));

        /// <summary>
        /// A geometry`s border color
        /// </summary>
        public Color BorderColor
        {
            get => (Color)GetValue(BorderColorProperty);
            set => SetValue(BorderColorProperty, value);
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

        /// <summary>
        /// Shape to be drawn
        /// </summary>
        public ShapeType Shape
        {
            get => (ShapeType)GetValue(ShapeProperty);
            set => SetValue(ShapeProperty, value);
        }

        /// <summary>
        /// Behavior to use
        /// </summary>
        public BehaviorType BehaviorType
        {
            get => (BehaviorType)GetValue(BehaviorTypeProperty);
            set => SetValue(BehaviorTypeProperty, value);
        }

        /// <summary>
        /// Shapes drawn above image
        /// </summary>
        public ObservableCollection<EditModel> EditModels
        {
            get => (ObservableCollection<EditModel>)GetValue(EditModelsProperty);
            set => SetValue(EditModelsProperty, value);
        }

        /// <summary>
        /// ID of current image
        /// </summary>
        public int CurrentImageID
        {
            get => (int)GetValue(CurrentImageIDProperty);
            set => SetValue(CurrentImageIDProperty, value);
        }

        #endregion

        #region Property changed callbacks

        /// <summary>
        /// Call <see cref="DrawGeometries(ObservableCollection{EditModel})"/> when <see cref="EditModels"/> changes ref
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        private static void EditModelsUpdate(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var behavior = d as DrawingBehavior;

            behavior.DrawGeometries(behavior.EditModels);
        }

        /// <summary>
        /// Call <see cref="ChangeBorderThickness"/> when <see cref="BorderThickness"/> changes it`s value
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        private static void BorderThicknessChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var behavior = d as DrawingBehavior;

            behavior.ChangeBorderThickness();
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
        /// Adorners that will be added to image adorner layer
        /// </summary>
        private readonly List<GeometryAdorner> _adorners;

        /// <summary>
        /// Image adorner layer
        /// </summary>
        private AdornerLayer _layer;

        /// <summary>
        /// Data context to manage database
        /// </summary>
        private readonly ApplicationContext _db;

        #endregion

        /// <summary>
        /// Constructor to subscribe to <see cref="IDSentEvent"/> and <see cref="ChangeBorderThicknessEvent"/> event aggregators
        /// </summary>
        public DrawingBehavior()
        {
            _adorners = new List<GeometryAdorner>();
            _db = new ApplicationContext();
        }

        #region Methods

        protected override void OnAttached()
        {
            base.OnAttached();

            AssociatedObject.MouseLeftButtonDown += OnMouseLeftButtonDown;
            AssociatedObject.MouseMove += OnMouseMove;
            AssociatedObject.MouseLeftButtonUp += OnMouseLeftButtonUp;
            
            AssociatedObject.Loaded += AssociatedObject_Loaded;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();

            AssociatedObject.MouseLeftButtonDown -= OnMouseLeftButtonDown;
            AssociatedObject.MouseMove -= OnMouseMove;
            AssociatedObject.MouseLeftButtonUp -= OnMouseLeftButtonUp;

            AssociatedObject.Loaded -= AssociatedObject_Loaded;
        }

        /// <summary>
        /// Initialize new adorner and get adorner layer of existing image
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (BehaviorType != BehaviorType.Drawing) return;

            _startPoint = e.GetPosition(AssociatedObject);

            var adorner = new GeometryAdorner(AssociatedObject, null)
            {
                Cursor = Cursors.Hand,
                BorderBrush = new SolidColorBrush(BorderColor),
                BorderThickness = BorderThickness,
                BackgroundOpacity = BackgroundOpacity,
                BackgroundColor = BackgroundColor
            };

            if (_layer == null) _layer = AdornerLayer.GetAdornerLayer(AssociatedObject);

            _layer.Add(adorner);
            _adorners.Add(adorner);

            adorner.MouseMove += OnMouseMove;
            adorner.MouseLeftButtonDown += OnMouseLeftButtonDown;
            adorner.MouseLeftButtonUp += OnMouseLeftButtonUp;
            adorner.MouseRightButtonDown += OnMouseRightButtonDown;
        }

        /// <summary>
        /// Removes adorner from layer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            var adorner = sender as GeometryAdorner;
            _layer.Remove(adorner);
            _adorners.Remove(adorner);

            _db.RemoveEditModel(adorner.ID);
        }

        /// <summary>
        /// Draw geometry above image
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (BehaviorType != BehaviorType.Drawing) return;

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                _endPoint = e.GetPosition(AssociatedObject);

                switch (Shape)
                {
                    case ShapeType.Rectangle: DrawRectangle(_adorners.Last());
                        break;
                    case ShapeType.Ellipse: DrawEllipse(_adorners.Last());
                        break;
                    case ShapeType.Line: DrawLine(_adorners.Last());
                        break;
                    case ShapeType.Polyline: DrawPolyline(_adorners.Last());
                        break;
                }
            }
        }

        /// <summary>
        /// Insert <see cref="EditModel"/> into database
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (BehaviorType != BehaviorType.Drawing) return;

            if (_adorners.Last().Geometry == null) return;

            var path = _adorners.Last().Geometry.GetFlattenedPathGeometry().ToString();
            var bgColor = _adorners.Last().BackgroundColor.ToString();
            var bgOpacity = _adorners.Last().BackgroundOpacity;
            var borderBrush = _adorners.Last().BorderBrush.ToString();

            var editModel = new EditModel(CurrentImageID, path, bgColor, bgOpacity, borderBrush);
            _db.InsertEditModel(editModel);
            EditModels.Add(editModel);
        }

        /// <summary>
        /// Draw all geometries above image after it`s first load
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AssociatedObject_Loaded(object sender, RoutedEventArgs e)
        {
            DrawGeometries(EditModels);
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
        private void DrawEllipse(GeometryAdorner adorner)
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
        /// Draw a polyline
        /// </summary>
        /// <param name="adorner"></param>
        private void DrawPolyline(GeometryAdorner adorner)
        {
            if (adorner.Geometry == null)
            {
                adorner.Geometry = new PathGeometry();
                adorner.BackgroundColor = Colors.Transparent;
            }

            var pathGeometry = adorner.Geometry as PathGeometry;

            if (pathGeometry == null) return;

            if (pathGeometry.Figures.Count == 0)
            {
                var pathFigure = new PathFigure
                {
                    StartPoint = _startPoint
                };

                pathGeometry.Figures.Add(pathFigure);
            }

            pathGeometry.Figures.Last().Segments.Add(new LineSegment(_endPoint, true));

            adorner.InvalidateVisual();
        }

        /// <summary>
        /// Changes geometries border thickness 
        /// </summary>
        private void ChangeBorderThickness()
        {
            if (AssociatedObject == null) return;

            if (_layer == null)
            {
                _layer = AdornerLayer.GetAdornerLayer(AssociatedObject);
            }

            foreach (var adorner in _adorners)
            {
                adorner.BorderThickness = BorderThickness;
            }
        }

        /// <summary>
        /// Gets all geometries related to image with <paramref name="imageModelID"/> from database and draws them
        /// </summary>
        /// <param name="imageModelID">ID of image</param>
        private void DrawGeometries(ObservableCollection<EditModel> editModels)
        {
            if (AssociatedObject == null) return;

            if (_layer == null)
            {
                _layer = AdornerLayer.GetAdornerLayer(AssociatedObject);
            }

            foreach (var adorner in _adorners)
            {
                _layer.Remove(adorner);
            }

            foreach (var model in editModels)
            {
                var borderColor = (Color)ColorConverter.ConvertFromString(model.BorderBrush);
                var backgroundColor = (Color)ColorConverter.ConvertFromString(model.BackgroundColor);

                var adorner = new GeometryAdorner(AssociatedObject, Geometry.Parse(model.Path))
                {
                    ID = model.ID,
                    Cursor = Cursors.Hand,
                    BorderBrush = new SolidColorBrush(borderColor),
                    BorderThickness = BorderThickness,
                    BackgroundOpacity = model.BackgroundOpacity,
                    BackgroundColor = backgroundColor
                };

                adorner.MouseMove += OnMouseMove;
                adorner.MouseLeftButtonDown += OnMouseLeftButtonDown;
                adorner.MouseLeftButtonUp += OnMouseLeftButtonUp;
                adorner.MouseRightButtonDown += OnMouseRightButtonDown;

                _adorners.Add(adorner);
                _layer.Add(adorner);
            }
        }

        #endregion
    }
}
