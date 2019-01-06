using Nyris.Sdk.Utils;

namespace Nyris.Sdk.Network.API.XOptions
{
    public sealed class SimilarityOptions : Options
    {
        public SimilarityOptions()
        {
            Reset();
        }

        public float Threshold { get; set; }

        public int Limit { get; set; }

        public override void Reset()
        {
            Enabled = true;
            Threshold = Constants.DEFAULT_INTEGER;
            Limit = Constants.DEFAULT_INTEGER;
        }
    }
}