namespace ConsoleTestApp
{
    using System;
    using Castle.MicroKernel.Registration;
    using Castle.Windsor.Installer;
    using Features.A;
    using Features.Errors;
    using Miruken.Castle;
    using Miruken.Context;
    using Miruken.Mvc;
    using Miruken.Mvc.Castle;
    using Miruken.Mvc.Console;
    using static Miruken.Protocol;

    internal class Program
    {
        private static void Main(string[] args)
        {
            var windsorHandler = new WindsorHandler(container =>
            {
                container.Install(
                    FromAssembly.This(),
                    new MvcInstaller(Classes.FromThisAssembly()),
                    new ConfigurationFactoryInstaller(Types.FromThisAssembly()),
                    new ResolvingInstaller(Classes.FromThisAssembly())
                );
            });

            Console.Title = "Console Test App";
            Console.CursorVisible = false;
            var appContext = new Context();
            appContext
                .AddHandlers(windsorHandler, new NavigateHandler(Window.Region))
                .AddHandler<IError>();

            P<INavigate>(appContext).Next<AController>(x =>
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
