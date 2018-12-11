using System.Collections.Generic;
using Newtonsoft.Json;

namespace Nyris.Sdk.Network.Model
{
    public class Offer
    {
        [JsonProperty(PropertyName = "oid")] public string Id { get; set; }

        [JsonProperty(PropertyName = "title")] public string Title { get; set; }

        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }

        [JsonProperty(PropertyName = "descriptionLong")]
        public string DescriptionLong { get; set; }

        [JsonProperty(PropertyName = "brand")] public string Brand { get; set; }

        [JsonProperty(PropertyName = "catalogNumbers")]
        public List<string> CatalogNumbers { get; set; }

        [JsonProperty(PropertyName = "keywords")]
        public List<string> Keywords { get; set; }

        [JsonProperty(PropertyName = "categories")]
        public List<string> Categories { get; set; }

        [JsonProperty(PropertyName = "availability")]
        public string Availability { get; set; }

        [JsonProperty(PropertyName = "groupId")]
        public string GroupId { get; set; }

        [JsonProperty(PropertyName = "price")] public string Price { get; set; }

        [JsonProperty(PropertyName = "salePrice")]
        public string SalePrice { get; set; }

        [JsonProperty(PropertyName = "links")] public Links Links { get; set; }

        [JsonProperty(PropertyName = "images")]
        public List<string> Images { get; set; }

        [JsonProperty(PropertyName = "metadata")]
        public string Metadata { get; set; }

        [JsonProperty(PropertyName = "sku")] public string Sku { get; set; }

        [JsonProperty(PropertyName = "score")] public float Score { get; set; }
    }
}