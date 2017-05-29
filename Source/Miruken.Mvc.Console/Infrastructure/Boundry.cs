namespace Miruken.Mvc.Console
{
    using System;

    public class Boundry    
    {
        public Boundry(Point start, Point end)
        {
            Start = start;
            End   = end;
        }

        public Point Start { get; set; }
        public Point End   { get; set; }

        public int Width           => Math.Max(End.X - Start.X, 0);
        public int Height          => Math.Max(End.Y - Start.Y, 0);
        public Rectangle Rectangle => new Rectangle(new Point(Start), new Size(Width, Height));
    }
}