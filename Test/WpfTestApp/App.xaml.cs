namespace WpfTestApp
{
    using System;
    using System.Windows.Forms;
    using System.Windows.Threading;
    using Castle.MicroKernel.Registration;
    using Features.HelloWorld;
    using Miruken.Castle;
    using Miruken.Context;
    using Miruken.Error;
    using Miruken.Mvc;
    using Miruken.Mvc.Castle;
    using Miruken.Mvc.Options;
    using Miruken.Mvc.Wpf;
    using Miruken.Mvc.Wpf.Animation;

    public partial class App
    {
        public App()
        {
            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
            DispatcherUnhandledException += OnUnhandledException;

            var appContext = new Context();
            var windsorHandler = new WindsorHandler(container =>
            {
                container.Install(new FeaturesInstaller(
                    new MvcFeature(), new HandleFeature())
                        .Use(Classes.FromThisAssembly(),
                             Classes.FromAssemblyContaining<StandardAnimations>()));
            });
            appContext.ContextEnded += _ => windsorHandler.Dispose();

            Controller.GlobalPrepare += h => h.Recover();
            appContext.AddHandlers(windsorHandler, new Navigator(new ViewRegion()));
            appContext.NewWindow().Next<HelloWorldController>().Greet();
        }

        private static void OnUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            Console.WriteLine(e);
            e.Handled = true;
        }

        private static void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var exceptionDlg = new ThreadExceptionDialog((Exception)e.ExceptionObject);
            exceptionDlg.ShowDialog();
            Application.Exit();
        }
    }
}
