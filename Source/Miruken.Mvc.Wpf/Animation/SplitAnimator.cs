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
            return AnimateStory(Split, fromView, toView, storyboard =>
            {
                FadeAnimator.Apply(storyboard, Split.Fade,
                    fromView, toView, true, Split.Mode ?? Mode.InOut);
                Apply(storyboard, Split, fromView, toView);
            }, removeFromView);
        }

        public override Promise Dismiss(
            ViewController fromView, ViewController toView)
        {
            return AnimateStory(Split, fromView, toView, storyboard =>
            {
                FadeAnimator.Apply(storyboard, Split.Fade,
                    fromView, toView, false, Split.Mode ?? Mode.InOut);
                Apply(storyboard, Split, fromView, toView, false);
            });
        }

        public void Apply(TimelineGroup storyboard,
            Split split, ViewController fromView, ViewController toView,
            bool present = true)
        {
            var mode     = split.Mode ?? Mode.In;
            var duration = storyboard.Duration.TimeSpan;
 
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

            var brush = new LinearGradientBrush
            {
                MappingMode  = BrushMappingMode.Absolute,
                SpreadMethod = GradientSpreadMethod.Repeat
            };
            ConfigureGradients(split, brush, offsetStart);

            var opacityMask  = view.OpacityMask;
            view.OpacityMask = brush;

            for (var index = 0; index < brush.GradientStops.Count; ++index)
            {
                var animation = new DoubleAnimation(offsetEnd, duration);
                if (index == offsetStart)
                    animation.BeginTime = new TimeSpan(duration.Ticks / 2);
                Configure(animation, split);
                storyboard.Children.Add(animation);
                Storyboard.SetTarget(animation, view);
                Storyboard.SetTargetProperty(animation,
                    new PropertyPath($"OpacityMask.GradientStops[{index}].Offset"));
            }

            storyboard.Completed += (s, _) => view.OpacityMask = opacityMask;
        }

        private static void ConfigureGradients(Split split, 
            LinearGradientBrush brush, double offsetStart)
        {
            var width  = split.Width ?? DefaultWidth;
            switch (split.Start ?? Origin.MiddleLeft)
            {
                case Origin.TopLeft:
                    brush.EndPoint = new Point(width, width);
                    break;
                case Origin.MiddleLeft:
                case Origin.MiddleCenter:
                    brush.EndPoint = new Point(width, 0);
                    break;
                case Origin.BottomLeft:
                    brush.StartPoint = new Point(0, width);
                    brush.EndPoint   = new Point(width, 0);
                    break;
                case Origin.TopRight:
                    brush.StartPoint = new Point(width, 0);
                    brush.EndPoint = new Point(0, width);
                    break;
                case Origin.MiddleRight:
                    brush.StartPoint = new Point(width, 0);
                    brush.EndPoint   = new Point(0, 0);
                    break;
                case Origin.BottomRight:
                    brush.StartPoint = new Point(width, width);
                    brush.EndPoint   = new Point(0, 0);
                    break;
                case Origin.TopCenter:
                    brush.EndPoint = new Point(0, width);
                    break;
                case Origin.BottomCenter:
                    brush.StartPoint = new Point(0, width);
                    brush.EndPoint   = new Point(0, 0);
                    break;
            }
            var gradientStops = brush.GradientStops;
            gradientStops.Add(new GradientStop(Colors.White, offsetStart));
            gradientStops.Add(new GradientStop { Offset = offsetStart });
        }
    }
}
