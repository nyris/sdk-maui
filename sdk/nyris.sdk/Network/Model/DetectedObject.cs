using Newtonsoft.Json;

namespace Io.Nyris.Sdk.Network.Model
{
    public class DetectedObject
    {
        [JsonProperty(PropertyName = "confidence")] 
        public float Confidence { get; set; }
        
        [JsonProperty(PropertyName = "region")] 
        public Region Region { get; set; }
    }
}