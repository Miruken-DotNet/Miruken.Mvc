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

        public override void InstallFeatures(FromDescriptor from)
        {
            var controllers = from
                .BasedOn(typeof(IController))
                .LifestyleCustom<ContextualLifestyleManager>()
                .WithServiceSelf();
            if (_configure != null)
                controllers.Configure(_configure);
        }
    }
}