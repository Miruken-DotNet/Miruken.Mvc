namespace Miruken.Mvc.Tests
{
    using System;
    using Animation;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Callback;
    using Context;
    using Mvc.Options;

    [TestClass]
    public class NavigateHandlerTests
    {
        private IContext _rootContext;
        private NavigateHandler _navigate;

        public class HelloController : Controller
        {
            public RegionOptions SayHello()
            {
                Console.WriteLine("Hello");
                var options = new RegionOptions();
                return IO.Handle(options, true) ? options : null;
            }
        }

        [TestInitialize]
        public void TestInitialize()
        {
            _rootContext = new Context();
            _navigate    = new NavigateHandler(new TestViewRegion());
            _rootContext.AddHandlers(_navigate, new TestContainer());
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _rootContext.End();
        }

        [TestMethod]
        public void Should_Navigate_Next()
        {
            var controller = _rootContext.Next<HelloController>();
            controller.SayHello();
            Assert.AreEqual(_rootContext, controller.Context);
        }

        [TestMethod]
        public void Should_Navigate_Push()
        {
            var controller = _rootContext.Push<HelloController>();
            controller.SayHello();
            Assert.AreEqual(_rootContext, controller.Context.Parent);
        }

        [TestMethod, 
         ExpectedException(typeof(InvalidOperationException))]
        public void Should_Reject_Initial_Property_Navigation()
        {
            var controller = _rootContext.Next<HelloController>();
            Assert.AreEqual(_rootContext, controller.Context);
        }

        [TestMethod]
        public void Should_Propogate_Next_Options()
        {
            var controller = 
                _rootContext.Push(Origin.MiddleLeft)
                .Next<HelloController>();
            var options = controller.SayHello();
            Assert.IsNotNull(options);
            var translation = options.Animation as Translate;
            Assert.IsNotNull(translation);
            Assert.AreEqual(Mode.InOut, translation.Mode);
            Assert.AreEqual(translation.Start, Origin.MiddleLeft);
        }

        [TestMethod]
        public void Should_Propogate_Push_Options()
        {
            var controller =
                _rootContext.SlideIn(Origin.MiddleRight)
                .Push<HelloController>();
            var options = controller.SayHello();
            Assert.IsNotNull(options);
            var translation = options.Animation as Translate;
            Assert.IsNotNull(translation);
            Assert.AreEqual(Mode.In, translation.Mode);
            Assert.AreEqual(translation.Start, Origin.MiddleRight);
        }

        [TestMethod, 
         ExpectedException(typeof(InvalidOperationException),
            "A context is required for navigation")]
        public void Should_Fail_If_Context_Missing()
        {
            _navigate.Next<HelloController>().SayHello();
        }
    }
}
