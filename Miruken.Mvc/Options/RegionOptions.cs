using Miruken.Callback;

namespace Miruken.Mvc.Options
{
    public class RegionOptions : Options<RegionOptions>
    {
        public object           Tag       { get; set; }
        public LayerOptions     Layer     { get; set; }
        public AnimationOptions Animation { get; set; }
        public WindowOptions    Window    { get; set; }

        public override void MergeInto(RegionOptions other)
        {
            if (Layer != null)
            {
                var l = other.Layer ?? new LayerOptions();
                Layer.MergeInto(l);
                if (other.Layer == null)
                    other.Layer = l;
            }

            if (Animation != null)
            {
                var a = other.Animation ?? new AnimationOptions();
                Animation.MergeInto(a);
                if (other.Animation == null)
                    other.Animation = a;
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
