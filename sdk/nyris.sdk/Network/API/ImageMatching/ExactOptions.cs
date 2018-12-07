namespace Nyris.Sdk.Network.API.ImageMatching
{
    public sealed class ExactOptions : Options
    {
        public ExactOptions()
        {
            Reset();
        }

        public override void Reset()
        {
            Enabled = true;
        }
    }
}