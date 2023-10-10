using Nyris.Api.Model;
using Nyris.UI.Common.Models;

namespace Nyris.UI.Common.Extensions;

public static class OfferExtensions { 
    
    public static Offer ToNyrisOffer(this OfferDto offerDto)
    {
        return new Offer(
            offerDto.Id, 
            offerDto.Title,
            offerDto.Description, 
            offerDto.DescriptionLong, 
            offerDto.Brand,
            offerDto.CatalogNumbers, 
            offerDto.Keywords, 
            offerDto.Categories, 
            offerDto.Availability,
            offerDto.GroupId, 
            offerDto.Price,
            offerDto.SalePrice, 
            offerDto.LinksDto.ToNyrisLinks(), 
            offerDto.Images,
            offerDto.Metadata, 
            offerDto.Sku, 
            offerDto.Score
        );
    }
}