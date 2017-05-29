namespace Miruken.Mvc.Console.Test
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class BufferReadLineTest
    {
        private MockConsole console;
        private Buffer      buffer;

        [TestInitialize]
        public void TestInitialize()
        {
            console = new MockConsole();
            buffer = new Buffer
            {
                Console        = console,
                ContentBoundry = new Boundry(new Point(2,2), new Point(8,8)),
                NextContent    = new Point(2,2),
                Border         = new Thickness(1),
                Padding        = new Thickness(1)
            };
        }

        [TestMethod]
        public void PressingEnterReturnsValue()
        {
            console.ReadKeys = new[] { KeyInfo.Enter };
            var target = buffer.EditInPlace("abc");
            Assert.AreEqual("abc", target);
        }

        [TestMethod]
        public void BackspaceRemovesCharacter()
        {
            console.ReadKeys = new[] { KeyInfo.Backspace, KeyInfo.Enter };
            var target = buffer.EditInPlace("abc");
            Assert.AreEqual("ab", target);
        }

        [TestMethod]
        public void BackspaceWillNotGoPastBegining()
        {
            console.ReadKeys = new[]
            {
                KeyInfo.Backspace,
                KeyInfo.Backspace,
                KeyInfo.Backspace,
                KeyInfo.Backspace,
                KeyInfo.Enter
            };
            var target = buffer.EditInPlace("abc");
            Assert.AreEqual("", target);
            Assert.AreEqual(2,  console.CursorLeft);
        }

        [TestMethod]
        public void LeftArrowMovesCursor()
        {
            console.ReadKeys = new[] { KeyInfo.LeftArrow, KeyInfo.Enter };
            buffer.EditInPlace("abc");
            Assert.AreEqual(4, console.CursorLeft);
        }

        [TestMethod]
        public void LeftArrowCursorToBegining()
        {
            console.ReadKeys = new[]
            {
                KeyInfo.LeftArrow,
                KeyInfo.LeftArrow,
                KeyInfo.LeftArrow,
                KeyInfo.Enter
            };
            buffer.EditInPlace("abc");
            Assert.AreEqual(2, console.CursorLeft);
        }

        [TestMethod]
        public void LeftArrowWillNotGoPastBegining()
        {
            console.ReadKeys = new[]
            {
                KeyInfo.LeftArrow,
                KeyInfo.LeftArrow,
                KeyInfo.LeftArrow,
                KeyInfo.LeftArrow,
                KeyInfo.Enter
            };
            buffer.EditInPlace("abc");
            Assert.AreEqual(2, console.CursorLeft);
        }

        [TestMethod]
        public void RightArrowCursorToEnd()
        {
            console.ReadKeys = new[]
            {
                KeyInfo.RightArrow,
                KeyInfo.RightArrow,
                KeyInfo.RightArrow,
                KeyInfo.Enter
            };
            buffer.EditInPlace("abc");
            Assert.AreEqual(8, console.CursorLeft);
        }

        [TestMethod]
        public void RightArrowWillNotGoPastEnd()
        {
            console.ReadKeys = new[]
            {
                KeyInfo.RightArrow,
                KeyInfo.RightArrow,
                KeyInfo.RightArrow,
                KeyInfo.RightArrow,
                KeyInfo.Enter
            };
            buffer.EditInPlace("abc");
            Assert.AreEqual(8, console.CursorLeft);
        }

        [TestMethod]
        public void AddCharacterToEnd()
        {
            console.ReadKeys = new[]
            {
                KeyInfo.D,
                KeyInfo.Enter
            };
            var target = buffer.EditInPlace("abc");
            Assert.AreEqual("abcd", target);
        }

        [TestMethod]
        public void AddTwoCharactersToEnd()
        {
            console.ReadKeys = new[]
            {
                KeyInfo.D,
                KeyInfo.E,
                KeyInfo.Enter
            };
            var target = buffer.EditInPlace("abc");
            Assert.AreEqual("abcde", target);
        }

        [TestMethod]
        public void AddThreeCharactersToEnd()
        {
            console.ReadKeys = new[]
            {
                KeyInfo.D,
                KeyInfo.E,
                KeyInfo.F,
                KeyInfo.Enter
            };
            var target = buffer.EditInPlace("abc");
            Assert.AreEqual("abcdef", target);
        }

        [TestMethod]
        public void WillNotAddCharactersPastTheEndOfContent()
        {
            console.ReadKeys = new[]
            {
                KeyInfo.D,
                KeyInfo.E,
                KeyInfo.F,
                KeyInfo.G,
                KeyInfo.Enter
            };
            var target = buffer.EditInPlace("abc");
            Assert.AreEqual("abcdef", target);
        }

        [TestMethod]
        public void ChangingCursorPositionWillStillNotAddCharactersPastTheEndOfContent()
        {
            console.ReadKeys = new[]
            {
                KeyInfo.LeftArrow,
                KeyInfo.D,
                KeyInfo.E,
                KeyInfo.F,
                KeyInfo.G,
                KeyInfo.Enter
            };
            var target = buffer.EditInPlace("abc");
            Assert.AreEqual("abdefc", target);
            Assert.AreEqual(console.CursorLeft, 7);
        }

        [TestMethod]
        public void PressingRightArrowsAndThenAddingCharacter()
        {
            console.ReadKeys = new[]
            {
                KeyInfo.RightArrow,
                KeyInfo.RightArrow,
                KeyInfo.RightArrow,
                KeyInfo.RightArrow,
                KeyInfo.D,
                KeyInfo.Enter
            };
            var target = buffer.EditInPlace("a");
            Assert.AreEqual("a    d", target);
        }
    }
}
