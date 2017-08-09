namespace ConsoleTestApp
{
    using System;
    using Castle.MicroKernel.Registration;
    using Features.A;
    using Features.Errors;
    using Miruken;
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
                        new MvcInstaller(),
                        new ConfigurationFactoryInstaller(),
                        new HandlerInstaller()).Use(
                            Types.FromThisAssembly()));
            });

            Console.Title = "Console Test App";
            Console.CursorVisible = false;
            var appContext = new Context();
            appContext
                .AddHandlers(windsorHandler, new NavigateHandler(Window.Region))
                .AddHandler<IError>();

            appContext.Proxy<INavigate>().Next<AController>(x =>
            {
                x.ShowAView();
                return true;
            });

            while (!Window.Quit)
            {
            }
        }
    }
}
