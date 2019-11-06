namespace Miruken.Mvc.Console.Test
{
    using System;
    using Console;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Buffer = Buffer;

    [TestClass]
    public class BufferWriteTests : TestBase
    {
        [TestMethod]
        public void WriteLine()
        {
            var cells = Render(new Size(2, 2), new Buffer()
                .WriteLine("abcd")
                .WriteLine("efgh")
                .WriteLine("ijkl"));

            char[][] expected =
            {
                new [] {'a', 'b'},
                new [] {'e', 'f'}
            };

            AssertCellsAreEquivelant(expected, cells);
        }

        [TestMethod]
        public void WriteLineRespectsNewLineCharacters()
        {
            var cells = Render(new Size(2, 2), new Buffer()
                .WriteLine("a" + Environment.NewLine + "b" + Environment.NewLine + "c"));

            char[][] expected =
            {
                new [] {'a', ' '},
                new [] {'b', ' '}
            };

            AssertCellsAreEquivelant(expected, cells);
        }

        [TestMethod]
        public void Wrap()
        {
            var cells = Render(new Size(2, 2), new Buffer()
                .Wrap("abcdefg"));

            char[][] expected =
            {
                new [] {'a', 'b'},
                new [] {'c', 'd'}
            };

            AssertCellsAreEquivelant(expected, cells);
        }

        [TestMethod]
        public void WrapEndsWithANewLine()
        {
            var cells = Render(new Size(2, 2), new Buffer()
                .Wrap("a")
                .Wrap("b"));

            char[][] expected =
            {
                new [] {'a', ' '},
                new [] {'b', ' '}
            };

            AssertCellsAreEquivelant(expected, cells);
        }

        [TestMethod]
        public void WrapRespectsNewLineCharacters()
        {
            var cells = Render(new Size(2, 3), new Buffer()
                .Wrap("abc" + Environment.NewLine + "defg"));

            char[][] expected =
            {
                new [] {'a', 'b'},
                new [] {'c', ' '},
                new [] {'d', 'e'}
            };

            AssertCellsAreEquivelant(expected, cells);
        }

        [TestMethod]
        public void WrapRespectsTabCharacters()
        {
            var cells = Render(new Size(12, 3), new Buffer()
                .Wrap("1234567890")
                .Wrap("\ta"));

            char[][] expected =
            {
                new [] {'1', '2', '3', '4', '5', '6', '7', '8', '9', '0', ' ', ' '},
                new [] {' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', 'a', ' ', ' ', ' '},
                new [] {' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' '}
            };

            AssertCellsAreEquivelant(expected, cells);
        }

        [TestMethod]
        public void Write()
        {
            var cells = Render(new Size(2, 2), new Buffer()
                .Write("ab"));

            char[][] expected =
            {
                new [] {'a', 'b'},
                new [] {' ', ' '}
            };

            AssertCellsAreEquivelant(expected, cells);
        }

        [TestMethod]
        public void WriteRespectsNewLineCharacters()
        {
            var cells = Render(new Size(3, 3), new Buffer()
                .Write("ab" + Environment.NewLine + "cd"));

            char[][] expected =
            {
                new [] {'a', 'b', ' '},
                new [] {'c', 'd', ' '},
                new [] {' ', ' ', ' '}
            };

            AssertCellsAreEquivelant(expected, cells);
        }

        [TestMethod]
        public void WriteLineRespectsTabCharacters()
        {
            var cells = Render(new Size(20, 3), new Buffer()
                .WriteLine("12345678901234567890")
                .WriteLine("\ta\ta"));

            char[][] expected =
            {
                new [] {'1', '2', '3', '4', '5', '6', '7', '8', '9', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '0'},
                new [] {' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', 'a', ' ', ' ', ' ', ' ', ' ', ' ', ' ', 'a', ' ', ' ', ' '},
                new [] {' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' '}
            };

            AssertCellsAreEquivelant(expected, cells);
        }

        [TestMethod]
        public void TabConsidersExistingPosition()
        {
            var cells = Render(new Size(20, 3), new Buffer()
                .WriteLine("12345678901234567890")
                .Write("bcdef")
                .Write("\ta")
                .Write("ghi")
                .Write("\ta"));

            char[][] expected =
            {
                new [] {'1', '2', '3', '4', '5', '6', '7', '8', '9', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '0'},
                new [] {'b', 'c', 'd', 'e', 'f', ' ', ' ', ' ', 'a', 'g', 'h', 'i', ' ', ' ', ' ', ' ', 'a', ' ', ' ', ' '},
                new [] {' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' '}
            };

            AssertCellsAreEquivelant(expected, cells);
        }

        [TestMethod]
        public void TabAlwaysHasAtleastOneSpaceBetweenCharactersWithTabSpacesMinusOne()
        {
            var cells = Render(new Size(20, 3), new Buffer()
                .WriteLine("12345678901234567890")
                .Write("bcdefgh")
                .Write("\ta"));

            char[][] expected =
            {
                new [] {'1', '2', '3', '4', '5', '6', '7', '8', '9', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '0'},
                new [] {'b', 'c', 'd', 'e', 'f', 'g', 'h', ' ', 'a', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' '},
                new [] {' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' '}
            };

            AssertCellsAreEquivelant(expected, cells);
        }

        [TestMethod]
        public void TabAlwaysHasAtleastOneSpaceBetweenCharacters()
        {
            var cells = Render(new Size(20, 3), new Buffer()
                .WriteLine("12345678901234567890")
                .Write("bcdefghi")
                .Write("\ta"));

            char[][] expected =
            {
                new [] {'1', '2', '3', '4', '5', '6', '7', '8', '9', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '0'},
                new [] {'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', 'a', ' ', ' ', ' '},
                new [] {' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' '}
            };

            AssertCellsAreEquivelant(expected, cells);
        }

        [TestMethod]
        public void WriteRespectsTabCharacters()
        {
            var cells = Render(new Size(20, 3), new Buffer()
                .WriteLine("12345678901234567890")
                .WriteLine("\ta\ta"));

            char[][] expected =
            {
                new [] {'1', '2', '3', '4', '5', '6', '7', '8', '9', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '0'},
                new [] {' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', 'a', ' ', ' ', ' ', ' ', ' ', ' ', ' ', 'a', ' ', ' ', ' '},
                new [] {' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' '}
            };

            AssertCellsAreEquivelant(expected, cells);
        }

        [TestMethod]
        public void WriteWrapsTabCharactersToTheNextLine()
        {
            var cells = Render(new Size(20, 3), new Buffer()
                .WriteLine("12345678901234567890")
                .WriteLine("\ta\ta\ta"));

            char[][] expected =
            {
                new [] {'1', '2', '3', '4', '5', '6', '7', '8', '9', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '0'},
                new [] {' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', 'a', ' ', ' ', ' ', ' ', ' ', ' ', ' ', 'a', ' ', ' ', ' '},
                new [] {'a', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' '}
            };

            AssertCellsAreEquivelant(expected, cells);
        }
    }
}
