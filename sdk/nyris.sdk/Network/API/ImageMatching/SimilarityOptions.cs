using Nyris.Sdk.Utils;

namespace Nyris.Sdk.Network.API.ImageMatching
{
    public sealed class SimilarityOptions : Options
    {
        public int Threshold { get; set; }
        
        public int Limit { get; set; }

        public SimilarityOptions()
        {
            Reset();
        }
        
        public override void Reset()
        {
            Enabled = true;
            Threshold = Constants.DEFAULT_INTEGER;
            Limit = Constants.DEFAULT_INTEGER;
        }
    }
}