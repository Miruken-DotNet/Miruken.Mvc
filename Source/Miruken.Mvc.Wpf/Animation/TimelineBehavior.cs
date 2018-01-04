namespace Miruken.Mvc.Wpf.Animation
{
    using System.Windows.Media.Animation;

    public class TimelineBehavior
    {
        public double?         Acceleration { get; set; }
        public FillBehavior?   Fill         { get; set; }
        public RepeatBehavior? Repeat       { get; set; }
        public double?         Speed        { get; set; }
    }
}
