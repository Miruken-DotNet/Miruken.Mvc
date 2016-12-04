namespace Miruken.Mvc.Views
{
    public static class ViewExtensions
    {
        public static IView Track(this IView view)
        {
            view?.Policy?.Track();
            return view;
        }

        public static IView Retain(this IView view)
        {
            view?.Policy?.Retain();
            return view;
        }

        public static IView Release(this IView view)
        {
            view?.Policy?.Release();
            return view;
        }

        public static IView DependsOn(this IView view, IView dependency)
        {
            if (view?.Policy != null && dependency != null)
                view.Policy.AddDependency(dependency.Policy);
            return view;
        }

        public static bool DoesDependOn(this IView view, IView dependency)
        {
            if (view?.Policy != null && dependency != null)
                return view.Policy.IsDependency(dependency.Policy);
            return false;
        }
    } 
}
