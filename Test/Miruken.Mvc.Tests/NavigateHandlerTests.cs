namespace Miruken.Mvc.Tests
{
    using System;
    using Animation;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Callback;
    using Callback.Policy;
    using Context;
    using Mvc.Options;
    using Views;

    [TestClass]
    public class NavigateHandlerTests
    {
        private Context _rootContext;
        private Navigator _navigator;

        static NavigateHandlerTests()
        {
            HandlerDescriptor.GetDescriptor<HelloController>();
            HandlerDescriptor.GetDescriptor<GoodbyeController>();
            HandlerDescriptor.GetDescriptor<PartialController>();
        }

        public class HelloController : Controller
        {
            [Provides, Contextual]
            public HelloController()
            {           
            }

            public NavigationOptions SayHello(string name)
            {
                Console.WriteLine($"Hello {name}");
                var navigation = IO.Resolve<Navigation>();
                Assert.IsNotNull(navigation);
                Assert.AreSame(this, navigation.Controller);
                Push<GoodbyeController>().SayGoodbye(name);
                return IO.GetOptions<NavigationOptions>();
            }

            public NavigationOptions SayHelloRegion(string name)
            {
                Console.WriteLine($"Hello region {name}");
                Show(Context.Region<GoodbyeController>(ctrl => ctrl.SayGoodbye($"region {name}")));
                return IO.GetOptions<NavigationOptions>();
            }

            public void Compose()
            {
                Partial<PartialController>().Render();
            }

            public void Render()
            {
                Assert.IsNotNull(Show<TestView>());
                var navigation = IO.Resolve<Navigation>();
                Assert.IsNotNull(navigation);
                Assert.AreSame(this, navigation.Controller);
                Assert.AreSame(navigation, Context.Resolve<Navigation>());
            }

            public void NextEnd()
            {
                Next<GoodbyeController>(ctrl => ctrl.Context.End());
            }

            public void Exception()
            {
                throw new InvalidOperationException("Crashed");
            }

            public void NextException()
            {
                Next<GoodbyeController>(ctrl => throw new InvalidOperationException("No manners"));
            }
        }

        public class GoodbyeController : Controller
        {
            [Provides, Contextual]
            public GoodbyeController()
            {             
            }

            public void SayGoodbye(string name)
            {
                Console.WriteLine($"Goodbye {name}");
            }
        }

        public class PartialController : Controller
        {
            [Provides, Contextual]
            public PartialController()
            {             
            }

            public void Render()
            {
                var navigation = IO.Resolve<Navigation>();
                Assert.AreSame(this, navigation.Controller);
                Assert.IsNotNull(navigation);
                var initiator = Context.Resolve<Navigation>();
                Assert.AreSame(this, initiator.Controller);
            }
        }

        [TestInitialize]
        public void TestInitialize()
        {
            _rootContext = new Context();
            _navigator    = new Navigator(new TestViewRegion());
            _rootContext.AddHandlers(new StaticHandler(), _navigator);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _rootContext.End();
        }

        [TestMethod]
        public void Should_Fail_Navigation_Without_Context()
        {
            var called = false;
            _navigator.Next<HelloController>(ctrl => ctrl.SayHello("hi"))
                .Catch((ex, _) =>
                {
                    Assert.IsInstanceOfType(ex, typeof(NotSupportedException));
                    called = true;
                });
            Assert.IsTrue(called);
        }

        [TestMethod]
        public void Should_Navigate_Next()
        {
            var ctrl = _rootContext.Next<HelloController>();
            ctrl.SayHello("Brenda");
            Assert.AreSame(_rootContext, ctrl.Context.Parent);
        }

        [TestMethod]
        public void Should_Navigate_Push()
        {
            var ctrl = _rootContext.Push<HelloController>();
            ctrl.SayHello("Craig");
            Assert.AreSame(_rootContext, ctrl.Context.Parent.Parent);
        }

        [TestMethod]
        public void Should_Navigate_Next_And_Push_Region()
        {
            _rootContext.Next<HelloController>(ctrl =>
            {
                ctrl.SayHelloRegion("Brenda");
                Assert.AreSame(_rootContext, ctrl.Context.Parent);
            });
        }

        [TestMethod]
        public void Should_Navigate_Push_And_Join()
        {
            var called = false;
            _rootContext.Push<HelloController>(ctrl =>
            {
                ctrl.Context.End();
                Assert.IsNull(ctrl.Context);
            }).Then((ctx, _) =>
            {
                Assert.AreSame(_rootContext, ctx.Parent.Parent);
                called = true;
            });
            Assert.IsTrue(called);
        }

        [TestMethod]
        public void Should_Navigate_Push_Next_And_Join()
        {
            var called = false;
            _rootContext.Push<HelloController>(ctrl =>
            {
                ctrl.NextEnd();
                Assert.IsNull(ctrl.Context);
            }).Then((ctx, _) =>
            {
                Assert.AreSame(_rootContext, ctx.Parent.Parent);
                called = true;
            });
            Assert.IsTrue(called);
        }

        [TestMethod]
        public void Should_Navigate_Partial()
        {
            var ctrl = _rootContext.Partial<HelloController>();
            ctrl.Compose();
            Assert.IsNull(ctrl.Context);
        }

        [TestMethod]
        public void Should_Fail_Navigation_If_Action_Throws_Exception()
        {
            var called = false;
            _rootContext.Next<HelloController>(ctrl =>
            {
                ctrl.Exception();
            }).Catch((ex, _) =>
            {
                var navigationException = ex as NavigationException;
                Assert.IsNotNull(navigationException);
                Assert.AreEqual("Crashed", ex.InnerException?.Message);
                Assert.AreSame(_rootContext, navigationException.Context);
                called = true;
            });
            Assert.IsTrue(called);
        }

        [TestMethod]
        public void Should_Fail_Navigation_If_Next_Action_Throws_Exception()
        {
            var called = false;
            _rootContext.Push<HelloController>(ctrl =>
            {
                ctrl.NextException();
            }).Catch((ex, _) =>
            {
                var navigationException = ex as NavigationException;
                Assert.IsNotNull(navigationException);
                Assert.AreEqual("No manners", ex.InnerException?.Message);
                Assert.AreSame(_rootContext, navigationException.Context.Parent?.Parent);
                called = true;
            });
            Assert.IsTrue(called);
        }

        [TestMethod]
        public void Should_Navigation_Next_After_Failed_Navigation()
        {
            var called = false;
            _rootContext.Push<HelloController>(ctrl =>
            {
                ctrl.NextException();
            }).Catch((ex, _) =>
            {
                var navigationException = ex as NavigationException;
                Assert.IsNotNull(navigationException);
                navigationException.Context.Parent?.Parent?
                    .Next<GoodbyeController>(gb => gb.SayGoodbye("Joe"));
                called = true;
            });
            Assert.IsTrue(called);
        }

        [TestMethod, 
         ExpectedException(typeof(InvalidOperationException))]
        public void Should_Reject_Initial_Property_Navigation()
        {
            var ctrl = _rootContext.Next<HelloController>();
            Assert.AreSame(_rootContext, ctrl.Context);
        }

        [TestMethod]
        public void Should_Propagate_Next_Options()
        {
            var ctrl = _rootContext.Push(Origin.MiddleLeft)
                .Next<HelloController>();
            var options = ctrl.SayHello("Kaitlyn");
            Assert.IsNotNull(options);
            var translation = options.Animation as Translate;
            Assert.IsNotNull(translation);
            Assert.AreEqual(Mode.InOut, translation.Mode);
            Assert.AreEqual(translation.Start, Origin.MiddleLeft);
        }

        [TestMethod]
        public void Should_Propagate_Push_Options()
        {
            var ctrl = _rootContext.SlideIn(Origin.MiddleRight)
                .Push<HelloController>();
            var options = ctrl.SayHello("Lauren");
            Assert.IsNotNull(options);
            var translation = options.Animation as Translate;
            Assert.IsNotNull(translation);
            Assert.AreEqual(Mode.In, translation.Mode);
            Assert.AreEqual(translation.Start, Origin.MiddleRight);
        }

        [TestMethod]
        public void Should_Render_A_View()
        {
            _rootContext.Next<HelloController>().Render();
        }
    }
}
