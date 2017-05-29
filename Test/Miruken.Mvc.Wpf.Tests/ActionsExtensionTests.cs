namespace Miruken.Mvc.Wpf.Tests
{
    using System;
    using System.Linq.Expressions;
    using System.Text.RegularExpressions;
    using System.Windows;
    using ExpressionEvaluator;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ActionsExtensionTests
    {
        public class Scope
        {
            public object ctrl { get; set; }
        }

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
        }

        [TestMethod]
        public void Should_Call_Action_With_Arguments()
        {
            var exp     = new CompiledExpression("SayHello(\"Craig\")");
            var execute = exp.ScopeCompile<HelloController>();
            var result  = execute(new HelloController());
            Assert.AreEqual("Hello Craig", result);
        }

        [TestMethod]
        public void Should_Call_Action_With_Context()
        {
            var registry = new TypeRegistry();
            var hello    = typeof(HelloController);
            var key      = $"{hello.FullName}".Replace(".", "_").Replace('+', '_');
            registry.RegisterType(key, hello);
            var exp = new CompiledExpression($"(({key})ctrl).SayHello(\"Craig\")")
            {
                TypeRegistry = registry
            };
            var execute = exp.ScopeCompile<Scope>();
            var result  = execute(new Scope {ctrl = new HelloController()});
            Assert.AreEqual("Hello Craig", result);
        }

        [TestMethod]
        public void Should_Extract_Method_And_Args()
        {
            const string exp = "SayHello(1, true)";
            var func = Regex.Match(exp, @"\b[^()]+\((.*)\)$");

            Console.WriteLine("FuncTag: " + func.Value);
            var innerArgs = func.Groups[1].Value;

            var args = Regex.Matches(innerArgs, @"([^,]+\(.+?\))|([^,]+)");

            Console.WriteLine("Matches: " + args.Count);
            foreach (var item in args)
                Console.WriteLine("Arg: " + item);
        }
    }
}
