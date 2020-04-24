namespace Miruken.Mvc.Wpf.TestApp
{
    using System;
    using System.Windows;
    using System.Windows.Threading;
    using Animation;
    using Context;
    using Features.HelloWorld;
    using Microsoft.Extensions.DependencyInjection;
    using Options;
    using Register;

    public partial class App
    {
        private Context _context;

        static App()
        {
            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
        }

        private void App_Startup(object sender, StartupEventArgs e)
        {
            _context = new ServiceCollection()
                .AddMiruken(configure => configure
                    .PublicSources(s => s.FromAssemblyOf<App>())
                    .WithWpf()
                ).Build();

            _context.NewWindow().Next<HelloWorldController>(ctrl => ctrl.Greet());
        }

        private void App_Exit(object sender, ExitEventArgs e)
        {
            _context?.End();
        }

        private void App_DispatcherUnhandledException(
            object                                sender, 
            DispatcherUnhandledExceptionEventArgs e)
        {
            Console.WriteLine(e);
            e.Handled = true;
        }

        private static void OnUnhandledException(
            object                      sender,
            UnhandledExceptionEventArgs e)
        {
            Current.Shutdown();
        }
    }
}
