using Nyris.Api.Utils;

namespace Nyris.Api.Api.RequestOptions
{
    public sealed class CategoryPredictionOptions : Options
    {
        public CategoryPredictionOptions()
        {
            Reset();
        }

        public float Threshold { get; set; }

        public int Limit { get; set; }

        public override void Reset()
        {
            Enabled = false;
            Threshold = Constants.DefaultInteger;
            Limit = Constants.DefaultInteger;
        }
    }
}
