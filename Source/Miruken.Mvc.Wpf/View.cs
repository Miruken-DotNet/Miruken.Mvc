using System.ComponentModel;
using System.Windows.Controls;
using Miruken.Mvc.Views;

namespace Miruken.Mvc.Wpf
{
    public class View : UserControl, IView
    {
        private ViewPolicy _policy;

        [EditorBrowsable(EditorBrowsableState.Never),
         DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ViewPolicy Policy => 
            _policy ?? (_policy = new ViewPolicy(this));

        [EditorBrowsable(EditorBrowsableState.Never),
         DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public object ViewModel
        {
            get { return DataContext; }
            set { DataContext = value; }
        }

        public virtual IViewLayer Display(IViewRegion region)
        {
            return region?.Show(this);
        }
    }
}
