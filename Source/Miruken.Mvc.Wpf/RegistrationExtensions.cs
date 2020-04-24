namespace Miruken.Mvc.Wpf
{
    using Register;
    using Views;

    public static class RegistrationExtensions
    {
        public static Registration WithWpf(this Registration registration,
            IViewRegion mainRegion = null)
        {
            if (!registration.CanRegister(typeof(RegistrationExtensions)))
                return registration;

            return registration.WithMvc()
                .Sources(sources => sources.FromAssemblyOf<ViewRegion>())
                .AddHandlers(new Navigator(mainRegion ?? new ViewRegion()));
        }
    }
}
