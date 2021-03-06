﻿namespace Miruken.Mvc.Wpf
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;

    public class ViewController : ContentControl
    {
        private readonly ViewRegion _region;

        public ViewController(ViewRegion region, object view)
        {
            _region = region;
            Content = view;
        }

        public double RegionWidth  => _region.ActualWidth;
        public double RegionHeight => _region.ActualHeight;
        public Size   RegionSize   => new Size(RegionWidth, RegionHeight);
        public Rect   RegionRect   => new Rect(RegionSize);

        public bool   IsChild => _region.Children.Contains(this);

        public void ShowView()
        {
            Visibility = Visibility.Visible;
        }

        public void CollapseView()
        {
            Visibility = Visibility.Collapsed;
        }

        public void HideView()
        {
            Visibility = Visibility.Hidden;
        }

        public bool AddView()
        {
            var children = _region.Children;
            if (children.Contains(this))
                return false;
            children.Add(this);
            return true;
        }

        public bool AddViewAtIndex(int index)
        {
            var children = _region.Children;
            var current  = children.IndexOf(this);
            if (current == index) return false;
            children.RemoveAt(current);
            if (current < index) --index;
            children.Insert(index, this);
            return true;
        }

        public bool AddViewAbove(ViewController view)
        {
            var children = _region.Children;
            var current  = children.IndexOf(this);
            if (view == null)
            {
                if (current >= 0) return false;
                children.Add(this);
                return true;
            }
            var index = children.IndexOf(view);
            if (index < 0) return false;
            if (current >= 0)
            {
                if (current > index) return false;
                children.RemoveAt(current);
                --index;
            }
            children.Insert(index + 1, this);
            return true;
        }

        public bool AddViewBelow(ViewController view)
        {
            var children = _region.Children;
            var current  = children.IndexOf(this);
            if (view == null)
            {
                if (current >= 0) return false;
                children.Add(this);
                return true;
            }
            var index = children.IndexOf(view);
            if (index < 0) return false;
            if (current >= 0)
            {
                if (current < index) return false;
                children.RemoveAt(current);
            }
            children.Insert(index, this);
            return true;
        }

        public bool ReplaceView(ViewController view)
        {
            var children = _region.Children;
            var index = children.IndexOf(view);
            if (index < 0) return false;
            children.RemoveAt(index);
            children.Insert(index, this);
            return true;
        }

        public bool RemoveView()
        {
            var children = _region.Children;
            if (children.Contains(this))
            {
                children.Remove(this);
                return true;
            }
            return false;
        }

        public Func<DependencyProperty, PropertyPath> 
            AddTransform(Transform transform)
        {
            if (transform == null)
                throw new ArgumentNullException(nameof(transform));
            TransformGroup group;
            var renderTransform = RenderTransform;
            if (renderTransform == null || 
                Equals(renderTransform, Transform.Identity))
                RenderTransform = group = new TransformGroup();
            else if ((group = renderTransform as TransformGroup) == null)
                throw new InvalidOperationException(
                    "Expected RenderTransform to be TransformGroup");
            var index = group.Children.Count;
            group.Children.Add(transform);
            return dep => new PropertyPath(
                $"RenderTransform.Children[{index}].{dep.Name}");
        }

        public ViewController CreateImageView(BitmapSource source)
        {
            var image = new Image { Source = source };
            return new ViewController(_region, image);
        }

        public BitmapSource CreateSnapshot(
            double dpiX = 96.0, double dpiY = 96.0)
        {
            var bounds = new Rect(_region.RenderSize);

            if (!IsMeasureValid || !IsArrangeValid)
            {
                Measure(bounds.Size);
                Arrange(bounds);
                UpdateLayout();
                ApplyTemplate();
            }

            var snapshot = new RenderTargetBitmap(
                (int) (bounds.Width * dpiX / 96.0),
                (int) (bounds.Height * dpiY / 96.0),
                dpiX, dpiY, PixelFormats.Pbgra32);

            var visualBrush   = new VisualBrush(this);
            var drawingVisual = new DrawingVisual();

            using (var drawingContext = drawingVisual.RenderOpen())
            {
                drawingContext.DrawRectangle(visualBrush, null, bounds);
            }

            snapshot.Render(drawingVisual);
            return snapshot;
        }
    }
}
