namespace Miruken.Mvc.Wpf.Animation
{
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using Mvc.Animation;

    public class RevealAnimator : BlendAnimator<Reveal>
    {
        public RevealAnimator(Reveal animation) 
            : base(animation)
        {
        }

        public override void Transition(
            Storyboard storyboard,
            ViewController fromView, ViewController toView,
            bool present = true)
        {
            var reveal = Animation;
            var mode   = reveal.Mode ?? Mode.Out;

            var fading = reveal.Fade != null
                ? new FadeAnimator(reveal.Fade)
                : null;

            var zooming = reveal.Zoom != null
                ? new ZoomAnimator(reveal.Zoom)
                : null;

            switch (mode)
            { 
                case Mode.In:
                    if (present)
                    {
                        zooming?.Animate(storyboard, fromView, true, true);
                        Animate(storyboard, toView, false, true);
                    }
                    else
                    {
                        zooming?.Animate(storyboard, toView, false, true);
                        Animate(storyboard, fromView, false, false);
                    }
                    break;
                case Mode.Out:
                case Mode.InOut:
                    if (present)
                    {
                        toView?.AddViewAbove(fromView);
                        if (mode == Mode.InOut)
                            fading?.Animate(storyboard, toView, false, true);
                        zooming?.Animate(storyboard, toView, false, true);
                        Animate(storyboard, fromView, true, true);
                    }
                    else
                    {
                        if (mode == Mode.InOut)
                            fading?.Animate(storyboard, fromView, true, false);
                        zooming?.Animate(storyboard, fromView, true, false);
                        Animate(storyboard, toView, true, false);
                    }
                    break;
            }
        }

        public override void Animate(
            Storyboard storyboard, ViewController view,
            bool animateOut, bool present)
        {
            var snapshot = view.CreateSnapshot();
            var part1    = view.CreateImage(snapshot);
            var part2    = view.CreateImage(snapshot);
            animateOut   = !(animateOut ^ present);

            view.HideView();
            WhenComplete(storyboard, view.ShowView);

            foreach (var part in new[] { part1, part2 })
            {
                Fade(storyboard, Animation.Fade,
                    animateOut ? part : null,
                    animateOut ? null : part,
                    present);
            }

            switch (Animation.Origin ?? Origin.MiddleLeft)
            {
                case Origin.MiddleLeft:
                case Origin.MiddleRight:
                    Translate(storyboard, part1, Origin.MiddleLeft, animateOut);
                    Translate(storyboard, part2, Origin.MiddleRight, animateOut);
                    break;
                case Origin.TopCenter:
                case Origin.BottomCenter:
                    Translate(storyboard, part1, Origin.TopCenter, animateOut);
                    Translate(storyboard, part2, Origin.BottomCenter, animateOut);
                    break;
            }
        }

        private void Translate(
            Storyboard storyboard, ViewController view, 
            Origin origin, bool animateOut)
        {
            var duration = storyboard.Duration;
            var size     = new Size(view.RegionWidth, view.RegionHeight);

            DoubleAnimation translateX = null;
            DoubleAnimation translateY = null;
            var property = view.AddTransform(new TranslateTransform());

            switch (origin)
            {
                case Origin.MiddleLeft:
                    size.Width = size.Width / 2;
                    view.Clip  = new RectangleGeometry(new Rect(size));
                    translateX = new DoubleAnimation(
                        animateOut ? 0 : -size.Width,
                        animateOut ? -size.Width : 0,
                        duration);
                    break;
                case Origin.MiddleRight:
                    size.Width = size.Width / 2;
                    view.Clip  = new RectangleGeometry(new Rect(
                        new Point(size.Width, 0), size));
                    translateX = new DoubleAnimation(
                        animateOut ? 0 : size.Width,
                        animateOut ? size.Width : 0,
                        duration);
                    break;
                case Origin.TopCenter:
                    size.Height = size.Height / 2;
                    view.Clip   = new RectangleGeometry(new Rect(size));
                    translateY  = new DoubleAnimation(
                        animateOut ? 0 : -size.Height,
                        animateOut ? -size.Height : 0,
                        duration);
                    break;
                case Origin.BottomCenter:
                    size.Height = size.Height / 2;
                    view.Clip   = new RectangleGeometry(new Rect(
                        new Point(0, size.Height), size));
                    translateY  = new DoubleAnimation(
                        animateOut ? 0 : size.Height,
                        animateOut ? size.Height : 0,
                        duration);
                    break;
            }

            if (translateX != null)
            {
                Configure(translateX, Animation, animateOut);
                storyboard.Children.Add(translateX);
                Storyboard.SetTarget(translateX, view);
                Storyboard.SetTargetProperty(translateX,
                    property(TranslateTransform.XProperty));
            }

            if (translateY != null)
            {
                Configure(translateY, Animation, animateOut);
                storyboard.Children.Add(translateY);
                Storyboard.SetTarget(translateY, view);
                Storyboard.SetTargetProperty(translateY,
                    property(TranslateTransform.YProperty));
            }

            WhenComplete(storyboard, () => view.RemoveView());
            view.AddView();
        }
    }
}
