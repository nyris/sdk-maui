namespace Nyris.Api.Api.RequestOptions
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
