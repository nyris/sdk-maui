namespace Nyris.Sdk.Network.API.XOptions
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