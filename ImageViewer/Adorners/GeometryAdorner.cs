using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace ImageViewer.Adorners
{
    public class GeometryAdorner : Adorner
    {
        /// <summary>
        /// ID of adorner
        /// </summary>
        public int ID { get; set; }
        /// <summary>
        /// Geometry to draw on image
        /// </summary>
        public Geometry Geometry { get; set; }

        /// <summary>
        /// Area to be croped
        /// </summary>
        public Rect Rect { get; set; }

        /// <summary>
        /// Border color
        /// </summary>
        public SolidColorBrush BorderBrush { get; set; } = Brushes.Black;

        /// <summary>
        /// Background color
        /// </summary>
        public Color BackgroundColor { get; set; } = Colors.Black;

        /// <summary>
        /// Border thickness
        /// </summary>
        public double BorderThickness { get; set; } = 1.5;

        /// <summary>
        /// Background opacity
        /// </summary>
        public double BackgroundOpacity { get; set; } = 0.2;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="adornedElement">Element to draw on</param>
        /// <param name="rect">Rectangle to draw</param>
        public GeometryAdorner(UIElement adornedElement, Rect rect) : base(adornedElement)
        {
            Rect = rect;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="adornedElement">Element to draw on</param>
        /// <param name="geometry">Geometry to draw</param>
        public GeometryAdorner(UIElement adornedElement, Geometry geometry) : base(adornedElement)
        {
            Geometry = geometry;
        }

        /// <summary>
        /// Draw the <see cref="Geometry"/>
        /// </summary>
        /// <param name="drawingContext"></param>
        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            SolidColorBrush renderBrush = new SolidColorBrush(BackgroundColor)
            {
                Opacity = BackgroundOpacity
            };
            Pen renderPen = new Pen(BorderBrush, BorderThickness);

            drawingContext.DrawGeometry(renderBrush, renderPen, Geometry);
        }
    }
}
