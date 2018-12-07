namespace Io.Nyris.Sdk.Network.API.ImageMatching
{
    public sealed class ExactOptions : Options
    {
        public ExactOptions()
        {
            Enabled = true;
        }

        public override void Reset()
        {
            Enabled = true;
        }
    }
}