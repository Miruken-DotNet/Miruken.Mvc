namespace Miruken.Mvc.Wpf
{
    using System;
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Input;

    public class ReadonlyWindow : Window
    {
        public ReadonlyWindow()
        {
            Focusable     = false;
            IsEnabled     = false;
            ShowInTaskbar = false;
            ShowActivated = false;
            WindowStyle   = WindowStyle.None;
            FocusManager.SetIsFocusScope(this, false);
        }

        protected override void OnActivated(EventArgs e)
        {
            if (DesignerProperties.GetIsInDesignMode(this)) return;
            Owner?.Activate();
        }
    }
}
