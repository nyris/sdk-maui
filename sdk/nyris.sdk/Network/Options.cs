namespace Nyris.Sdk.Network
{
    public abstract class Options
    {
        public bool Enabled { get; set; } = false;

        public abstract void Reset();
    }
}