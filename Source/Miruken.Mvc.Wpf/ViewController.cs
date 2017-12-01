namespace Miruken.Mvc.Wpf
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;

    public class ViewController : ContentControl
    {
        private readonly ViewRegion _region;

        public ViewController(ViewRegion region, object view)
        {
            _region = region;
            Content = view;
        }

        public double RegionWidth => _region.ActualWidth;
        public double RegionHeight => _region.ActualHeight;

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
            if (children.Contains(this))
                return false;
            children.Insert(index, this);
            return true;
        }

        public bool AddViewAbove(ViewController view)
        {
            var children = _region.Children;
            if (children.Contains(this))
                return false;
            if (view == null)
            {
                children.Add(this);
                return true;
            }
            var index = children.IndexOf(view);
            if (index < 0) return false;
            children.Insert(index + 1, this);
            return true;
        }

        public bool AddViewBelow(ViewController view)
        {
            var children = _region.Children;
            if (children.Contains(this))
                return false;
            if (view == null)
            {
                children.Add(this);
                return true;
            }
            var index = children.IndexOf(view);
            if (index < 0) return false;
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
    }
}
