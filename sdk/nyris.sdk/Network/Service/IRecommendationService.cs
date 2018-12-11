using System;
using System.Net.Http;
using System.Threading.Tasks;
using Refit;

namespace Nyris.Sdk.Network.Service
{
    public interface IRecommendationService
    {
        [Get("/recommend/v1/{sku}")]
        IObservable<T> GetOffersBySku<T>([Header("Accept")] string accept,
            [Header("User-Agent")] string userAgent,
            [Header("X-Api-Key")] string apiKey,
            [Header("Accept-Language")] string acceptLanguage,
            string sku);

        [Get("/recommend/v1/{sku}")]
        Task<T> GetOffersBySkuAsync<T>([Header("Accept")] string accept,
            [Header("User-Agent")] string userAgent,
            [Header("X-Api-Key")] string apiKey,
            [Header("Accept-Language")] string acceptLanguage,
            string sku);
    }
}