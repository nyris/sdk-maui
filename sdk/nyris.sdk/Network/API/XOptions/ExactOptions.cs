namespace Nyris.Sdk.Network.API.XOptions
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