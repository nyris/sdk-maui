namespace Nyris.Sdk.Network.API
{
    public abstract class Options
    {
        public bool Enabled { get; set; } = false;

        public abstract void Reset();
    }
}