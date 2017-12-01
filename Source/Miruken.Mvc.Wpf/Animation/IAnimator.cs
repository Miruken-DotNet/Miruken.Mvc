namespace Miruken.Mvc.Wpf.Animation
{
    using Concurrency;

    public interface IAnimator 
    {
        Promise Animate(ViewController fromView, ViewController toView);
    }
}
