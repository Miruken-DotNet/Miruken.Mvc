namespace Miruken.Mvc.Console.Test
{
    using Console;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ElementTests : TestBase
    {
        private const string alphaString    = "aaaaaa";

        [TestMethod]
        public void CreatesBorder()
        {
            var buffer = new Buffer
            {
                Border = new Thickness(1)
            }
            .WriteLine(alphaString)
            .WriteLine(alphaString)
            .WriteLine(alphaString);

            var cells = Render(new Size(3, 3), buffer);

            char[][] expected =
            {
                new [] {'-', '-', '-'},
                new [] {'|', 'a', '|'},
                new [] {'-', '-', '-'},
            };

            AssertCellsAreEquivelant(expected, cells);
        }

        [TestMethod]
        public void CreatesBorderLeft()
        {
            var buffer = new Buffer
            {
                Border = new Thickness(1, 0, 0, 0)
            }
            .WriteLine(alphaString)
            .WriteLine(alphaString)
            .WriteLine(alphaString);

            var cells = Render(new Size(3, 3), buffer);

            char[][] expected =
            {
                new [] {'|', 'a', 'a'},
                new [] {'|', 'a', 'a'},
                new [] {'|', 'a', 'a'},
            };

            AssertCellsAreEquivelant(expected, cells);
        }

        [TestMethod]
        public void CreatesBorderRight()
        {
            var buffer = new Buffer
            {
                Border = new Thickness(0, 0, 1, 0)
            }
            .WriteLine(alphaString)
            .WriteLine(alphaString)
            .WriteLine(alphaString);

            var cells = Render(new Size(3, 3), buffer);

            char[][] expected =
            {
                new [] {'a', 'a', '|'},
                new [] {'a', 'a', '|'},
                new [] {'a', 'a', '|'},
            };

            AssertCellsAreEquivelant(expected, cells);
        }

        [TestMethod]
        public void CreatesBorderTop()
        {
            var buffer = new Buffer
            {
                Border = new Thickness(0, 1, 0, 0)
            }
            .WriteLine(alphaString)
            .WriteLine(alphaString)
            .WriteLine(alphaString);

            var cells = Render(new Size(3, 3), buffer);

            char[][] expected =
            {
                new [] {'-', '-', '-'},
                new [] {'a', 'a', 'a'},
                new [] {'a', 'a', 'a'},
            };

            AssertCellsAreEquivelant(expected, cells);
        }

        [TestMethod]
        public void CreatesBorderBottom()
        {
            var buffer = new Buffer
            {
                Border = new Thickness(0, 0, 0, 1)
            }
            .WriteLine(alphaString)
            .WriteLine(alphaString)
            .WriteLine(alphaString);

            var cells = Render(new Size(3, 3), buffer);

            char[][] expected =
            {
                new [] {'a', 'a', 'a'},
                new [] {'a', 'a', 'a'},
                new [] {'-', '-', '-'}
            };

            AssertCellsAreEquivelant(expected, cells);
        }

        [TestMethod]
        public void CreatesLeftPadding()
        {
            var buffer = new Buffer
            {
                Padding = new Thickness(1, 0, 0, 0)
            }
            .WriteLine(alphaString)
            .WriteLine(alphaString)
            .WriteLine(alphaString);

            var cells = Render(new Size(3, 3), buffer);
            char[][] expected =
            {
                new [] {' ', 'a', 'a'},
                new [] {' ', 'a', 'a'},
                new [] {' ', 'a', 'a'}
            };

            AssertCellsAreEquivelant(expected, cells);
        }

        [TestMethod]
        public void CreatesRightPadding()
        {
            var buffer = new Buffer
            {
                Padding = new Thickness(0, 0, 1, 0)
            }
            .WriteLine(alphaString)
            .WriteLine(alphaString)
            .WriteLine(alphaString);

            var cells = Render(new Size(3, 3), buffer);

            char[][] expected =
            {
                new [] {'a', 'a', ' '},
                new [] {'a', 'a', ' '},
                new [] {'a', 'a', ' '}
            };

            AssertCellsAreEquivelant(expected, cells);
        }

        [TestMethod]
        public void CreatesTopPadding()
        {
            var buffer = new Buffer
            {
                Padding = new Thickness(0, 1, 0, 0)
            }
            .WriteLine(alphaString)
            .WriteLine(alphaString)
            .WriteLine(alphaString);

            var cells = Render(new Size(3, 3), buffer);

            char[][] expected =
            {
                new [] {' ', ' ', ' '},
                new [] {'a', 'a', 'a'},
                new [] {'a', 'a', 'a'}
            };

            AssertCellsAreEquivelant(expected, cells);
        }

        [TestMethod]
        public void CreatesBottomPadding()
        {
            var buffer = new Buffer
            {
                Padding = new Thickness(0, 0, 0, 1)
            }
            .WriteLine(alphaString)
            .WriteLine(alphaString)
            .WriteLine(alphaString);

            var cells = Render(new Size(3, 3), buffer);

            char[][] expected =
            {
                new [] {'a', 'a', 'a'},
                new [] {'a', 'a', 'a'},
                new [] {' ', ' ', ' '}
            };

            AssertCellsAreEquivelant(expected, cells);
        }

        [TestMethod]
        public void CreatesContent()
        {
            var buffer = new Buffer
            {
                Border = new Thickness(1)
            }
            .WriteLine("abcd")
            .WriteLine("efgh");

            var cells = Render(new Size(4, 4), buffer);

            char[][] expected =
            {
                new [] {'-', '-', '-', '-'},
                new [] {'|', 'a', 'b', '|'},
                new [] {'|', 'e', 'f', '|'},
                new [] {'-', '-', '-', '-'}
            };

            AssertCellsAreEquivelant(expected, cells);
        }

        [TestMethod]
        public void CreatesMarginLeft()
        {
            var buffer = new Buffer
            {
                Border = new Thickness(1, 0, 0, 0),
                Margin = new Thickness(1, 0, 0, 0)
            };

            var cells = Render(new Size(4, 4), buffer);

            char[][] expected =
            {
                new[] {' ', '|', ' ', ' '},
                new[] {' ', '|', ' ', ' '},
                new[] {' ', '|', ' ', ' '},
                new[] {' ', '|', ' ', ' '}
            };

            AssertCellsAreEquivelant(expected, cells);
        }

        [TestMethod]
        public void CreatesMarginRight()
        {
            var buffer = new Buffer
            {
                Border = new Thickness(0, 0, 1, 0),
                Margin = new Thickness(0, 0, 1, 0)
            };

            var cells = Render(new Size(4, 4), buffer);

            char[][] expected =
            {
                new[] {' ', ' ', '|', ' '},
                new[] {' ', ' ', '|', ' '},
                new[] {' ', ' ', '|', ' '},
                new[] {' ', ' ', '|', ' '}
            };

            AssertCellsAreEquivelant(expected, cells);
        }

        [TestMethod]
        public void CreatesMarginTop()
        {
            var buffer = new Buffer
            {
                Border = new Thickness(0, 1, 0, 0),
                Margin = new Thickness(0, 1, 0, 0)
            };

            var cells = Render(new Size(4, 4), buffer);

            char[][] expected =
            {
                new[] {' ', ' ', ' ', ' '},
                new[] {'-', '-', '-', '-'},
                new[] {' ', ' ', ' ', ' '},
                new[] {' ', ' ', ' ', ' '}
            };

            AssertCellsAreEquivelant(expected, cells);
        }

        [TestMethod]
        public void CreatesMarginBottom()
        {
            var buffer = new Buffer
            {
                Border = new Thickness(0, 0, 0, 1),
                Margin = new Thickness(0, 0, 0, 1)
            };

            var cells = Render(new Size(4, 4), buffer);

            char[][] expected =
            {
                new[] {' ', ' ', ' ', ' '},
                new[] {' ', ' ', ' ', ' '},
                new[] {'-', '-', '-', '-'},
                new[] {' ', ' ', ' ', ' '}
            };

            AssertCellsAreEquivelant(expected, cells);
        }

        [TestMethod]
        public void MultipleBorders()
        {
            var buffer = new Buffer
                {
                    Border = new Thickness(2)
                }
                .WriteLine("ab")
                .WriteLine("cd");

            var cells = Render(new Size(6, 6), buffer);

            char[][] expected =
            {
                new[] {'-', '-', '-', '-', '-', '-'},
                new[] {'-', '-', '-', '-', '-', '-'},
                new[] {'|', '|', 'a', 'b', '|', '|'},
                new[] {'|', '|', 'c', 'd', '|', '|'},
                new[] {'-', '-', '-', '-', '-', '-'},
                new[] {'-', '-', '-', '-', '-', '-'}
            };

            AssertCellsAreEquivelant(expected, cells);
        }

        [TestMethod]
        public void SingleBorderMultiplePadding()
        {
            var buffer = new Buffer
                {
                    Border  = new Thickness(1),
                    Padding = new Thickness(2)
                }
                .WriteLine("ab")
                .WriteLine("cd");

            var cells = Render(new Size(8, 8), buffer);

            char[][] expected =
            {
                new[] {'-', '-', '-', '-', '-', '-', '-', '-'},
                new[] {'|', ' ', ' ', ' ', ' ', ' ', ' ', '|'},
                new[] {'|', ' ', ' ', ' ', ' ', ' ', ' ', '|'},
                new[] {'|', ' ', ' ', 'a', 'b', ' ', ' ', '|'},
                new[] {'|', ' ', ' ', 'c', 'd', ' ', ' ', '|'},
                new[] {'|', ' ', ' ', ' ', ' ', ' ', ' ', '|'},
                new[] {'|', ' ', ' ', ' ', ' ', ' ', ' ', '|'},
                new[] {'-', '-', '-', '-', '-', '-', '-', '-'}
            };

            AssertCellsAreEquivelant(expected, cells);
        }

        [TestMethod]
        public void MultipleMarginBorderPadding()
        {
            var buffer = new Buffer
                {
                    Margin  = new Thickness(2),
                    Border  = new Thickness(2),
                    Padding = new Thickness(2)
                }
                .WriteLine("ab")
                .WriteLine("cd");

            var cells = Render(new Size(14, 14), buffer);

            char[][] expected =
            {
                new[] {' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' '},
                new[] {' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' '},
                new[] {' ', ' ', '-', '-', '-', '-', '-', '-', '-', '-', '-', '-', ' ', ' '},
                new[] {' ', ' ', '-', '-', '-', '-', '-', '-', '-', '-', '-', '-', ' ', ' '},
                new[] {' ', ' ', '|', '|', ' ', ' ', ' ', ' ', ' ', ' ', '|', '|', ' ', ' '},
                new[] {' ', ' ', '|', '|', ' ', ' ', ' ', ' ', ' ', ' ', '|', '|', ' ', ' '},
                new[] {' ', ' ', '|', '|', ' ', ' ', 'a', 'b', ' ', ' ', '|', '|', ' ', ' '},
                new[] {' ', ' ', '|', '|', ' ', ' ', 'c', 'd', ' ', ' ', '|', '|', ' ', ' '},
                new[] {' ', ' ', '|', '|', ' ', ' ', ' ', ' ', ' ', ' ', '|', '|', ' ', ' '},
                new[] {' ', ' ', '|', '|', ' ', ' ', ' ', ' ', ' ', ' ', '|', '|', ' ', ' '},
                new[] {' ', ' ', '-', '-', '-', '-', '-', '-', '-', '-', '-', '-', ' ', ' '},
                new[] {' ', ' ', '-', '-', '-', '-', '-', '-', '-', '-', '-', '-', ' ', ' '},
                new[] {' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' '},
                new[] {' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' '}
            };

            AssertCellsAreEquivelant(expected, cells);
        }
    }
}
