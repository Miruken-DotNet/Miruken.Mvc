namespace Miruken.Mvc.Wpf.Animation
{
    using System.Windows.Media.Animation;

    public class TimelineBehavior
    {
        public double?         AccelerationRatio { get; set; }
        public double?         DecelerationRatio { get; set; }
        public FillBehavior?   FillBehavior      { get; set; }
        public RepeatBehavior? RepeatBehavior    { get; set; }
        public double?         SpeedRatio        { get; set; }
    }
}
