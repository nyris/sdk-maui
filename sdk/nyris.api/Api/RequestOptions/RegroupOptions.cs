using Nyris.Api.Utils;

namespace Nyris.Api.Api.RequestOptions
{
    public sealed class RegroupOptions : Options
    {
        public RegroupOptions()
        {
            Reset();
        }

        public float Threshold { get; set; }

        public override void Reset()
        {
            Enabled = false;
            Threshold = Constants.DefaultInteger;
        }
    }
}
