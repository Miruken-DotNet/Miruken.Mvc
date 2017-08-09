namespace WpfTestApp
{
    using Castle.MicroKernel.Registration;
    using Features.HelloWorld;
    using Miruken.Castle;
    using Miruken.Context;
    using Miruken.Mvc;
    using Miruken.Mvc.Castle;
    using Miruken.Mvc.Options;
    using Miruken.Mvc.Wpf;

    public partial class App
    {
        public App()
        {
            var appContext = new Context();
            var windsorHandler = new WindsorHandler(container =>
            {
                container.Install(new FeaturesInstaller(
                    new MvcInstaller(), new HandlerInstaller()).Use(
                    Classes.FromThisAssembly()));
            });
            appContext.ContextEnded += _ => windsorHandler.Dispose();

            appContext.AddHandlers(windsorHandler, new NavigateHandler(new ViewRegion()));
            appContext.NewWindow().Next<HelloWorldController>().Greet();
        }
    }
}
