namespace Miruken.Mvc.Console.Test
{
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Buffer = Console.Buffer;

    [TestClass]
    public class ViewTests : TestBase
    {
        private View _view;
        private char[][] _expected = {
            new [] {'-','-','-','-','-','-','-','-','-','-'},
            new [] {'-','-','-','-','-','-','-','-','-','-'},
            new [] {'|','|','H','e','a','d','e','r','|','|'},
            new [] {'|','|',' ',' ',' ',' ',' ',' ','|','|'},
            new [] {'-','-','-','-','-','-','-','-','-','-'},
            new [] {'-','-','-','-','-','-','-','-','-','-'},
            new [] {'-','-','-','-','-','-','-','-','-','-'},
            new [] {'-','-','-','-','-','-','-','-','-','-'},
            new [] {'-','-','-','-','-','-','-','-','-','-'},
            new [] {'|','|','|','B','o','d','y','|','|','|'},
            new [] {'|','|','|',' ',' ',' ',' ','|','|','|'},
            new [] {'|','|','|',' ',' ',' ',' ','|','|','|'},
            new [] {'|','|','|',' ',' ',' ',' ','|','|','|'},
            new [] {'|','|','|',' ',' ',' ',' ','|','|','|'},
            new [] {'|','|','|',' ',' ',' ',' ','|','|','|'},
            new [] {'|','|','|',' ',' ',' ',' ','|','|','|'},
            new [] {'|','|','|',' ',' ',' ',' ','|','|','|'},
            new [] {'-','-','-','-','-','-','-','-','-','-'},
            new [] {'-','-','-','-','-','-','-','-','-','-'},
            new [] {'-','-','-','-','-','-','-','-','-','-'}
        };

        [TestMethod]
        public void WithViewRegions()
        {
            _view = new TestViewWithViewRegions();
            var cells = Render(new Size(10, 20), _view);
            AssertCellsAreEquivelant(_expected, cells);
        }

        [TestMethod]
        public void WithOutViewRegions()
        {
            _view = new TestViewWithOutViewRegions();
            var cells = Render(new Size(10, 20), _view);
            AssertCellsAreEquivelant(_expected, cells);
        }

        [TestMethod]
        public void WithExactlyEnoughRoomForContent()
        {
            _view = new TestViewWith();
            var cells = Render(new Size(12, 7), _view);
            AssertCellsAreEquivelant(new []
            {
                new []{'-','-','-','-','-','-','-','-','-','-','-','-'},
                new []{'|',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ','|'},
                new []{'|',' ',' ','L','i','n','e',' ','1',' ',' ','|'},
                new []{'|',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ','|'},
                new []{'|',' ',' ','L','i','n','e',' ','2',' ',' ','|'},
                new []{'|',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ','|'},
                new []{'-','-','-','-','-','-','-','-','-','-','-','-'}
            }, cells);
        }

        [TestMethod]
        public void WhenContentIs1Taller()
        {
            _view = new TestViewWith();
            var cells = Render(new Size(12, 6), _view);
            AssertCellsAreEquivelant(new []
            {
                new []{'-','-','-','-','-','-','-','-','-','-','-','-'},
                new []{'|',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ','|'},
                new []{'|',' ',' ','L','i','n','e',' ','1',' ',' ','|'},
                new []{'|',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ','|'},
                new []{'|',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ','|'},
                new []{'-','-','-','-','-','-','-','-','-','-','-','-'}
            }, cells);
        }

        [TestMethod]
        public void WhenContentIs2Taller()
        {
            _view = new TestViewWith();
            var cells = Render(new Size(12, 5), _view);
            AssertCellsAreEquivelant(new []
            {
                new []{'-','-','-','-','-','-','-','-','-','-','-','-'},
                new []{'|',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ','|'},
                new []{'|',' ',' ','L','i','n','e',' ','1',' ',' ','|'},
                new []{'|',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ','|'},
                new []{'-','-','-','-','-','-','-','-','-','-','-','-'}
            }, cells);
        }

        [TestMethod]
        public void WhenContentIs3Taller()
        {
            _view = new TestViewWith();
            var cells = Render(new Size(12, 4), _view);
            AssertCellsAreEquivelant(new []
            {
                new []{'-','-','-','-','-','-','-','-','-','-','-','-'},
                new []{'|',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ','|'},
                new []{'|',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ','|'},
                new []{'-','-','-','-','-','-','-','-','-','-','-','-'}
            }, cells);
        }
    }

    public class TestViewWithViewRegions : View
    {
        public TestViewWithViewRegions()
        {
            var dock = new DockPanel();
            Content = dock;

            var headerContent = new Buffer
            {
                Border = new Thickness(2)
            }.WriteLine("Header");

            var bodyContent = new Buffer
            {
                Border = new Thickness(3)
            }.WriteLine("Body");

            var header = new ViewRegion();
            header.Add(headerContent);

            var body = new ViewRegion();
            body.Add(bodyContent);

            dock.Add(header, Dock.Top, 30);
            dock.Add(body, Dock.Fill);
        }
    }

    public class TestViewWithOutViewRegions : View
    {
        public TestViewWithOutViewRegions()
        {
            var dock = new DockPanel();
            Content = dock;

            var header= new Buffer
            {
                Border = new Thickness(2)
            }.WriteLine("Header");

            var body= new Buffer
            {
                Border = new Thickness(3)
            }.WriteLine("Body");

            dock.Add(header, Dock.Top, 30);
            dock.Add(body, Dock.Fill);
        }
    }

    public class TestViewWith: View
    {
        public TestViewWith()
        {
            var dock = new DockPanel();
            Content = dock;

            var headerContent = new Buffer();
            headerContent.WriteLine("Line 1");
            headerContent.WriteLine();
            headerContent.WriteLine("Line 2");

            var header = new ViewRegion
            {
                Border = new Thickness(1),
                Padding = new Thickness(2, 1)
            };
            header.Add(headerContent);

            dock.Add(header, Dock.Top, 100);
        }
    }
}
