namespace Miruken.Mvc.Console
{
    using System.Collections.Generic;
    using System.Linq;

    public class StackPanel : Panel
    {
        private readonly List<FrameworkElement> _children = new List<FrameworkElement>();

        public override FrameworkElement[] Children => _children.ToArray();

        public override void Measure(Size availableSize)
        {
            base.Measure(availableSize);
            foreach (var child in Children)
                child.Measure(DesiredSize);
        }

        public void Add(FrameworkElement child)
        {
            _children.Add(child);
        }

        public void Remove(FrameworkElement child)
        {
            if (_children.Contains(child))
                _children.Remove(child);
        }

        public override void RemoveLast()
        {
            if (_children.Any())
                _children.Remove(_children.Last());
        }

        public override void Arrange(Rectangle rectangle)
        {
            base.Arrange(rectangle);

            var xStart = Point.X + Margin.Left + Border.Left   + Padding.Left;
            var yStart = Point.Y + Margin.Top  + Border.Top    + Padding.Top;
            var xEnd   = Point.X + rectangle.Size.Width  - Margin.Right  - Border.Right  - Padding.Right;
            var yEnd   = Point.Y + rectangle.Size.Height - Margin.Bottom - Border.Bottom - Padding.Bottom;
            var boundry = new Boundry(new Point(xStart, yStart), new Point(xEnd, yEnd));

            foreach (var child in Children)
            {
                var xOffset = 0;
                var yOffset = 0;

                switch (child.HorizontalAlignment)
                {
                    case HorizontalAlignment.Left:
                        break;
                    case HorizontalAlignment.Center:
                        if (child.DesiredSize.Width < boundry.Width && (boundry.Width - child.DesiredSize.Width)%2 > 0 )
                            child.DesiredSize.Width++;
                        xOffset = (boundry.Width - child.DesiredSize.Width)/2;
                        break;
                    case HorizontalAlignment.Right:
                        if (child.DesiredSize.Width < boundry.Width)
                            boundry.Start.X = boundry.End.X - child.DesiredSize.Width;
                        break;
                    default:
                        child.DesiredSize.Width = boundry.Width;
                        break;
                }

                switch (child.VerticalAlignment)
                {
                    case VerticalAlignment.Top:
                        break;
                    case VerticalAlignment.Center:
                        if (child.DesiredSize.Height < boundry.Height && (boundry.Height - child.DesiredSize.Height)%2 > 0)
                            child.DesiredSize.Height++;
                        yOffset = (boundry.Height - child.DesiredSize.Height)/2;
                        break;
                    case VerticalAlignment.Bottom:
                        if (child.DesiredSize.Height < boundry.Height)
                            boundry.Start.Y = boundry.End.Y - child.DesiredSize.Height;
                        break;
                    default:
                        child.DesiredSize.Height = boundry.Height;
                        break;
                }

                child.Arrange(new Rectangle(
                    new Point(boundry.Start.X + xOffset, boundry.Start.Y + yOffset),
                    new Size(boundry.Width, boundry.Height)));
            }
        }
    }
}