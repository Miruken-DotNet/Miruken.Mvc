using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Miruken.Context;
using Miruken.MVC;
using static Miruken.Protocol;

namespace Miruken.Mvc.Tests
{
    [TestClass]
    public class NavigateHandlerTests
    {
        private IContext _rootContext;
        private NavigateHandler _navigate;

        public class HelloController : Controller
        {
            public void SayHello()
            {
                Console.WriteLine("Hello");
            }
        }

        [TestInitialize]
        public void TestInitialize()
        {
            _rootContext = new Context.Context();
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

        [TestMethod, 
         ExpectedException(typeof(InvalidOperationException),
            "A context is required for navigation")]
        public void Should_Fail_If_Context_Missing()
        {
            _navigate.Next<HelloController>().SayHello();
        }
    }
}
