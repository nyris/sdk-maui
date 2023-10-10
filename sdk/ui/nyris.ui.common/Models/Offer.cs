namespace Nyris.UI.Common.Models;

public class Offer
{
    public Offer(string id, string title, string description, string descriptionLong, string brand,
        List<string> catalogNumbers, List<string> keywords, List<string> categories, string availability,
        string groupId, string price, string salePrice, Links links, List<string> images, string metadata, string sku,
        float score)
    {
        Id = id;
        Title = title;
        Description = description;
        DescriptionLong = descriptionLong;
        Brand = brand;
        CatalogNumbers = catalogNumbers;
        Keywords = keywords;
        Categories = categories;
        Availability = availability;
        GroupId = groupId;
        Price = price;
        SalePrice = salePrice;
        Links = links;
        Images = images;
        Metadata = metadata;
        Sku = sku;
        Score = score;
    }

    public string Id { get; }

    public string Title { get; }

    public string Description { get; }

    public string DescriptionLong { get; }

    public string Brand { get; }

    public List<string> CatalogNumbers { get; }

    public List<string> Keywords { get; }

    public List<string> Categories { get; }

    public string Availability { get; }

    public string GroupId { get; }

    public string Price { get; }

    public string SalePrice { get; }

    public Links Links { get; }

    public List<string> Images { get; }

    public string Metadata { get; }

    public string Sku { get; }

    public float Score { get; }

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