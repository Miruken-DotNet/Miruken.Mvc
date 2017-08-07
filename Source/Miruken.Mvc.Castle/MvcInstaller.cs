namespace Miruken.Mvc.Castle
{
    using System;
    using Miruken.Castle;
    using global::Castle.MicroKernel.Registration;

    public class MvcInstaller : FeatureInstaller
    {
        private Action<ComponentRegistration> _configure;

        public MvcInstaller ConfigureControllers(Action<ComponentRegistration> configure)
        {
            _configure += configure;
            return this;
        }

        protected override void InstallFeature(FeatureAssembly feature)
        {
            var controllers = Classes.FromAssembly(feature.Assembly)
                .BasedOn(typeof(IController))
                .LifestyleCustom<ContextualLifestyleManager>()
                .WithServiceSelf();
            if (_configure != null)
                controllers.Configure(_configure);
            Container.Register(controllers);
        }
    }
}