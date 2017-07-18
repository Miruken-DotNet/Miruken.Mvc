namespace WpfTestApp
{
    using System.Reflection;
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
                container
                    .Install(WithFeatures.FromAssembly(
                        Assembly.GetExecutingAssembly()),
                    new MvcInstaller(), new ResolvingInstaller());
            });
            appContext.ContextEnded += _ => windsorHandler.Dispose();

            appContext.AddHandlers(windsorHandler, new NavigateHandler(new ViewRegion()));
            appContext.NewWindow().Next<HelloWorldController>().Greet();
        }
    }
}
