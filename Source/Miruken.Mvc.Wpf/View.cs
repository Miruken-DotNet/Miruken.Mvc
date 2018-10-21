using System.ComponentModel;
using System.Windows.Controls;
using Miruken.Mvc.Views;

namespace Miruken.Mvc.Wpf
{
    public class View : UserControl, IView
    {
        [EditorBrowsable(EditorBrowsableState.Never),
         DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public object ViewModel
        {
            get => DataContext;
            set => DataContext = value;
        }

        public virtual IViewLayer Display(IViewRegion region)
        {
            return region?.Show(this);
        }
    }
}
