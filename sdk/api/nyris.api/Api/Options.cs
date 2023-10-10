namespace Nyris.Api.Api;

public abstract class Options
{
    public bool Enabled { get; set; }

    public abstract void Reset();
}