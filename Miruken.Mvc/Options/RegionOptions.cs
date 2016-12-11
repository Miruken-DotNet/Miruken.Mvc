using Miruken.Callback;

namespace Miruken.Mvc.Options
{
    public class RegionOptions : CallbackOptions<RegionOptions>
    {
        public object       Tag   { get; set; }
        public LayerOptions Layer { get; set; }

        public override void MergeInto(RegionOptions other)
        {
            if (Layer != null)
            {
                var l = other.Layer ?? new LayerOptions();
                Layer.MergeInto(l);
                if (other.Layer == null)
                    other.Layer = l;
            }
        }
    }
}
