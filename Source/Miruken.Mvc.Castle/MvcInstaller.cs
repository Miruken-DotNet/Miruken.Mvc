using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Miruken.Castle;

namespace Miruken.Mvc.Castle
{
    public class MvcInstaller : IWindsorInstaller
    {
        private readonly FromAssemblyDescriptor[] _fromAssemblies;

        public MvcInstaller(params FromAssemblyDescriptor[] fromAssemblies)
        {
            _fromAssemblies = fromAssemblies;
        }

        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            foreach (var assemebly in _fromAssemblies)
            {
                container.Register(assemebly
                    .BasedOn(typeof(IController))
                    .LifestyleCustom<ContextualLifestyleManager>()
                    .WithServiceSelf());
            }
        }
    }
}