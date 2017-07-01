using Castle.Core;
using Miruken.Castle;
using Miruken.Mvc;
using Miruken.Mvc.Castle;
using Miruken.Mvc.Views;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Miruken.Callback;
using Miruken.Context;

namespace MIruken.Mvc.Castle.Tests
{
    using System.Reflection;

    [TestClass]
    public class MvcInstallerTests
    {
        protected IContext _rootContext;
        protected WindsorHandler _container;

        public class HelloView : IView
        {
            public object      ViewModel  { get; set; }
            public ViewPolicy  Policy     { get; set; }
            public IViewLayer  Layer      { get; set; }
            public IController Controller { get; set; }

            public IViewLayer Display(IViewRegion region)
            {
                return null;
            }
        }

        public class HelloController : Controller
        {
            public string SayHello(string name)
            {
                return $"Hello {name}";
            }
        }

        [TestInitialize]
        public void TestInitialize()
        {
            _rootContext = new Context();
            _container   = new WindsorHandler(container =>
                container.Install(
                    Features.FromAssembly(Assembly.GetExecutingAssembly()),
                    new MvcInstaller()));
            _rootContext.AddHandlers(_container, new NavigateHandler(new TestViewRegion()));
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _container.Dispose();
            _rootContext.End();
        }

        [TestMethod]
        public void Should_Register_Controllers_As_Contextual()
        {
            var controller = _rootContext.Resolve<HelloController>();
            Assert.IsInstanceOfType(controller, typeof(HelloController));
            var handler = _container.Container.Kernel.GetHandler(typeof(HelloController));
            var componentModel = handler.ComponentModel;
            Assert.AreEqual(LifestyleType.Custom, componentModel.LifestyleType);
        }

        [TestMethod]
        public void Should_Navigate_Next_Controller()
        {
            var message =_rootContext.Next<HelloController>().SayHello("Craig");
            Assert.AreEqual("Hello Craig", message);
        }

        [TestMethod]
        public void Should_Navigate_Push_Controller()
        {
            var message = _rootContext.Push<HelloController>().SayHello("Craig");
            Assert.AreEqual("Hello Craig", message);
        }
    }
}
