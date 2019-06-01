namespace Miruken.Mvc.Castle
{
    using System;
    using System.Collections.Generic;
    using Miruken.Castle;
    using global::Castle.MicroKernel.Registration;

    public class MvcFeature : FeatureInstaller
    {
        private Action<ComponentRegistration> _configure;

        public MvcFeature ConfigureControllers(Action<ComponentRegistration> configure)
        {
            _configure += configure;
            return this;
        }

        public override IEnumerable<FromDescriptor> GetFeatures()
        {
            yield return Classes.FromAssemblyContaining<Navigator>();
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