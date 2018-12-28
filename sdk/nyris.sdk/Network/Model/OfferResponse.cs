using System.Collections.Generic;
using Newtonsoft.Json;

namespace Nyris.Sdk.Network.Model
{
    public sealed class OfferResponse : INyrisResponse
    {
        [JsonProperty(PropertyName = "request_code")]
        public string RequestCode { get; set; }
        
        [JsonProperty(PropertyName = "results")]
        public List<Offer> Offers { get; set; }

        [JsonProperty(PropertyName = "predicted_category")]
        public Dictionary<string, float> PredictedCategories { get; set; }

        public override string ToString()
        {
            return $"Request Code: {RequestCode}, \n" +
                   $"Offers: {Offers}, \n" +
                   $"Predicted Categories: {PredictedCategories}";
        }
    }
}