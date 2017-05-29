namespace Miruken.Mvc.Console
{
    public class Size
    {
        public int Width  { get; set; }
        public int Height { get; set; }

        public Size()
        {
        }

        public Size(Size size)
        {
            Width  = size.Width;
            Height = size.Height;
        }

        public Size(int width, int height)
        {
            Width  = width;
            Height = height;
        }

        public static Size Default => new Size(0, 0);
    }
}