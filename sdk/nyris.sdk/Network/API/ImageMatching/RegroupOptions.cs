using Nyris.Sdk.Utils;

namespace Nyris.Sdk.Network.API.ImageMatching
{
    public sealed class RegroupOptions : Options
    {
        public RegroupOptions()
        {
            Reset();
        }
     
        public int Threshold { get; set; }
        
        public override void Reset()
        {
            Enabled = false;
            Threshold = Constants.DEFAULT_INTEGER;
        }
    }
}