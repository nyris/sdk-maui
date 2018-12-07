using System.Collections.Generic;
using Newtonsoft.Json;

namespace Nyris.Sdk.Network.Model
{
    public class Region
    {
        [JsonProperty(PropertyName = "left")] 
        public float Left { get; set; }
        
        [JsonProperty(PropertyName = "top")] 
        public float Top { get; set; }
        
        [JsonProperty(PropertyName = "right")] 
        public float Right { get; set; }
        
        [JsonProperty(PropertyName = "bottom")] 
        public float Bottom { get; set; }

        public float Width() => Right - Left;
        
        public float Height() =>Bottom - Top;
    }
}