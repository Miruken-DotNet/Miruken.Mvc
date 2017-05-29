namespace Miruken.Mvc.Console
{

    public abstract class Panel: FrameworkElement
    {
        public  abstract FrameworkElement[] Children { get; }

        public abstract void RemoveLast();

        public override void Render(Cells cells)
        {
            base.Render(cells);
            foreach (var child in Children)
            {
                child.Render(cells);
            }
        }
    }
}
