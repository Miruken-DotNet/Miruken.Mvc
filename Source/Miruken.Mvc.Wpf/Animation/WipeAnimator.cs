namespace Miruken.Mvc.Wpf.Animation
{
    using System;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using Concurrency;
    using Mvc.Animation;

    public class WipeAnimator : Animator
    {
        public WipeAnimator(Wipe wipe)
        {
            if (wipe == null)
                throw new ArgumentNullException(nameof(wipe));
            Wipe = wipe;
        }

        public Wipe Wipe { get; }

        public override Promise Present(
            ViewController fromView, ViewController toView,
            bool removeFromView)
        {
            return AnimateStory(Wipe, fromView, toView,
                (storyboard, duration) =>
                {
                    FadeAnimator.Apply(storyboard, Wipe.Fade,
                        fromView, toView, duration, true, Wipe.Mode ?? Mode.InOut);
                    Apply(storyboard, Wipe, fromView, toView, duration);
                }, removeFromView);
        }

        public override Promise Dismiss(
            ViewController fromView, ViewController toView)
        {
            return AnimateStory(Wipe, fromView, toView,
                (storyboard, duration) =>
                {
                    FadeAnimator.Apply(storyboard, Wipe.Fade,
                        fromView, toView, duration, false, Wipe.Mode ?? Mode.InOut);
                    Apply(storyboard, Wipe, fromView, toView, duration, false);
                });
        }

        public void Apply(TimelineGroup storyboard,
            Wipe wipe, ViewController fromView, ViewController toView,
            TimeSpan duration, bool present = true)
        {
            var mode = wipe.Mode ?? Mode.In;

            var offsetStart = 0;
            ViewController view;
            if (present)
            {
                if (mode == Mode.Out)
                {
                    offsetStart = 1;
                    view        = fromView;
                    toView?.AddViewBelow(fromView);
                }
                else
                {
                    view = toView;
                    toView.AddViewAbove(fromView);
                }
            }
            else
            {
                if (mode == Mode.Out)
                {
                    view = toView;
                    toView?.AddViewAbove(fromView);
                }
                else
                {
                    offsetStart = 1;
                    view = fromView;
                }
            }
            var offsetEnd = offsetStart == 0 ? 1 : 0;

            var brush = new LinearGradientBrush();
            ConfigureGradients(wipe, brush, offsetStart);

            var opacityMask  = view.OpacityMask;
            view.OpacityMask = brush;

            var overlap = wipe.OverlapDuration ?? TimeSpan.FromMilliseconds(50);

            for (var index = 0; index < brush.GradientStops.Count; ++index)
            {
                var animation = new DoubleAnimation(offsetEnd, duration);
                if (index == offsetStart)
                    animation.BeginTime = overlap;
                storyboard.Children.Add(animation);
                Storyboard.SetTarget(animation, view);
                Storyboard.SetTargetProperty(animation,
                    new PropertyPath($"OpacityMask.GradientStops[{index}].Offset"));
            }

            storyboard.Completed += (s, _) => view.OpacityMask = opacityMask;
        }

        private static void ConfigureGradients(Wipe wipe, 
            LinearGradientBrush brush, double offsetStart)
        {
            switch (wipe.Start ?? Origin.MiddleLeft)
            {
                case Origin.TopLeft:
                    brush.EndPoint = new Point(1, 1);
                    break;
                case Origin.MiddleLeft:
                case Origin.MiddleCenter:
                    brush.EndPoint = new Point(1, 0);
                    break;
                case Origin.BottomLeft:
                    brush.StartPoint = new Point(0, 1);
                    brush.EndPoint   = new Point(1, 0);
                    break;
                case Origin.TopRight:
                    brush.StartPoint = new Point(1, 0);
                    brush.EndPoint = new Point(0, 1);
                    break;
                case Origin.MiddleRight:
                    brush.StartPoint = new Point(1, 0);
                    brush.EndPoint   = new Point(0, 0);
                    break;
                case Origin.BottomRight:
                    brush.StartPoint = new Point(1, 1);
                    brush.EndPoint   = new Point(0, 0);
                    break;
                case Origin.TopCenter:
                    brush.EndPoint = new Point(0, 1);
                    break;
                case Origin.BottomCenter:
                    brush.StartPoint = new Point(0, 1);
                    brush.EndPoint   = new Point(0, 0);
                    break;
            }
            var gradientStops = brush.GradientStops;
            gradientStops.Add(new GradientStop(Colors.White, offsetStart));
            gradientStops.Add(new GradientStop { Offset = offsetStart });
        }
    }
}
