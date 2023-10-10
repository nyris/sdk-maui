using System.Diagnostics;
using System.Net.Http.Headers;
using JetBrains.Annotations;

namespace Nyris.Api.Utils;

internal class HttpLoggingAndRetryHandler : HttpRetryHandler
{
    private const string Id = Constants.SdkId;
    private readonly string[] _types = { "html", "text", "xml", "json", "txt", "x-www-form-urlencoded" };

    public HttpLoggingAndRetryHandler(HttpClientHandler httpClientHandler) : base(httpClientHandler)
    {
    }

    private bool IsTextBasedContentType([NotNull] HttpHeaders headers)
    {
        if (!headers.TryGetValues("Content-Type", out var values)) return false;

        var header = string.Join(" ", values).ToLowerInvariant();
        return _types.Any(t => header.Contains(t));
    }

    protected override async Task OnRequestPreparingAsync(HttpRequestMessage request)
    {
        var msg = $"[{Id} - Request]";

        Debug.WriteLine($"{msg}========Start==========");
        Debug.WriteLine(
            $"{msg} {request.Method} {request.RequestUri.PathAndQuery} {request.RequestUri.Scheme}/{request.Version}");
        Debug.WriteLine($"{msg} Host: {request.RequestUri.Scheme}://{request.RequestUri.Host}");

        foreach (var header in request.Headers)
            Debug.WriteLine($"{msg} {header.Key}: {string.Join(", ", header.Value)}");

        if (request.Content == null) return;

        foreach (var header in request.Content.Headers)
            Debug.WriteLine($"{msg} {header.Key}: {string.Join(", ", header.Value)}");

        if (!(request.Content is StringContent) && !IsTextBasedContentType(request.Headers) &&
            !IsTextBasedContentType(request.Content.Headers))
            return;

        var result = await request.Content.ReadAsStringAsync();
        Debug.WriteLine($"{msg} Content:");
        Debug.WriteLine($"{msg} {string.Join("", result.Take(255))}...");
    }

    protected override async Task OnRequestFinishedAsync(DateTime end, DateTime start,
        HttpRequestMessage request,
        HttpResponseMessage response)
    {
        var msg = $"[{Id} - Request]";
        Debug.WriteLine($"{msg} Duration: {end - start}");
        Debug.WriteLine($"{msg}==========End==========");

        Debug.WriteLine("");
        msg = $"[{Id} - Response]";
        Debug.WriteLine($"{msg}=========Start=========");

        Debug.WriteLine(
            $"{msg} {request.RequestUri.Scheme.ToUpper()}/{response.Version} {(int)response.StatusCode} {response.ReasonPhrase}");

        foreach (var header in response.Headers)
            Debug.WriteLine($"{msg} {header.Key}: {string.Join(", ", header.Value)}");

        Debug.WriteLine($"{msg}=========Request content=========");
        var requestContent = await request.Content.ReadAsStringAsync();
        Debug.WriteLine($"{msg} Request content: \n");
        Debug.WriteLine($"{requestContent}");
        Debug.WriteLine("\n");
        

        
        if (response.Content != null)
        {
            foreach (var header in response.Content.Headers)
                Debug.WriteLine($"{msg} {header.Key}: {string.Join(", ", header.Value)}");

            if (response.Content is StringContent || IsTextBasedContentType(response.Headers) ||
                IsTextBasedContentType(response.Content.Headers))
            {
                start = DateTime.Now;
                var result = await response.Content.ReadAsStringAsync();
                end = DateTime.Now;

                Debug.WriteLine($"{msg} Content:");
                Debug.WriteLine($"{msg} {string.Join("", result.Take(255))}...");
                Debug.WriteLine($"{msg} Duration: {end - start}");
            }
        }

        Debug.WriteLine($"{msg}==========End==========");
    }
}