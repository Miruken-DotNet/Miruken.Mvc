namespace Miruken.Mvc.Tests
{
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
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
                return IO.Handle(options) ? options : null;
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
                _rootContext.Animate(a => a.Push.Left())
                .Next<HelloController>();
            var options = controller.SayHello();
            Assert.IsNotNull(options);
            var animation = options.Animation;
            Assert.IsNotNull(animation);
            Assert.AreEqual(animation.Effect, AnimationEffect.PushLeft);
        }

        [TestMethod]
        public void Should_Propogate_Push_Options()
        {
            var controller =
                _rootContext.Animate(a => a.Push.Left())
                .Push<HelloController>();
            var options = controller.SayHello();
            Assert.IsNotNull(options);
            var animation = options.Animation;
            Assert.IsNotNull(animation);
            Assert.AreEqual(animation.Effect, AnimationEffect.PushLeft);
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
