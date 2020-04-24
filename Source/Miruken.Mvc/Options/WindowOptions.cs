namespace Miruken.Mvc.Options
{
    using System;
    using Callback;

    public enum ScreenFill
    {
        FullScreen,
        VirtualScreen,
        CenterScreen,
        CenterVirtualScreen,
        CenterParent,
        SplitLeft,
        SplitRight,
        SplitTop,
        SplitBottom
    }

    public class WindowOptions : Options<WindowOptions>
    {
        public string      Name       { get; set; }
        public string      Title      { get; set; }
        public bool?       Modal      { get; set; }
        public bool?       Standalone { get; set; }
        public bool?       Readonly   { get; set; }
        public bool?       HideCursor { get; set; }
        public ScreenFill? FillScreen { get; set; }
        public Type        WindowType { get; set; }

        public override void MergeInto(WindowOptions other)
        {
            if (other == null) return;

            if (Name != null && other.Name == null)
                other.Name = Name;

            if (Title != null && other.Title == null)
                other.Title = Title;

            if (Modal.HasValue && !other.Modal.HasValue)
                other.Modal = Modal;

            if (Standalone.HasValue && !other.Standalone.HasValue)
                other.Standalone = Standalone;

            if (Readonly.HasValue && !other.Readonly.HasValue)
                other.Readonly = Readonly;

            if (HideCursor.HasValue && !other.HideCursor.HasValue)
                other.HideCursor = HideCursor;

            if (FillScreen.HasValue && !other.FillScreen.HasValue)
                other.FillScreen = FillScreen;

            if (WindowType != null && other.WindowType == null)
                other.WindowType = WindowType;
        }
    }

    public static class WindowOptionsExtensions
    {
        public static IHandler NewWindow(this IHandler handler, WindowOptions options = null)
        {
            return new NavigationOptions
            {
                Window = options ?? new WindowOptions()
            }.Decorate(handler);
        }

        public static IHandler NewWindow(this IHandler handler, Action<WindowBuilder> build)
        {
            var window = new WindowOptions();
            if (build != null)
            {
                var builder = new WindowBuilder(window);
                build(builder);
            }
            return new NavigationOptions { Window = window }.Decorate(handler);
        }

        public static IHandler Modal(this IHandler handler, WindowOptions window = null)
        {
            window ??= new WindowOptions();
            window.Modal = true;
            return new NavigationOptions { Window = window }.Decorate(handler);
        }

        public static IHandler Modal(this IHandler handler, Action<WindowBuilder> build)
        {
            var window = new WindowOptions { Modal = true };
            if (build != null)
            {
                var builder = new WindowBuilder(window);
                build(builder);
            }
            return new NavigationOptions { Window = window }.Decorate(handler);
        }

        public static IHandler Standalone(this IHandler handler, WindowOptions window = null)
        {
            window ??= new WindowOptions();
            window.Standalone = true;
            return new NavigationOptions { Window = window }.Decorate(handler);
        }

        public static IHandler Standalone(this IHandler handler, Action<WindowBuilder> build)
        {
            var window = new WindowOptions { Standalone = true };
            if (build != null)
            {
                var builder = new WindowBuilder(window);
                build(builder);
            }
            return new NavigationOptions { Window = window }.Decorate(handler);
        }
    }
}
