namespace Miruken.Mvc.Wpf.Tests
{
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using NReco.Linq;

    [TestClass]
    public class ActionsExtensionTests
    {
        public class HelloController : Controller
        {
            public string SayHello()
            {
                return "Hello";
            }

            public string SayHello(string name)
            {
                return $"Hello {name}";
            }

            public void DoNothing()
            {
            }
        }

        [TestMethod]
        public void Should_Execute_Action()
        {
            var parser = new LambdaParser();
            parser.Eval("@ctrl.DoNothing()", variable => variable switch
                {
                    "@ctrl" => new HelloController(),
                    _ => throw new ArgumentException($"Unknown variable '{variable}'")
                }
            );
        }

        [TestMethod]
        public void Should_Evaluate_Expression()
        {
            var parser = new LambdaParser();
            var result = parser.Eval("@ctrl.SayHello(@name)", variable =>
                variable switch
                   {
                        "@ctrl" => new HelloController(),
                        "@name" => (object)"Craig",
                        _ => throw new ArgumentException($"Unknown variable '{variable}'")
                    }
            );
            Assert.AreEqual("Hello Craig", result);
        }
    }
}
