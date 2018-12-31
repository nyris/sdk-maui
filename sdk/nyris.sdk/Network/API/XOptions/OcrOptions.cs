namespace Nyris.Sdk.Network.API.XOptions
{
    public sealed class OcrOptions : Options
    {
        public OcrOptions()
        {
            Reset();
        }

        public override void Reset()
        {
            Enabled = true;
        }
    }
}