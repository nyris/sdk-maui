namespace Nyris.Api.Api.RequestOptions
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
