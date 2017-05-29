namespace MajorLeagueMiruken.Console.Features.Layout
{
    using Miruken.Mvc.Console;

    public class LayoutView : View
    {
        public ViewRegion Header { get; set; }
        public ViewRegion Body   { get; set; }

        public LayoutView()
        {
            var dock = new DockPanel();
            Content = dock;

            Header = new ViewRegion
            {
                Border = new Thickness(1),
                Padding = new Thickness(2, 1)
            };
            dock.Add(Header, Dock.Top, 15);

            Body = new ViewRegion
            {
                Padding = new Thickness(2, 1)
            };
            dock.Add(Body, Dock.Fill);
        }
    }
}
