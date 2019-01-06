using Nyris.Sdk.Utils;

namespace Nyris.Sdk.Network.API.XOptions
{
    public class CategoryPredictionOptions : Options
    {
        public CategoryPredictionOptions()
        {
            Reset();
        }

        public float Threshold { get; set; }

        public int Limit { get; set; }

        public sealed override void Reset()
        {
            Enabled = false;
            Threshold = Constants.DEFAULT_INTEGER;
            Limit = Constants.DEFAULT_INTEGER;
        }
    }
}