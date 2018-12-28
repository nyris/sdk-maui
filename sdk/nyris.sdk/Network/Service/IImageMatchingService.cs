using System;
using System.Net.Http;
using System.Threading.Tasks;
using Refit;

namespace Nyris.Sdk.Network.Service
{
    internal interface IImageMatchingService
    {
        [Post("/find/v1")]
        IObservable<ApiResponse<string>> Match([Header("Accept")] string accept,
            [Header("User-Agent")] string userAgent,
            [Header("X-Api-Key")] string apiKey,
            [Header("Accept-Language")] string acceptLanguage,
            [Header("X-Options")] string xOptions,
            [Header("Content-Type")] string contentType,
            [Body] ByteArrayContent image);

        [Post("/find/v1")]
        Task<ApiResponse<string>> MatchAsync([Header("Accept")] string accept,
            [Header("User-Agent")] string userAgent,
            [Header("X-Api-Key")] string apiKey,
            [Header("Accept-Language")] string acceptLanguage,
            [Header("X-Options")] string xOptions,
            [Header("Content-Type")] string contentType,
            [Body] ByteArrayContent image);
    }
}