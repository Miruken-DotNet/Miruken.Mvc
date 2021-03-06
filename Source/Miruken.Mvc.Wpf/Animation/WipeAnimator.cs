﻿namespace Miruken.Mvc.Wpf.Animation
{
    using System;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using Mvc.Animation;

    public class WipeAnimator : BlendAnimator<Wipe>
    {
        public WipeAnimator(Wipe wipe) : base(wipe)
        {
        }

        public override void Transition(
            Storyboard storyboard,
            ViewController fromView, ViewController toView,
            bool present = true)
        {
            var wipe     = Animation;
            var mode     = wipe.Mode ?? Mode.In;
            var duration = storyboard.Duration.TimeSpan;

            Fade(storyboard, wipe.Fade, fromView, toView,
                 present, mode);

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

            view.OpacityMask = brush;

            var overlap = wipe.OverlapDuration ?? TimeSpan.FromMilliseconds(50);

            for (var index = 0; index < brush.GradientStops.Count; ++index)
            {
                var animation = new DoubleAnimation(offsetEnd, duration);
                if (index == offsetStart)
                    animation.BeginTime = overlap;
                Configure(animation, wipe, false);
                storyboard.Children.Add(animation);
                Storyboard.SetTarget(animation, view);
                Storyboard.SetTargetProperty(animation,
                    new PropertyPath($"OpacityMask.GradientStops[{index}].Offset"));
            }
        }

        public override void Animate(
            Storyboard storyboard, ViewController view,
            bool animateOut, bool present)
        {
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
                    brush.EndPoint   = new Point(0, 1);
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
