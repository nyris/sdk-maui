using Nyris.Sdk.Utils;

namespace Nyris.Sdk.Network.API.XOptions
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
            Threshold = Constants.DEFAULT_INTEGER;
        }
    }
}