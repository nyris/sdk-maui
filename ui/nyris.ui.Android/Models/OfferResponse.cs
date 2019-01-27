using System.Collections.Generic;
using System.Linq;
using Android.OS;
using Android.Runtime;
using Java.Interop;
using Nyris.Api.Model;

namespace Nyris.UI.Android.Models
{
    public class OfferResponse : Java.Lang.Object, IParcelable
    {
        public string TakenImagePath { get; set; }

        public string RequestCode { get; set; }

        public List<Offer> Offers { get; set; }

        public List<PredictedCategory> PredictedCategories { get; set; }

        public OfferResponse()
        {
        }

        public OfferResponse(OfferResponseDto offerResponseDto)
        {
            RequestCode = offerResponseDto?.RequestCode;

            Offers = offerResponseDto?.Offers.Select(offer =>
            {
                return new Offer
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
                };
            }).ToList();

            PredictedCategories = new List<PredictedCategory>();
            if (offerResponseDto?.PredictedCategories == null)
            {
                return;
            }
            foreach (var entry in offerResponseDto?.PredictedCategories)
            {
                PredictedCategories.Add(new PredictedCategory
                {
                    Name = entry.Key,
                    Score = entry.Value
                });
            }
        }

        [ExportField("CREATOR")]
        public static ParcelableCreator InitializeCreator()
        {
            return new ParcelableCreator();
        }

        public void WriteToParcel(Parcel dest, [GeneratedEnum] ParcelableWriteFlags flags)
        {
            dest.WriteString(TakenImagePath);
            dest.WriteString(RequestCode);
            dest.WriteTypedList(Offers);
            dest.WriteTypedList(PredictedCategories);
        }

        public int DescribeContents()
        {
            return 0;
        }

        public class ParcelableCreator : Java.Lang.Object, IParcelableCreator
        {
            public Java.Lang.Object CreateFromParcel(Parcel source)
            {
                var searcherOfferResponse = new OfferResponse
                {
                    TakenImagePath = source.ReadString(),
                    RequestCode = source.ReadString(),
                    Offers = new List<Offer>(),
                    PredictedCategories = new List<PredictedCategory>()
                };

                var nOffers = source.CreateTypedArrayList(Offer.InitializeCreator());
                foreach (Offer item in nOffers)
                {
                    searcherOfferResponse.Offers.Add(item);
                }

                var nPredicatedCategories = source.CreateTypedArrayList(PredictedCategory.InitializeCreator());
                foreach (PredictedCategory item in nPredicatedCategories)
                {
                    searcherOfferResponse.PredictedCategories.Add(item);
                }

                return searcherOfferResponse;
            }

            public Java.Lang.Object[] NewArray(int size)
            {
                return new Java.Lang.Object[size];
            }
        }
    }
}
