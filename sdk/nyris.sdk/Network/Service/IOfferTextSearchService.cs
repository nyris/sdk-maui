using System;
using System.Net.Http;
using System.Threading.Tasks;
using Refit;

namespace Nyris.Sdk.Network.Service
{
    internal interface IOfferTextSearchService
    {
        [Post("/find/v1/text")]
        IObservable<T> SearchOffers<T>([Header("Accept")] string accept,
            [Header("User-Agent")] string userAgent,
            [Header("X-Api-Key")] string apiKey,
            [Header("Accept-Language")] string acceptLanguage,
            [Header("X-Options")] string xOptions,
            [Body] StringContent stringContent);

        [Post("/find/v1/text")]
        Task<T> SearchOffersAsync<T>([Header("Accept")] string accept,
            [Header("User-Agent")] string userAgent,
            [Header("X-Api-Key")] string apiKey,
            [Header("Accept-Language")] string acceptLanguage,
            [Header("X-Options")] string xOptions,
            [Body] StringContent stringContent);
    }
}