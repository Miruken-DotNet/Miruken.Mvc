namespace Miruken.Mvc.Wpf.TestApp.Features.HelloWorld
{
    using System;
    using System.IO;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;

    /// <summary>
    /// Interaction logic for HelloWorld.xaml
    /// </summary>
    public partial class HelloWorld : View
    {
        public HelloWorld()
        {
            InitializeComponent();
        }

        public void button_Capture_Click(object sender, RoutedEventArgs e)
        {
            var bounds = VisualTreeHelper.GetDescendantBounds(this);
            var region = new Int32Rect(0, 0,
                (int)bounds.Size.Width / 2, (int)bounds.Size.Height);
            Capture(this, new Uri(@"c:\temp\screenshot.png"), region);
        }

        public void Capture(Visual visual, Uri destination, Int32Rect? bounds = null)
        {
            const double dpiX = 96.0, dpiY = 96.0;

            try
            {
                var snapshot = Snapshot(visual, dpiX, dpiY);

                if (bounds != null)
                    snapshot = new CroppedBitmap(snapshot, bounds.Value);

                //PNG encoder for creating PNG file
                var encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(snapshot));
                using (var stream = new FileStream(
                    destination.LocalPath, FileMode.Create, FileAccess.Write))
                {
                    encoder.Save(stream);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }

        public BitmapSource Snapshot(
            Visual visual, double dpiX, double dpiY)
        {
            var bounds = VisualTreeHelper.GetDescendantBounds(this);

            //Specification for target bitmap like width/height pixel etc.
            var snapshot = new RenderTargetBitmap(
                (int)(bounds.Width * dpiX / 96.0),
                (int)(bounds.Height * dpiY / 96.0),
                dpiX, dpiY, PixelFormats.Pbgra32);
            //creates Visual Brush of UIElement
            var visualBrush = new VisualBrush(visual);
            var drawingVisual = new DrawingVisual();
            using (var drawingContext = drawingVisual.RenderOpen())
            {
                //draws image of element
                drawingContext.DrawRectangle(
                    visualBrush, null,
                    new Rect(new Point(0, 0), bounds.Size));
            }
            //renders image
            snapshot.Render(drawingVisual);
            return snapshot;
        }
    }
}
