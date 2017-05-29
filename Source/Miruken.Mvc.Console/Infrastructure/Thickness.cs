namespace Miruken.Mvc.Console
{
    public class Thickness
    {
        public int Left   { get; set; }
        public int Top    { get; set; }
        public int Right  { get; set; }
        public int Bottom { get; set; }

        public Thickness(int border)
            :this (border, border)
        {
        }

        public Thickness(int LeftRight, int TopBottom)
            :this (LeftRight, TopBottom, LeftRight, TopBottom)
        {
        }

        public Thickness(int left, int top, int right, int bottom)
        {
            Left   = left;
            Top    = top;
            Right  = right;
            Bottom = bottom;
        }

        public static Thickness Default => new Thickness(0);
    }
}