using Newtonsoft.Json;

namespace Nyris.Sdk.Network.Model
{
    public class DetectedObjectDto
    {
        [JsonProperty(PropertyName = "confidence")]
        public float Confidence { get; set; }

        [JsonProperty(PropertyName = "region")]
        public RegionDto Region { get; set; }

        public override string ToString()
        {
            return $"Confidence: {Confidence}, \n" +
                   $"Region: {Region}";
        }
    }
}