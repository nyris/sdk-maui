using Newtonsoft.Json;

namespace Nyris.Api.Model;

public class RegionsObjectDto
{
    [JsonProperty(PropertyName = "regions")]
    public DetectedObjectDto[] regions { get; set; }
}

public class DetectedObjectDto
{
    [JsonProperty(PropertyName = "confidence")]
    public float Confidence { get; set; }

    [JsonProperty(PropertyName = "region")]
    public RegionDto RegionDto { get; set; }

    public override string ToString()
    {
        return $"Confidence: {Confidence}, \n" +
               $"Region: {RegionDto}";
    }
}