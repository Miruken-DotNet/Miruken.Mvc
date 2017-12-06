namespace Miruken.Mvc.Wpf.Animation
{
    using System;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using Concurrency;
    using Mvc.Animation;

    public class SplitAnimator : Animator
    {
        private const int DefaultWidth = 30;

        public SplitAnimator(Split split)
        {
            if (split == null)
                throw new ArgumentNullException(nameof(split));
            Split = split;
        }

        public Split Split { get; }

        public override Promise Present(
            ViewController fromView, ViewController toView,
            bool removeFromView)
        {
            return AnimateStory(Split, fromView, toView,
                (storyboard, duration) =>
                {
                    FadeAnimator.Apply(storyboard, Split.Fade,
                        fromView, toView, duration, true, Split.Mode ?? Mode.InOut);
                    Apply(storyboard, Split, fromView, toView, duration);
                }, removeFromView);
        }

        public override Promise Dismiss(
            ViewController fromView, ViewController toView)
        {
            return AnimateStory(Split, fromView, toView,
                (storyboard, duration) =>
                {
                    FadeAnimator.Apply(storyboard, Split.Fade,
                        fromView, toView, duration, false, Split.Mode ?? Mode.InOut);
                    Apply(storyboard, Split, fromView, toView, duration, false);
                });
        }

        public static void Apply(TimelineGroup storyboard,
            Split split, ViewController fromView, ViewController toView,
            TimeSpan duration, bool present = true)
        {
            var width    = split.Width ?? DefaultWidth;
            var vertical = split.Orientation != Orientation.Horizontal;
            var brush    = new LinearGradientBrush
            {
                MappingMode  = BrushMappingMode.Absolute,
                SpreadMethod = GradientSpreadMethod.Repeat,
                StartPoint   = new Point(0, 0),
                EndPoint     = vertical ? new Point(width, 0) : new Point(0, width)
            };
            var gradientStops = brush.GradientStops;
            ViewController view;
            if (present)
            {
                view = toView;
                toView.AddViewAbove(fromView);
                gradientStops.Add(new GradientStop(Colors.White, 0));
                gradientStops.Add(new GradientStop());

            }
            else
            {
                view = fromView;
                gradientStops.Add(new GradientStop { Offset = 1 });
                gradientStops.Add(new GradientStop(Colors.White, 0));
            }

            var opacityMask  = view.OpacityMask;
            view.OpacityMask = brush;

            for (var index = 0; index < gradientStops.Count; ++index)
            {
                var animation = new DoubleAnimation(present ? 1 : 0, duration);
                if (index == 0)
                    animation.BeginTime = new TimeSpan(duration.Ticks / 2);
                storyboard.Children.Add(animation);
                Storyboard.SetTarget(animation, view);
                Storyboard.SetTargetProperty(animation,
                    new PropertyPath($"OpacityMask.GradientStops[{index}].Offset"));
            }

            storyboard.Completed += (s, _) => view.OpacityMask = opacityMask;
        }
    }
}
