namespace Miruken.Mvc.Console
{
    using Views;

    public abstract class View : ContentControl, IView
    {
        #region IView

        public object ViewModel  { get; set; }

        public virtual IViewLayer Display(IViewRegion region)
        {
            return region?.Show(this);
        }

        #endregion
    }

    public abstract class View<C> : View
        where C : class, IController
    {
        public C Controller => (C) ViewModel;
    }
}
