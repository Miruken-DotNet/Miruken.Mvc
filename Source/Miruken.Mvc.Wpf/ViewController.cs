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

        public double ExpectedWidth => _region.ActualWidth;

        public void AddToParent()
        {
            _region.Children.Add(this);
        }

        public void AddToParentAtIndex(int index)
        {
            _region.Children.Insert(index, this);
        }

        public bool AddToParentAfter(ViewController view)
        {
            if (view == null)
            {
                _region.Children.Add(this);
                return true;
            }
            var children = _region.Children;
            var index    = children.IndexOf(view);
            if (index < 0) return false;
            _region.Children.Insert(index + 1, this);
            return true;
        }

        public void RemoveFromParent()
        {
            var children = _region.Children;
            if (children.Contains(this))
                children.Remove(this);
        }
    }
}
