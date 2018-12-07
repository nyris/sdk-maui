using Newtonsoft.Json;

namespace Io.Nyris.Sdk.Network.Model
{
    public class Links
    {
        [JsonProperty(PropertyName = "main")]
        public string main { get; set; }
        
        [JsonProperty(PropertyName = "mobile")]
        public string Mobile { get; set; }
    }
}