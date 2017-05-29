namespace MajorLeagueMiruken.Console
{
    using System;
    using Castle.MicroKernel.Registration;
    using Castle.Windsor.Installer;
    using Features.Error;
    using Features.Layout;
    using Miruken.Castle;
    using Miruken.Context;
    using Miruken.Mvc;
    using Miruken.Mvc.Castle;
    using Miruken.Mvc.Console;

    using static Miruken.Protocol;

    class Program
    {
        static void Main(string[] args)
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

            Console.Title = "Major League Miruken";
            Console.Clear();

            var appContext = new Context();
            appContext
                .AddHandlers(windsorHandler, new NavigateHandler(Window.Region))
                .AddHandler<IError>();

            P<INavigate>(appContext).Next<LayoutController>(x =>
            {
                x.ShowLayout();
                return true;
            });

            while (!Window.Quit)
            {
            }
        }
    }
}
