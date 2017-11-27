namespace Miruken.Mvc.Wpf.Animation
{
    using Concurrency;

    public interface IAnimator 
    {
        Promise Animate(ViewController oldView, ViewController newView);
    }
}
