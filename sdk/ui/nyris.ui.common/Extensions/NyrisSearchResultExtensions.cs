using Nyris.Api.Model;

namespace Nyris.UI.Common.Extensions;

public static class NyrisSearchResultExtensions
{
    public static NyrisSearcherResult ToNyrisSearcherResult(this OfferResponseDto responseDto)
    {
        var offers = responseDto.Offers.Select(offerDto => offerDto.ToNyrisOffer()).ToList();
        var predictedCategories =
            responseDto.PredictedCategories.ToDictionary(entry => entry.Key, entry => entry.Value);
        return new NyrisSearcherResult(responseDto.RequestCode, offers, predictedCategories);
    }
}