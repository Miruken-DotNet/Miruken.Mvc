namespace Miruken.Mvc.Console.Test
{
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Buffer = Buffer;

    [TestClass]
    public class BufferEditTest
    {
        private MockConsole console;
        private Buffer      buffer;

        [TestInitialize]
        public void TestInitialize()
        {
            console = new MockConsole();
        }

        [TestMethod]
        public void ContentWidthGreaterThan3ButTooSmallForPromtAndValue()
        {
            CreateBuffer(4);
            console.ReadKeys = new[]
            {
                KeyInfo.Enter
            };
            var target = buffer.Edit("xyz", "abc");
            Assert.AreEqual("abc", target);
            AssertConsoleText("...");
        }

        [TestMethod]
        public void ContentWidthExactly3()
        {
            CreateBuffer(3);
            console.ReadKeys = new[]
            {
                KeyInfo.Enter
            };
            var target = buffer.Edit("xyz", "abc");
            Assert.AreEqual("abc", target);
            AssertConsoleText("...");
        }

        [TestMethod]
        public void ContentWidthLessThan3()
        {
            CreateBuffer(2);
            console.ReadKeys = new[]
            {
                KeyInfo.Enter
            };
            var target = buffer.Edit("xyz", "abc");
            Assert.AreEqual("abc", target);
            AssertConsoleText("");
        }

        [TestMethod]
        public void ContentWidthWideEnoughForPromptAndContent()
        {
            CreateBuffer(9);
            console.ReadKeys = new[]
            {
                KeyInfo.Enter
            };
            var target = buffer.Edit("xyz", "abc");
            Assert.AreEqual("abc", target);
            AssertConsoleText("? xyz abc");
        }

        private void AssertConsoleText(string expected)
        {
            var target = console.Builder.ToString().Trim();
            Console.WriteLine($"console: [{target}]");
            Assert.AreEqual(expected, target);
        }

        private void CreateBuffer(int contentWidth)
        {
            var start = new Point(2, 2);
            var end = new Point(2 + contentWidth, 2);
            buffer = new Buffer
            {
                Console        = console,
                ContentBoundry = new Boundry(start, end),
                NextContent    = new Point(2,2),
                Border         = new Thickness(1),
                Padding        = new Thickness(1)
            };
        }
    }
}
