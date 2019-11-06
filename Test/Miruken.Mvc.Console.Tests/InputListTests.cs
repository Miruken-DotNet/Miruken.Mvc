namespace Miruken.Mvc.Console.Test
{
    using System;
    using System.Linq;
    using Forms;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Buffer = Buffer;

    [TestClass]
    public class InputListTest
    {
        private Buffer    buffer;
        private InputList list;
        private string    question;
        private string    selection;

        [TestInitialize]
        public void TestInitialize()
        {
            question       = "What is your favorite fruit?";
            selection      = string.Empty;
            var console    = new MockConsole();
            Window.Console = console;
            buffer = new Buffer();
            list = new InputList(
                question,
                new[] {"Apple", "Banana", "Grape", "Kiwi", "Orange"},
                s => selection = s)
            {
                Console     = console,
                Buffer      = buffer,
                LongestText = question.Length
            };
        }

        [TestMethod]
        public void ShowsTextAndHelpInTheBeginning()
        {
            AssertIsTrueFirstTime(x => x.Contains($"? {question} (Use arrow keys)"));
        }

        [TestMethod]
        public void DropsHelpTextAfterKeyPress()
        {
            list.KeyPressed(KeyInfo.DownArrow);
            AssertIsTrue(x => !x.Contains("(Use arrow keys)"));
        }

        [TestMethod]
        public void ShowsFilterHelpWhenFilterIsEmpty()
        {
            AssertIsTrue(x => x.Contains($"filter: (Type to filter)"));
        }

        [TestMethod]
        public void ShowsFilterTextWhenFilterIsApplied()
        {
            list.KeyPressed(KeyInfo.A);
            AssertIsTrue(x => x.Contains("filter: a"));
        }

        [TestMethod]
        public void FiltersList()
        {
            list.KeyPressed(KeyInfo.A);
            AssertIsTrue(x => !x.Contains("Kiwi"));
        }

        [TestMethod]
        public void SelectsFirstFilteredItem()
        {
            list.KeyPressed(KeyInfo.A);
            list.KeyPressed(KeyInfo.N);
            AssertIsTrue(x => x.Contains("> Banana"));
        }

        [TestMethod]
        public void ArrowsWorkWithFilteredItems()
        {
            list.KeyPressed(KeyInfo.A);
            list.KeyPressed(KeyInfo.N);
            list.KeyPressed(KeyInfo.DownArrow);
            AssertIsTrue(x => x.Contains("> Orange"));
        }

        [TestMethod]
        public void CanPressDownWithoutErrorWithFilteredList()
        {
            list.KeyPressed(KeyInfo.A);
            list.KeyPressed(KeyInfo.N);
            list.KeyPressed(KeyInfo.DownArrow);
            list.KeyPressed(KeyInfo.DownArrow);
            AssertIsTrue(x => x.Contains("> Orange"));
        }

        [TestMethod]
        public void CanPressUpWithoutErrorWithFilteredList()
        {
            list.KeyPressed(KeyInfo.A);
            list.KeyPressed(KeyInfo.N);
            list.KeyPressed(KeyInfo.UpArrow);
            list.KeyPressed(KeyInfo.UpArrow);
            AssertIsTrue(x => x.Contains("> Banana"));
        }

        [TestMethod]
        public void BackspaceRemovesFilter()
        {
            list.KeyPressed(KeyInfo.A);
            list.KeyPressed(KeyInfo.Backspace);
            AssertIsTrue(x => x.Contains("Kiwi"));
        }

        [TestMethod]
        public void WithZeroItems()
        {
            list.KeyPressed(KeyInfo.A);
            list.KeyPressed(KeyInfo.A);
            AssertIsTrue(x => x.Contains("0 Items"));
        }

        [TestMethod]
        public void FirstItemSelectedByDefault()
        {
            AssertIsTrueFirstTime(x => x.Contains("> Apple"));
        }

        [TestMethod]
        public void CallsCallbackWithSelectedItem()
        {
            list.KeyPressed(KeyInfo.Enter);
            Assert.AreEqual("Apple", selection);
        }

        [TestMethod]
        public void PrintsResult()
        {
            list.KeyPressed(KeyInfo.Enter);
            Assert.IsTrue(buffer.Outputs.Last().Text.Contains($"? {question} Apple"));
        }

        [TestMethod]
        public void ArrowDownMovesSelectedItemDownOne()
        {
            list.KeyPressed(KeyInfo.DownArrow);
            AssertIsTrue(x => x.Contains("> Banana"));
        }

        [TestMethod]
        public void ArrowDownTwice()
        {
            list.KeyPressed(KeyInfo.DownArrow);
            list.KeyPressed(KeyInfo.DownArrow);
            AssertIsTrue(x => x.Contains("> Grape"));
        }

        [TestMethod]
        public void UpArrow()
        {
            list.KeyPressed(KeyInfo.DownArrow);
            list.KeyPressed(KeyInfo.DownArrow);
            AssertIsTrue(x => x.Contains("> Grape"));
            list.KeyPressed(KeyInfo.UpArrow);
            AssertIsTrue(x => x.Contains("> Banana"));
        }

        [TestMethod]
        public void CanPressUpArrowEvenAtTheTopWithoutError()
        {
            list.KeyPressed(KeyInfo.UpArrow);
            AssertIsTrue(x => x.Contains("> Apple"));
        }

        [TestMethod]
        public void CanPressDownEvenAtTheBottomWithoutError()
        {
            list.KeyPressed(KeyInfo.DownArrow);
            list.KeyPressed(KeyInfo.DownArrow);
            list.KeyPressed(KeyInfo.DownArrow);
            list.KeyPressed(KeyInfo.DownArrow);
            list.KeyPressed(KeyInfo.DownArrow);
            list.KeyPressed(KeyInfo.DownArrow);
            AssertIsTrue(x => x.Contains("> Orange"));
        }

        public void AssertIsTrueFirstTime(Func<string, bool> assertion)
        {
            AssertIsTrue(assertion, true);
        }

        public void AssertIsTrue(Func<string, bool> assertion, bool firsttime = false)
        {
            var result = list.Render(firsttime);
            Console.WriteLine(result);
            Assert.IsTrue(assertion.Invoke(result));
        }
    }

    [TestClass]
    public class InputListExistingValueTest
    {
        private Buffer    buffer;
        private InputList list;
        private string    question;
        private string    selection;

        [TestMethod]
        public void SelectsFirstValue()
        {
            CreateInputList("Apple");
            AssertIsTrue(x => x.Contains("> Apple"));
        }

        [TestMethod]
        public void SelectsMiddleValue()
        {
            CreateInputList("Banana");
            AssertIsTrue(x => x.Contains("> Banana"));
        }

        [TestMethod]
        public void SelectsLastValue()
        {
            CreateInputList("Orange");
            AssertIsTrue(x => x.Contains("> Orange"));
        }

        [TestMethod]
        public void MovesDefaultSelection()
        {
            CreateInputList("Banana");
            list.KeyPressed(KeyInfo.DownArrow);
            AssertIsTrue(x => x.Contains("> Grape"));
        }

        public void AssertIsTrue(Func<string, bool> assertion, bool firsttime = false)
        {
            var result = list.Render(firsttime);
            Console.WriteLine(result);
            Assert.IsTrue(assertion.Invoke(result));
        }

        private void CreateInputList(string value)
        {
            question       = "What is your favorite fruit?";
            selection      = string.Empty;
            var console    = new MockConsole();
            Window.Console = console;
            buffer         = new Buffer();

            list = new InputList(
                question,
                value,
                new[] {"Apple", "Banana", "Grape", "Kiwi", "Orange"},
                s => selection = s)
            {
                Console     = console,
                Buffer      = buffer,
                LongestText = question.Length
            };
        }
    }
}
