using Refit;

namespace Nyris.Api.Service;

internal interface IImageMatchingService
{
    [Multipart("nyris-xamarin-sdk-boundary")]
    [Post("/find/v1.1")]
    IObservable<ApiResponse<string>> Match([Header("Accept")] string accept,
        [Header("User-Agent")] string userAgent,
        [Header("X-Api-Key")] string apiKey,
        [Header("Accept-Language")] string acceptLanguage,
        [Header("X-Options")] string xOptions,
        HttpContent matchingMultiPart);

    [Multipart("nyris-xamarin-sdk-boundary")]
    [Post("/find/v1.1")]
    Task<ApiResponse<string>> MatchAsync([Header("Accept")] string accept,
        [Header("User-Agent")] string userAgent,
        [Header("X-Api-Key")] string apiKey,
        [Header("Accept-Language")] string acceptLanguage,
        [Header("X-Options")] string xOptions,
        HttpContent matchingMultiPart);
}