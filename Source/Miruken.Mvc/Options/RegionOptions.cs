using Miruken.Callback;

namespace Miruken.Mvc.Options
{
    using Animation;

    public class RegionOptions : Options<RegionOptions>
    {
        public object        Tag       { get; set; }
        public LayerOptions  Layer     { get; set; }
        public WindowOptions Window    { get; set; }
        public IAnimation    Animation { get; set; }

        public override void MergeInto(RegionOptions other)
        {
            if (other == null) return;

            if (Layer != null)
            {
                var l = other.Layer ?? new LayerOptions();
                Layer.MergeInto(l);
                if (other.Layer == null)
                    other.Layer = l;
            }

            if (Animation != null)
            {
                other.Animation = other.Animation?.Merge(Animation)
                               ?? Animation;
            }

            if (Window != null)
            {
                var w = other.Window ?? new WindowOptions();
                Window.MergeInto(w);
                if (other.Window == null)
                    other.Window = w;
            }
        }
    }
}
