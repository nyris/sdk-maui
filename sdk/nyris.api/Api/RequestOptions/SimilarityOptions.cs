using Nyris.Api.Utils;

namespace Nyris.Api.Api.RequestOptions
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
            Threshold = Constants.DefaultInteger;
            Limit = Constants.DefaultInteger;
        }
    }
}
