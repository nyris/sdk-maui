using Nyris.Api.Model;

namespace Nyris.UI.Common.Extensions;

public static class NyrisSearchResultExtensions
{
    public static NyrisSearcherResult ToNyrisSearcherResult(this OfferResponseDto responseDto, byte[] imageBytes)
    {
        var offers = responseDto.Offers.Select(offerDto => offerDto.ToNyrisOffer()).ToList();
        return new NyrisSearcherResult(responseDto.RequestCode, offers);
    }
}