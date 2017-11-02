namespace WpfTestApp.Features.Settings
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using Miruken.Mvc.Wpf;

    public partial class Settings : View
    {
        private readonly Random _random = new Random();

        public Settings()
        {
            InitializeComponent();
        }

        private void Add1_Click(object sender, RoutedEventArgs e)
        {
            var child = new Label
            {
                Background = new SolidColorBrush(ColorChoices[_random.Next(3)]),
                Content    = "Hello",
                Margin     = new Thickness(10, 10, 10, 0)
            };

            Container.Children.Add(child);

            // Create a DoubleAnimation to animate the width of the button.
            var doubleAnimation = new DoubleAnimation
            {
                From           = 0,
                To             = 100,
                Duration       = TimeSpan.FromMilliseconds(200),
                EasingFunction = new CubicEase(),
                //FillBehavior =  FillBehavior.Stop
            };
            doubleAnimation.Completed += (o, args) =>
            {
                var finalHeight = child.Height;
                child.BeginAnimation(HeightProperty, null);
                child.Height = finalHeight;
                //child.Height = 100;
            };

            // Configure the animation to target the button's Height property.
            Storyboard.SetTargetProperty(doubleAnimation, new PropertyPath(HeightProperty));

            // **Set the Target Object for the Animation.**
            Storyboard.SetTarget(doubleAnimation, child);

            // Create a storyboard to contain the animation.
            var storyboard = new Storyboard();
            storyboard.Children.Add(doubleAnimation);
            storyboard.Begin(child);
        }

        private void Remove_Click(object sender, RoutedEventArgs e)
        {
            var children = Container.Children;
            if (children.Count == 0) return;
            var child = (FrameworkElement)children[children.Count - 1];

            // Create a DoubleAnimation to animate the width of the button.
            var doubleAnimation = new DoubleAnimation
            {
                To             = 0,
                Duration       = TimeSpan.FromMilliseconds(200),
                EasingFunction = new CubicEase()
            };
            doubleAnimation.Completed += (o, args) => children.Remove(child);

            // Configure the animation to target the button's Height property.
            Storyboard.SetTargetProperty(doubleAnimation, new PropertyPath(HeightProperty));

            // **Set the Target Object for the Animation.**
            Storyboard.SetTarget(doubleAnimation, child);

            // Create a storyboard to contain the animation.
            var storyboard = new Storyboard();
            storyboard.Children.Add(doubleAnimation);
            storyboard.Begin(child);
        }


        private void Add_Click(object sender, RoutedEventArgs e)
        {
            var child = new Border
            {
                Height          = ActualHeight,
                Width           = ActualWidth,
                Background      = new SolidColorBrush(ColorChoices[_random.Next(3)]),
                RenderTransform = new TranslateTransform()
            };

            var children = Container.Children;
            var current  = children.Count > 0 ? children[0] : null;
            children.Add(child);

            var doubleAnimation = new DoubleAnimation
            {
                To       = 0,
                From     = -ActualHeight,
                Duration = TimeSpan.FromMilliseconds(200)
            };
            doubleAnimation.Completed += (o, args) =>
            {
                if (current != null)
                    children.Remove(current);
                child.RenderTransform.BeginAnimation(TranslateTransform.YProperty, null);
            };
            child.RenderTransform.BeginAnimation(TranslateTransform.YProperty, doubleAnimation);
        }

        private static readonly Color[] ColorChoices =
        {
            Colors.Red, Colors.DeepSkyBlue, Colors.Orange
        };        
    }
}
