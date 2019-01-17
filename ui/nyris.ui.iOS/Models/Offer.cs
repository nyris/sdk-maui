using System.Collections.Generic;

namespace Nyris.UI.iOS.Models
{
    public class Offer
    {
        public string Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string DescriptionLong { get; set; }

        public string Brand { get; set; }

        public List<string> CatalogNumbers { get; set; }

        public List<string> Keywords { get; set; }

        public List<string> Categories { get; set; }

        public string Availability { get; set; }

        public string GroupId { get; set; }

        public string Price { get; set; }

        public string SalePrice { get; set; }

        public Links Links { get; set; }

        public List<string> Images { get; set; }

        public string Metadata { get; set; }

        public string Sku { get; set; }

        public float Score { get; set; }

        public override string ToString()
        {
            return $"Id: {Id}, \n" +
                   $"Title: {Title}, \n" +
                   $"Description: {Description}, \n" +
                   $"Description Long: {DescriptionLong}, \n" +
                   $"Brand: {Brand}, \n" +
                   $"Catalog Numbers: {CatalogNumbers}, \n" +
                   $"Keywords: {Keywords}, \n" +
                   $"Categories: {Categories}, \n" +
                   $"Availability: {Availability}, \n" +
                   $"Group Id: {GroupId}, \n" +
                   $"Price: {Price}, \n" +
                   $"Sale Price: {SalePrice}, \n" +
                   $"Links: {Links}, \n" +
                   $"Images: {Images}, \n" +
                   $"Metadata: {Metadata}, \n" +
                   $"Sku: {Sku}, \n" +
                   $"Score: {Score}";
        }

    }
}