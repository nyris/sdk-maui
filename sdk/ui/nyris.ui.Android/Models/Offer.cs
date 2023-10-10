using System.Collections.Generic;
using Android.OS;
using Android.Runtime;
using Java.Interop;
using JClass = Java.Lang.Class;

namespace Nyris.UI.Android.Models;

public class Offer : Java.Lang.Object, IParcelable
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

    [ExportField("CREATOR")]
    public static IParcelableCreator InitializeCreator()
    {
        return new ParcelableCreator();
    }

    public void WriteToParcel(Parcel dest, [GeneratedEnum] ParcelableWriteFlags flags)
    {
        dest.WriteString(Id);
        dest.WriteString(Title);
        dest.WriteString(Description);
        dest.WriteString(DescriptionLong);
        dest.WriteString(Brand);
        dest.WriteStringList(CatalogNumbers);
        dest.WriteStringList(Keywords);
        dest.WriteStringList(Categories);
        dest.WriteString(Availability);
        dest.WriteString(GroupId);
        dest.WriteString(Price);
        dest.WriteString(SalePrice);
        dest.WriteParcelable(Links, flags);
        dest.WriteStringList(Images);
        dest.WriteString(Metadata);
        dest.WriteString(Sku);
        dest.WriteFloat(Score);
    }

    public int DescribeContents()
    {
        return 0;
    }

    public class ParcelableCreator : Java.Lang.Object, IParcelableCreator
    {
        public Java.Lang.Object CreateFromParcel(Parcel source)
        {
            var offer = new Offer();
            offer.Id = source.ReadString();
            offer.Title = source.ReadString();
            offer.Description = source.ReadString();
            offer.DescriptionLong = source.ReadString();
            offer.Brand = source.ReadString();
            offer.CatalogNumbers = new List<string>();
            source.ReadStringList(offer.CatalogNumbers);
            offer.Keywords = new List<string>();
            source.ReadStringList(offer.Keywords);
            offer.Categories = new List<string>();
            source.ReadStringList(offer.Categories);
            offer.Availability = source.ReadString();
            offer.GroupId = source.ReadString();
            offer.Price = source.ReadString();
            offer.SalePrice = source.ReadString();
            offer.Links = source.ReadParcelable(JClass.FromType(typeof(Links)).ClassLoader) as Links;
            offer.Images = new List<string>();
            source.ReadStringList(offer.Images);
            offer.Metadata = source.ReadString();
            offer.Sku = source.ReadString();
            offer.Score = source.ReadFloat();
            return offer;
        }

        public Java.Lang.Object[] NewArray(int size)
        {
            return new Offer[size];
        }
    }
}