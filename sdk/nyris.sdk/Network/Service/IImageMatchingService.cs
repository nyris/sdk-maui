using System;
using System.Net.Http;
using System.Threading.Tasks;
using Refit;

namespace Nyris.Sdk.Network.Service
{
    public interface IImageMatchingService
    {
        [Post("/find/v1")]
        IObservable<T> Match<T>([Header("Accept")] string accept,
            [Header("User-Agent")] string userAgent,
            [Header("X-Api-Key")] string apiKey,
            [Header("Accept-Language")] string acceptLanguage,
            [Header("X-Options")] string xOptions,
            [Header("Content-Type")] string contentType,
            [Header("Content-Length")] string contentLength,
            ByteArrayContent image);
        
        [Post("/find/v1")]
        Task<T> MatchAsync<T>([Header("Accept")] string accept,
            [Header("User-Agent")] string userAgent,
            [Header("X-Api-Key")] string apiKey,
            [Header("Accept-Language")] string acceptLanguage,
            [Header("X-Options")] string xOptions,
            [Header("Content-Type")] string contentType,
            [Header("Content-Length")] string contentLength,
            ByteArrayContent image);
    }
}