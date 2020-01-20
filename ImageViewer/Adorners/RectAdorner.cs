using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace ImageViewer.Adorners
{
    public class RectAdorner : Adorner
    {
        /// <summary>
        /// Rectangle to draw on image
        /// </summary>
        public Rect Rect { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="adornedElement">Element to draw on</param>
        /// <param name="rect">Rectangle to draw</param>
        public RectAdorner(UIElement adornedElement, Rect rect) : base(adornedElement)
        {
            Rect = rect;
        }

        /// <summary>
        /// Draw the <see cref="Rect"/>
        /// </summary>
        /// <param name="drawingContext"></param>
        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            SolidColorBrush renderBrush = new SolidColorBrush(Colors.Black)
            {
                Opacity = 0.2
            };
            Pen renderPen = new Pen(new SolidColorBrush(Colors.Black), 1.5);

            drawingContext.DrawRectangle(renderBrush, renderPen, Rect);
        }
    }
}
