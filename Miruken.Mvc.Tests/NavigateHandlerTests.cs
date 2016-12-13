namespace Miruken.Mvc.Tests
{
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Context;
    using Options;

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
            Assert.AreSame(_rootContext, controller.Context);
            controller.SayHello();
        }

        [TestMethod]
        public void Should_Navigate_Push()
        {
            var controller = _rootContext.Push<HelloController>();
            Assert.AreSame(_rootContext, controller.Context.Parent);
            controller.SayHello();
        }

        [TestMethod]
        public void Should_Propogate_Options()
        {
            var controller = 
                _rootContext.Animate(a => a.Push.Left())
                .Next<HelloController>();
            Assert.AreSame(_rootContext, controller.Context);
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
