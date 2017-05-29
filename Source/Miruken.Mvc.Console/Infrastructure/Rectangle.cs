namespace Miruken.Mvc.Console
{
    public class Rectangle
    {
        public Point  Location { get; set; }
        public int    Height   { get; set; }
        public int    Width    { get; set; }

        public Rectangle()
        {
        }

        public Rectangle(Point point, Size size)
        {
            Location = point;
            Size     = size;
        }

        public Size Size
        {
            get
            {
                return new Size(Width, Height);
            }
            set
            {
                Height = value.Height;
                Width  = value.Width;
            }
        }
    }
}