using System.Collections.Generic;
using Newtonsoft.Json;

namespace Nyris.Sdk.Network.Model
{
    public class OfferResponse
    {
        [JsonProperty(PropertyName = "results")] 
        public List<Offer> Offers { get; set; }
        
        [JsonProperty(PropertyName = "predicted_category")] 
        public List<Dictionary<string, float>> PredictedCategories { get; set; }
    }
}