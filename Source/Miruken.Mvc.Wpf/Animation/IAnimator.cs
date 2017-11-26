namespace Miruken.Mvc.Wpf.Animation
{
    using System.Windows;
    using Concurrency;

    public interface IAnimator 
    {
        Promise Animate(
            ViewRegion region,    
            FrameworkElement oldView,
            FrameworkElement newView);
    }
}
