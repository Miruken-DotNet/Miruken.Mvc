using Miruken.Callback;

namespace Miruken.Mvc.Options
{
    using System;
    using System.Linq;
    using Views;

    public class RegionOptions : Options<RegionOptions>
    {
        public object Tag       { get; set; }
        public bool?  Push      { get; set; }
        public bool?  Overlay   { get; set; }
        public bool?  Unload    { get; set; }
        public bool?  Immediate { get; set; }

        public Func<IViewLayer[], IViewLayer> Selector { get; set; }

        public IViewLayer Choose(IViewLayer[] layers)
        {
            return Selector?.Invoke(layers);
        }

        public override void MergeInto(RegionOptions other)
        {
            if (other == null) return;

            if (Tag != null && other.Tag == null)
                other.Tag = Tag;

            if (Push.HasValue && !other.Push.HasValue)
                other.Push = Push;

            if (Overlay.HasValue && !other.Overlay.HasValue)
                other.Overlay = Overlay;

            if (Unload.HasValue && !other.Unload.HasValue)
                other.Unload = Unload;

            if (Immediate.HasValue && !other.Immediate.HasValue)
                other.Immediate = Immediate;

            if (Selector != null && other.Selector == null)
                other.Selector = Selector;
        }
    }

    public static class RegionOptionsExtensions
    {
        public static IHandler PushLayer(this IHandler handler)
        {
            return new NavigationOptions
            {
                Region = new RegionOptions { Push = true }
            }.Decorate(handler);
        }

        public static IHandler Overlay(this IHandler handler)
        {
            return new NavigationOptions
            {
                Region = new RegionOptions { Overlay = true }
            }.Decorate(handler);
        }

        public static IHandler UnloadRegion(this IHandler handler)
        {
            return new NavigationOptions
            {
                Region = new RegionOptions { Unload = true }
            }.Decorate(handler);
        }

        public static IHandler DisplayImmediate(this IHandler handler)
        {
            return new NavigationOptions
            {
                Region = new RegionOptions { Immediate = true }
            }.Decorate(handler);
        }

        public static IHandler Layer(this IHandler handler, IViewLayer layer)
        {
            return new NavigationOptions
            {
                Region = new RegionOptions
                {
                    Selector = layers =>
                    {
                        if (!layers.Contains(layer))
                            throw new ArgumentException("Layer not found");
                        return layer;
                    }
                }
            }.Decorate(handler);
        }

        public static IHandler Layer(this IHandler handler, int offset)
        {
            return new NavigationOptions
            {
                Region = new RegionOptions
                {
                    Selector = layers =>
                    {
                        var index = offset < 0
                                  ? layers.Length + offset - 1
                                  : offset;
                        if (index < 0 || index >= layers.Length)
                            throw new IndexOutOfRangeException();
                        return layers[index];
                    }
                }
            }.Decorate(handler);
        }
    }
}
