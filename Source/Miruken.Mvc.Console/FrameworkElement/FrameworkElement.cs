namespace Miruken.Mvc.Console
{
    using System;

    public abstract class FrameworkElement
    {
        public Size                Size                { get; set; }
        public Size                DesiredSize         { get; set; }
        public Size                ActualSize          { get; set; }
        public Thickness           Margin              { get; set; }
        public Thickness           Border              { get; set; }
        public Thickness           Padding             { get; set; }
        public Point               Point               { get; set; }
        public VerticalAlignment   VerticalAlignment   { get; set; }
        public HorizontalAlignment HorizontalAlignment { get; set; }
        public Boundry             ContentBoundry      { get; set; }

        protected FrameworkElement()
        {
            Point       = Point.Default;
            Margin      = Thickness.Default;
            Border      = Thickness.Default;
            Padding     = Thickness.Default;
        }

        public virtual void Initialize()
        {
        }

        public virtual void Measure(Size availableSize)
        {
            if (Size == null)
            {
                DesiredSize = new Size(availableSize);
                return;
            }

            var height = Size.Height > availableSize.Height
               ? availableSize.Height
               : Size.Height;
            var width = Size.Width > availableSize.Width
               ? availableSize.Width
               : Size.Width;

            DesiredSize = new Size(Math.Max(0, width),  Math.Max(0, height));
        }

        public virtual void Arrange(Rectangle rectangle)
        {
            Point = rectangle.Location;

            var availableSize = rectangle.Size;
            var height = DesiredSize.Height > availableSize.Height
               ? availableSize.Height
               : DesiredSize.Height;
            var width = DesiredSize.Width > availableSize.Width
               ? availableSize.Width
               : DesiredSize.Width;
            ActualSize = new Size(Math.Max(width, 0),  Math.Max(height, 0));
        }

        public virtual void Render(Cells cells)
        {
            new RenderElement().Handle(this, cells);
        }

        public virtual void KeyPressed(ConsoleKeyInfo keyInfo)
        {
        }

        public virtual Boundry Boundry
        {
            get
            {
                var x2 = Point.X + ActualSize.Width;
                var y2 = Point.Y + ActualSize.Height;
                return new Boundry(new Point(Point), new Point(x2, y2));
            }
        }
    }
}
