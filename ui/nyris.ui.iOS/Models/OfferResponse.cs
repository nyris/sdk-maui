using System.Collections.Generic;
using System.Linq;
using Nyris.Api.Model;

namespace Nyris.UI.iOS.Models
{
    public class OfferResponse
    {
        public string RequestCode { get; set; }

        public List<Offer> Offers { get; set; }

        public List<PredictedCategory> PredictedCategories { get; set; }

        public OfferResponse()
        {
        }

        public OfferResponse(OfferResponseDto offerResponseDto)
        {
            RequestCode = offerResponseDto?.RequestCode;

            Offers = offerResponseDto?.Offers.Select(offer => new Offer
            {
                Id = offer.Id,
                Title = offer.Title,
                Description = offer.Description,
                DescriptionLong = offer.DescriptionLong,
                Brand = offer.Brand,
                CatalogNumbers = offer.CatalogNumbers,
                Keywords = offer.Keywords,
                Categories = offer.Categories,
                Availability = offer.Availability,
                GroupId = offer.GroupId,
                Price = offer.Price,
                SalePrice = offer.SalePrice,
                Links = new Links
                {
                    Main = offer.LinksDto?.Main,
                    Mobile = offer.LinksDto?.Mobile
                },
                Images = offer.Images,
                Metadata = offer.Metadata,
                Sku = offer.Sku,
                Score = offer.Score,
            }).ToList();

            PredictedCategories = new List<PredictedCategory>();
            if (offerResponseDto?.PredictedCategories == null)
            {
                return;
            }
            foreach (var (key, value) in offerResponseDto?.PredictedCategories)
            {
                PredictedCategories.Add(new PredictedCategory
                {
                    Name = key,
                    Score = value
                });
            }
        }
    }
}