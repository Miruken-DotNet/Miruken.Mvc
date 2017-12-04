namespace Miruken.Mvc.Wpf.Animation
{
    using Concurrency;

    public interface IAnimator 
    {
        Promise Present(
            ViewController fromView, ViewController toView,
            bool removeFromView);

        Promise Dismiss(
            ViewController fromView, ViewController toView);
    }
}
