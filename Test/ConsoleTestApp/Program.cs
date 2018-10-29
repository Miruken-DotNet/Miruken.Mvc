namespace ConsoleTestApp
{
    using System;
    using Castle.MicroKernel.Registration;
    using Features.A;
    using Features.Errors;
    using Miruken.Castle;
    using Miruken.Context;
    using Miruken.Mvc;
    using Miruken.Mvc.Castle;
    using Miruken.Mvc.Console;

    internal class Program
    {
        private static void Main(string[] args)
        {
            var windsorHandler = new WindsorHandler(container =>
            {
                container.Install(
                    new FeaturesInstaller(
                        new MvcFeature(), new ConfigurationFeature(),
                        new HandleFeature()).Use(
                            Types.FromThisAssembly()));
            });

            Console.Title = "Console Test App";
            Console.CursorVisible = false;
            var appContext = new Context();
            appContext.AddHandlers(windsorHandler,
                new Navigator(Window.Region),
                new ErrorHandler());

            appContext.Next<AController>(x => x.ShowAView());

            while (!Window.Quit)
            {
            }
        }
    }
}
