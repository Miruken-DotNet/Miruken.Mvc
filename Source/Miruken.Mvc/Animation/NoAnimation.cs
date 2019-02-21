namespace Miruken.Mvc.Animation
{
    using Callback;
    using Options;

    public class NoAnimation : Animation
    {
        public static readonly NoAnimation Instance = new NoAnimation();

        private NoAnimation()
        {    
        }
    }

    public static class NoAnimationExtensions
    {
        public static IHandler NoAnimation(this IHandler handler)
        {
            return new NavigationOptions
            {
                Animation = Mvc.Animation.NoAnimation.Instance
            }.Decorate(handler);
        }
    }
}
