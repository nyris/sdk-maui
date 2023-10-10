using System.Net.Http.Headers;
using System.Net.Mime;
using System.Text;
using JetBrains.Annotations;
using Nyris.Api.Model;

namespace Nyris.Api.Utils;

internal static class Extensions
{
    [ContractAnnotation("value:null=>true")]
    public static bool IsEmpty([CanBeNull] this string value)
    {
        return string.IsNullOrEmpty(value);
    }
    
    public static List<HttpContent> CreateContent(this NyrisFilter nyrisFilter, int filterIndex, string boundary)
    {
        var contentList = new List<HttpContent>();
        // append the filters type as multipart, example:
        // Content-Type: text/plain; charset=utf-8
        // Content-Disposition: form-data; name="filters[0].filterType"
        //
        // color
        // --a2f521f2-f7f0-4341-b831-f19c95393fa2
        var filterTypeContent = new StringContent(nyrisFilter.Type);
        filterTypeContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
        {
            Name = $"filters[{filterIndex}].filterType"
        };
        contentList.Add(filterTypeContent);
        
        // append the filters values as multipart
        // example:
        // --a2f521f2-f7f0-4341-b831-f19c95393fa2
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
        foreach (var (item, index) in nyrisFilter.Values.Select((value, i) => ( value, i )) )
        {
            var filterValuesContent = new StringContent(nyrisFilter.Values[index], Encoding.UTF8, MediaTypeNames.Text.Plain);
            filterValuesContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
            {
                Name = $"filters[{filterIndex}].filterValue[{index}]"
            };
            contentList.Add(filterValuesContent);
        }

        return contentList;
    }
}