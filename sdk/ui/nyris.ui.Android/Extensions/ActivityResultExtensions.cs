using Android.OS;
using Android.Util;
using Nyris.UI.Common;
using NyrisOffer = Nyris.UI.Common.Models.Offer;
using NyrisLinks = Nyris.UI.Common.Models.Links;
using AndroidX.Activity.Result;
using Nyris.UI.Android.Models;
using AndroidOSVersion = Android.OS.Build.VERSION;

namespace Nyris.UI.Android.Extensions;
public static class ActivityResultExtensions
{
    internal static NyrisSearcherResult? ToNyrisSearchResult(this ActivityResult activityResult)
    {
        var data = activityResult.Data;
        if (activityResult.ResultCode != (int)Result.Ok || activityResult.Data == null) return null;
        try
        {
            OfferResponse offerResponse;
            if (AndroidOSVersion.SdkInt >= BuildVersionCodes.Tiramisu)
            {
                offerResponse =
                    data.GetParcelableExtra(NyrisSearcher.SearchResultKey, 
                        Java.Lang.Class.FromType(typeof(OfferResponse))) as OfferResponse;
            }
            else
            {
                offerResponse = data.GetParcelableExtra(NyrisSearcher.SearchResultKey) as OfferResponse;
            }

            return new NyrisSearcherResult(RequestCode: offerResponse!.RequestCode,
                Offers: offerResponse.Offers.Select(x => x.ToNyrisOffer()).ToList());
        }
        catch (Exception e)
        {
            Log.Error("NyrisSearcher", e.Message);
            return null;
        }
    }

    private static NyrisOffer ToNyrisOffer(this Offer offer)
    {
        return new NyrisOffer(
            offer.Id, 
            offer.Title,
            offer.Description, 
            offer.DescriptionLong, 
            offer.Brand,
            offer.CatalogNumbers, 
            offer.Keywords, 
            offer.Categories, 
            offer.Availability,
            offer.GroupId, 
            offer.Price,
            offer.SalePrice, 
            offer.Links.ToNyrisLinks(), 
            offer.Images,
            offer.Metadata, 
            offer.Sku, 
            offer.Score
        );
    }

    private static NyrisLinks ToNyrisLinks(this Links? links)
    {
        return new NyrisLinks(links?.Main, links?.Mobile);
    }
}
