using SixFlags.CF.Miruken.Callback;

namespace SixFlags.CF.Miruken.MVC.Options
{
    public class RegionOptions : CallbackOptions<RegionOptions>
    {
        public object           Tag       { get; set; }
        public LayerOptions     Layer     { get; set; }
        public AnimationOptions Animation { get; set; }

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
        }
    }
}
