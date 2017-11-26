namespace Miruken.Mvc.Wpf
{
    using System.Windows;
    using System.Windows.Controls;

    public class ViewController : ContentControl
    {
        public ViewController(FrameworkElement view)
        {
            Content = view;
        }
    }
}
