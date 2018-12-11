using System.Collections.Generic;
using Newtonsoft.Json;

namespace Nyris.Sdk.Network.Model
{
    public sealed class OfferResponse
    {
        [JsonProperty(PropertyName = "results")]
        public List<Offer> Offers { get; set; }

        [JsonProperty(PropertyName = "predicted_category")]
        public Dictionary<string, float> PredictedCategories { get; set; }
    }
}