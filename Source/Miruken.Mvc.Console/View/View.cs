namespace Miruken.Mvc.Console
{
    using Views;

    public abstract class View : ContentControl, IView
    {
        #region IView

        private ViewPolicy _policy;

        public ViewPolicy Policy
        {
            get { return _policy ?? (_policy = new ViewPolicy(this)); }
            set { _policy = value; }
        }

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
