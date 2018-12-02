namespace Nyris.Sdk.Network.API.ImageMatching
{
    public sealed class RecommendationOptions : Options
    {
        public RecommendationOptions()
        {
            Reset();
        }

        public override void Reset()
        {
            Enabled = false;
        }
    }
}