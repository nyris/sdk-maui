using Newtonsoft.Json;

namespace Nyris.Sdk.Network.Model
{
    public class Links
    {
        [JsonProperty(PropertyName = "main")] public string Main { get; set; }

        [JsonProperty(PropertyName = "mobile")]
        public string Mobile { get; set; }

        public override string ToString()
        {
            return $"Main: {Main}, \n" +
                   $"Mobile: {Mobile}";
        }
    }
}