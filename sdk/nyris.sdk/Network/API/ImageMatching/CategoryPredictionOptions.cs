using Nyris.Sdk.Utils;

namespace Nyris.Sdk.Network.API.ImageMatching
{
    public class CategoryPredictionOptions : Options
    {
        public int Threshold { get; set; }
        
        public int Limit { get; set; }

        public CategoryPredictionOptions()
        {
            Reset();
        }
        
        public override void Reset()
        {
            Enabled = false;
            Threshold = Constants.DEFAULT_INTEGER;
            Limit = Constants.DEFAULT_INTEGER;
        }
    }
}