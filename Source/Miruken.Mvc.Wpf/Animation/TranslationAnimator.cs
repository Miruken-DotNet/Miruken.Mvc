namespace Miruken.Mvc.Wpf.Animation
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using Concurrency;
    using Mvc.Animation;

    public class TranslationAnimator : IAnimator
    {
        public TranslationAnimator(Translation translation)
        {
            Translation = translation;
        }

        public Translation Translation { get; }

        public Promise Animate(
            ViewRegion region, 
            FrameworkElement oldView, 
            FrameworkElement newView)
        {
            var children  = region.Children;
            var fromIndex = children.IndexOf(oldView);
            if (fromIndex >= 0)
            {
                var from = CreateWrapper(region, fromIndex);
            }
            var to = CreateWrapper(region, fromIndex);
            to.Content = newView;
            return Promise.Empty;
        }

        private Brush GetVisualBrush(Visual visual)
        {
            var brush = new VisualBrush(visual)
            {
                ViewportUnits = BrushMappingMode.Absolute
            };
            RenderOptions.SetCachingHint(brush, CachingHint.Cache);
            RenderOptions.SetCacheInvalidationThresholdMinimum(brush, 40);
            return brush;
        }

        private static ContentControl CreateWrapper(
            Panel container, int index)
        {
            var control = new ContentControl
            {
                RenderTransform = new TranslateTransform(0, 0)
            };
            if (index >= 0)
                container.Children.Insert(index + 1, control);
            else
                container.Children.Add(control);
            return control;
        }
    }
}
