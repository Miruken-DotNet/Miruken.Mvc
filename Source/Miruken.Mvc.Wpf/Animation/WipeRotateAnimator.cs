namespace Miruken.Mvc.Wpf.Animation
{
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using Mvc.Animation;

    public class WipeRotateAnimator : BlendAnimator<WipeRotate>
    {
        public WipeRotateAnimator(WipeRotate wipe) 
            : base(wipe)
        {
        }

        public override void Transition(
            Storyboard storyboard,
            ViewController fromView, ViewController toView,
            bool present = true, Mode? defaultMmode = null)
        {
            var wipe    = Animation;
            var fading  = wipe.Fade != null 
                        ? new FadeAnimator(wipe.Fade) 
                        : null;
            var zooming = wipe.Zoom != null 
                        ? new ZoomAnimator(wipe.Zoom) 
                        : null;

            if (present)
            {
                fading?.Animate(storyboard, fromView, true, true);
                zooming?.Animate(storyboard, fromView, true, true);
            }
            else
            {
                fading?.Animate(storyboard, toView, false, false);
                zooming?.Animate(storyboard, toView, false, false);
            }

            if (wipe.Converge == true)
                ApplyConverge(storyboard, wipe, fromView, toView, present);
            else
                ApplyRotate(storyboard, wipe, fromView, toView, present);
        }

        public override void Animate(
            Storyboard storyboard, ViewController view,
            bool animateOut, bool present)
        {
        }

        private static void ApplyRotate(
            TimelineGroup storyboard, WipeRotate wipe,
            ViewController fromView, ViewController toView,
            bool present = true)
        {
            var transform = new RotateTransform();
            var brush     = new LinearGradientBrush
            {
                EndPoint  = new Point(1, 0),
                Transform  = transform
            };
            var gradientStops = brush.GradientStops;
            gradientStops.Add(new GradientStop(Colors.White, 0));
            gradientStops.Add(new GradientStop { Offset = .1 });

            var animation = new DoubleAnimation
            {
                Duration = storyboard.Duration
            };
            Configure(animation, wipe, false);

            ViewController view;
            if (present)
            {
                view            = toView;
                transform.Angle = 10;
                animation.To    = -90;
                toView.AddViewAbove(fromView);
            }
            else
            {
                view            = fromView;
                transform.Angle = -90;
                animation.To    = 10;
            }

            var opacityMask  = view.OpacityMask;
            view.OpacityMask = brush;

            storyboard.Children.Add(animation);
            Storyboard.SetTarget(animation, view);
            Storyboard.SetTargetProperty(animation,
                new PropertyPath("OpacityMask.Transform.Angle"));

            storyboard.Completed += (s, _) => view.OpacityMask = opacityMask;
        }

        private static void ApplyConverge(
            TimelineGroup storyboard, WipeRotate wipe,
            ViewController fromView, ViewController toView,
            bool present = true)
        {
            var mode     = wipe.Mode ?? Mode.In;
            var duration = storyboard.Duration.TimeSpan;

            double fromAngle, toAngle;
            ViewController view;
            if (present)
            {
                if (mode == Mode.Out)
                {
                    fromAngle = -45;
                    toAngle   = -5;
                    view      = fromView;
                    toView?.AddViewBelow(fromView);
                }
                else
                {
                    fromAngle = 5;
                    toAngle   = -45;
                    view      = toView;
                    toView.AddViewAbove(fromView);
                }
            }
            else
            {
                if (mode == Mode.Out)
                {
                    fromAngle = 5;
                    toAngle   = -45;
                    view      = toView;
                    toView?.AddViewAbove(fromView);
                }
                else
                {
                    fromAngle = -45;
                    toAngle   = -5;
                    view      = fromView;
                }
            }

            var brush     = new DrawingBrush();
            var group     = new DrawingGroup();
            var geometry  = Geometry.Parse("M0,0 L1,0 L1,1 L0,1 z");
            var gradients = new GradientStopCollection
            {
                new GradientStop(Colors.White, 0),
                new GradientStop { Offset = .1 }
            };

            var drawTop = new GeometryDrawing
            {
                Geometry = geometry,
                Brush    = new LinearGradientBrush
                {
                    EndPoint  = new Point(1, 0),
                    Transform = new RotateTransform
                    {
                        Angle = fromAngle
                    },
                    GradientStops = gradients
                }
            };

            var drawBottom = new GeometryDrawing
            {
                Geometry = geometry,
                Brush    = new LinearGradientBrush
                {
                    StartPoint = new Point(1, 1),
                    EndPoint   = new Point(0, 1),
                    Transform = new RotateTransform
                    {
                        CenterX = 1,
                        CenterY = 1,
                        Angle   = fromAngle,
                    },
                    GradientStops = gradients
                }
            };

            brush.Drawing = group;
            group.Children.Add(drawTop);
            group.Children.Add(drawBottom);

            var opacityMask  = view.OpacityMask;
            view.OpacityMask = brush;

            var animationTop = new DoubleAnimation(toAngle, duration);
            Configure(animationTop, wipe, false);
            storyboard.Children.Add(animationTop);
            Storyboard.SetTarget(animationTop, view);
            Storyboard.SetTargetProperty(animationTop,
                new PropertyPath("OpacityMask.Drawing.Children[0].Brush.Transform.Angle"));

            var animationBottom = new DoubleAnimation(toAngle, duration);
            Configure(animationBottom, wipe, false);
            storyboard.Children.Add(animationBottom);
            Storyboard.SetTarget(animationBottom, view);
            Storyboard.SetTargetProperty(animationBottom,
                new PropertyPath("OpacityMask.Drawing.Children[1].Brush.Transform.Angle"));

            storyboard.Completed += (s, _) => view.OpacityMask = opacityMask;
        }
    }
}
