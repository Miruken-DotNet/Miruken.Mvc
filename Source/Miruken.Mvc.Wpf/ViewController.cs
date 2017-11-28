namespace Miruken.Mvc.Wpf
{
    using System.Windows;
    using System.Windows.Controls;

    public class ViewController : ContentControl
    {
        private readonly ViewRegion _region;

        public ViewController(ViewRegion region, FrameworkElement view)
        {
            _region = region;
            Content = view;
        }

        public double RegionWidth => _region.ActualWidth;
        public double RegionHeight => _region.ActualHeight;

        public void AddView()
        {
            _region.Children.Add(this);
        }

        public void AddViewAtIndex(int index)
        {
            _region.Children.Insert(index, this);
        }

        public bool AddViewAfter(ViewController view)
        {
            if (view == null)
            {
                _region.Children.Add(this);
                return true;
            }
            var children = _region.Children;
            var index    = children.IndexOf(view);
            if (index < 0) return false;
            children.Insert(index + 1, this);
            return true;
        }

        public bool AddViewBefore(ViewController view)
        {
            if (view == null)
            {
                _region.Children.Add(this);
                return true;
            }
            var children = _region.Children;
            var index    = children.IndexOf(view);
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
    }
}
