namespace Io.Nyris.Sdk.Network.API
{
    public abstract class Options
    {
        public static int UNDEFINED_LIMIT = -1;
        public static float UNDEFINED_THRESHOLD = -1F;
        public const int DEFAULT_LIMIT = 20;
        
        public bool Enabled { get; set; } = false;

        public abstract void Reset();
    }
}