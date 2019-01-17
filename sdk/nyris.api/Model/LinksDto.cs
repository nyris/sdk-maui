using Newtonsoft.Json;

namespace Nyris.Api.Model
{
    public class LinksDto
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