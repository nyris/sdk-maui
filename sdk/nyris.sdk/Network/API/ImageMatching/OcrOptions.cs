namespace Io.Nyris.Sdk.Network.API.ImageMatching
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