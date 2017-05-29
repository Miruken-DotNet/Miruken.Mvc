namespace Miruken.Mvc.Console
{
    using System;

    public class ContentControl: FrameworkElement
    {
        public FrameworkElement Content { get; set; }

        public override void Measure(Size availableSize)
        {
            base.Measure(availableSize);
            Content.Measure(availableSize);
        }

        public override void Arrange(Rectangle rectangle)
        {
            base.Arrange(rectangle);
            Content.Arrange(rectangle);
        }

        public override void Render(Cells cells)
        {
            base.Render(cells);
            Content.Render(cells);
        }

        public override void KeyPressed(ConsoleKeyInfo keyInfo)
        {
            base.KeyPressed(keyInfo);
            Content.KeyPressed(keyInfo);
        }
    }
}