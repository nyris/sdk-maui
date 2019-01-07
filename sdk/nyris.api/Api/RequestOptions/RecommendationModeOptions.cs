namespace Nyris.Api.Api.RequestOptions
{
    public sealed class RecommendationModeOptions : Options
    {
        public RecommendationModeOptions()
        {
            Reset();
        }

        public override void Reset()
        {
            Enabled = false;
        }
    }
}
