using System.Net.Http.Headers;
using Nyris.Api.Api.RequestOptions;
using Nyris.Api.Utils;
using Refit;

namespace Nyris.Api;

/// <summary>
/// Multipart data wrapper for the nyris filters and image bytes sent with the API.
/// </summary>
internal sealed class ImageMatchingMultiPart : MultipartItem
{
    private readonly NyrisFilterOption _filters;
    private readonly byte[] _imageBytes;
    private readonly string _boundary;
    public ImageMatchingMultiPart(string fileName, string? contentType, NyrisFilterOption filters, byte[] imageBytes, string boundary) : base(fileName, contentType, "image")
    {
        _filters = filters;
        _imageBytes = imageBytes;
        _boundary = boundary;
    }
    
    /// <inheritdoc cref="MultipartItem.CreateContent" />
    protected override HttpContent CreateContent()
    {
        var imageMultiPart = new ByteArrayContent(_imageBytes);
        imageMultiPart.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
        {
            Name = Name,
            FileName = FileName
        };
        imageMultiPart.Headers.ContentType = new MediaTypeHeaderValue(ContentType);
        if (_filters.Filters.Count == 0)
        {
            return imageMultiPart;
        }
        
        var multipartContent = new MultipartContent("form-data", _boundary);
        
        // append the filters as multipart (StringContent)
        // example:
        // --a2f521f2-f7f0-4341-b831-f19c95393fa2
        // Content-Type: text/plain; charset=utf-8
        // Content-Disposition: form-data; name="filters[0].filterType"
        //
        // color
        //     --a2f521f2-f7f0-4341-b831-f19c95393fa2
        // Content-Type: text/plain; charset=utf-8
        // Content-Disposition: form-data; name="filters[0].filterValue[0]"
        //
        // red
        //     --a2f521f2-f7f0-4341-b831-f19c95393fa2
        // Content-Type: text/plain; charset=utf-8
        // Content-Disposition: form-data; name="filters[0].filterValue[1]"
        //
        // blue
        //     --a2f521f2-f7f0-4341-b831-f19c95393fa2--
        foreach (var (filter, index) in _filters.Filters.Select((value, i) => ( value, i )) )
        {
            filter.CreateContent(index, _boundary)
                .ForEach( multipartFilter => multipartContent.Add(multipartFilter));
        }
        
        _filters.Reset();
        multipartContent.Add(imageMultiPart);
        return multipartContent;
    }
    
    public async Task<string> RawContent()
    {
        var rawContent  = await CreateContent().ReadAsStringAsync();
        return rawContent;
    }
}
