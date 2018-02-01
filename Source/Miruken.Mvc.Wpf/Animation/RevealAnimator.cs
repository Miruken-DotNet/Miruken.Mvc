namespace Miruken.Mvc.Wpf.Animation
{
    using System;
    using System.Linq;
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
            var reveal  = Animation;
            var mode    = reveal.Mode ?? Mode.Out;
            var fading  = FadeAnimator.For(reveal);
            var zooming = ZoomAnimator.For(reveal.Zoom);

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
            var part1    = view.CreateImageView(snapshot);
            var part2    = view.CreateImageView(snapshot);
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
                case Origin.TopLeft:
                case Origin.BottomRight:
                    Translate(storyboard, part1, Origin.TopLeft, animateOut);
                    Translate(storyboard, part2, Origin.BottomRight, animateOut);
                    break;
                case Origin.TopRight:
                case Origin.BottomLeft:
                    Translate(storyboard, part1, Origin.TopRight, animateOut);
                    Translate(storyboard, part2, Origin.BottomLeft, animateOut);
                    break;
            }
        }

        private void Translate(
            Storyboard storyboard, ViewController view, 
            Origin origin, bool animateOut)
        {
            var size     = view.RegionSize;
            var duration = storyboard.Duration;

            DoubleAnimation translateX = null;
            DoubleAnimation translateY = null;
            var property = view.AddTransform(new TranslateTransform());

            switch (origin)
            {
                case Origin.TopLeft:
                case Origin.MiddleLeft:
                case Origin.BottomLeft:
                    if (origin == Origin.MiddleLeft)
                        size.Width = size.Width / 2;
                    translateX = new DoubleAnimation(
                        animateOut ? 0 : -size.Width,
                        animateOut ? -size.Width : 0,
                        duration);
                    break;
                case Origin.TopRight:
                case Origin.MiddleRight:
                case Origin.BottomRight:
                    if (origin == Origin.MiddleRight)
                        size.Width = size.Width / 2;
                    translateX = new DoubleAnimation(
                        animateOut ? 0 : size.Width,
                        animateOut ? size.Width : 0,
                        duration);
                    break;
            }

            switch (origin)
            {
                case Origin.TopLeft:
                case Origin.TopRight:
                case Origin.TopCenter:
                    if (origin == Origin.TopCenter)
                        size.Height = size.Height / 2;
                    translateY  = new DoubleAnimation(
                        animateOut ? 0 : -size.Height,
                        animateOut ? -size.Height : 0,
                        duration);
                    break;
                case Origin.BottomCenter:
                case Origin.BottomRight:
                case Origin.BottomLeft:
                    if (origin == Origin.BottomCenter)
                        size.Height = size.Height / 2;
                    translateY  = new DoubleAnimation(
                        animateOut ? 0 : size.Height,
                        animateOut ? size.Height : 0,
                        duration);
                    break;
            }

            switch (origin)
            {
                case Origin.MiddleLeft:
                    view.Clip = new RectangleGeometry(new Rect(size));
                    break;
                case Origin.MiddleRight:
                    view.Clip = new RectangleGeometry(new Rect(
                        new Point(size.Width, 0), size));
                    break;
                case Origin.TopCenter:
                    view.Clip = new RectangleGeometry(new Rect(size));
                    break;
                case Origin.BottomCenter:
                    view.Clip = new RectangleGeometry(new Rect(
                        new Point(0, size.Height), size));
                    break;
                case Origin.TopLeft:
                    view.Clip = CreatePath(
                        new Point(0, size.Height),
                        new Point(0, 0),
                        new Point(size.Width, 0));
                    break;
                case Origin.TopRight:
                    view.Clip = CreatePath(
                        new Point(0, 0),
                        new Point(size.Width, 0),
                        new Point(size.Width, size.Height));
                    break;
                case Origin.BottomRight:
                    view.Clip = CreatePath(
                        new Point(size.Width, 0),
                        new Point(size.Width, size.Height),
                        new Point(0, size.Height));
                    break;
                case Origin.BottomLeft:
                    view.Clip = CreatePath(
                        new Point(0, 0),
                        new Point(0, size.Height),
                        new Point(size.Width, size.Height));
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

        private static PathGeometry CreatePath(params Point[] points)
        {
            return points.Length >= 3 
                 ? new PathGeometry(new PathFigureCollection
                 {
                     new PathFigure
                     {
                         StartPoint = points[0],
                         Segments   = new PathSegmentCollection(
                             points.Skip(1).Select(p => new LineSegment(p, false)))

                     }
                 }) : null;
        }
    }
}
