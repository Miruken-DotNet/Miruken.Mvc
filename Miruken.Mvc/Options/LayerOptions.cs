using System;
using System.Linq;
using Miruken.Callback;
using Miruken.Mvc.Views;

namespace Miruken.Mvc.Options
{
    public class LayerOptions : CallbackOptions<LayerOptions>
    {
        public bool? Push      { get; set; }

        public bool? Overlay   { get; set; }

        public bool? Unload    { get; set; }

        public bool? Immediate { get; set; }

        public Func<IViewLayer[], IViewLayer> Selector { get; set; }
 
        public IViewLayer Choose(IViewLayer[] layers)
        {
            return Selector?.Invoke(layers);
        }

        public override void MergeInto(LayerOptions other)
        {
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

    public static class LayerOptionsExtensions
    {
        public static IHandler PushLayer(this IHandler handler)
        {
            return handler == null ? null
                 : new OptionsHandler<RegionOptions>(handler,
                     new RegionOptions {
                        Layer = new LayerOptions { Push = true }
                     });
        }

        public static IHandler Overlay(this IHandler handler)
        {
            return handler == null ? null
                 : new OptionsHandler<RegionOptions>(handler,
                     new RegionOptions
                     {
                         Layer = new LayerOptions { Overlay = true }
                     });
        }

        public static IHandler UnloadRegion(this IHandler handler)
        {
            return handler == null ? null
                 : new OptionsHandler<RegionOptions>(handler,
                     new RegionOptions {
                        Layer = new LayerOptions { Unload = true }
                     });
        }

        public static IHandler DisplayImmediate(this IHandler handler)
        {
            return handler == null ? null
                 : new OptionsHandler<RegionOptions>(handler,
                     new RegionOptions
                     {
                         Layer = new LayerOptions { Immediate = true }
                     });
        }

        public static IHandler Layer(this IHandler handler, IViewLayer layer)
        {
            return handler == null ? null
                 : new OptionsHandler<RegionOptions>(handler, 
                     new RegionOptions {
                         Layer = new LayerOptions {
                            Selector = layers => {
                                if (!layers.Contains(layer))
                                    throw new ArgumentException("Layer not found");
                                return layer;
                            }
                         }
                     });
        }

        public static IHandler Layer(this IHandler handler, int offset)
        {
            return handler == null ? null
                 : new OptionsHandler<RegionOptions>(handler,
                     new RegionOptions {
                        Layer = new LayerOptions {
                            Selector = layers => {
                                 var index = offset < 0
                                           ? layers.Length + offset - 1
                                           : offset;
                                 if (index < 0 || index >= layers.Length)
                                     throw new IndexOutOfRangeException();
                                 return layers[index];
                             }
                        }
                 });
        }
    }
}
