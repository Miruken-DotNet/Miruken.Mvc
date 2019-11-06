namespace Miruken.Mvc.Console.Test
{
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    public class TestBase
    {
        public Cells Render(Size size, FrameworkElement element)
        {
            var cells = new Cells(size.Height, size.Width, '*');
            element.Measure(size);
            element.Arrange(new Rectangle(new Point(0,0), size));
            element.Render(cells);

            Console.WriteLine(cells.ToString());
            return cells;
        }

        public void AssertCellsAreEquivelant(char[][] expected, Cells cells)
        {
            Assert.AreEqual(expected.Length, cells.Height);
            for (var i = 0; i < expected.Length; i++)
            {
                CollectionAssert.AreEqual(expected[i], cells[i]);
            }
        }
    }
}