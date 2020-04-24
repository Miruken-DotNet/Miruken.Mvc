namespace Miruken.Mvc
{
    using Register;
    using Scrutor;

    public static class RegistrationExtensions
    {
        public static Registration WithMvc(this Registration registration)
        {
            if (!registration.CanRegister(typeof(RegistrationExtensions)))
                return registration;

            return registration
                .Sources(sources => sources.FromAssemblyOf<Controller>())
                .Select((selector, publicOnly) =>
                    selector.AddClasses(x => x.AssignableTo<IController>(), publicOnly)
                        .UsingRegistrationStrategy( 
                            // Controllers are Handlers so they were initially Singletons
                            RegistrationStrategy.Replace(ReplacementBehavior.All))
                        .AsSelf().WithScopedLifetime());
        }
    }
}
