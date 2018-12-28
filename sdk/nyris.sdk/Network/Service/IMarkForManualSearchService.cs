using System;
using System.Net.Http;
using System.Threading.Tasks;
using Refit;

namespace Nyris.Sdk.Network.Service
{
    public interface IMarkForManualSearchService
    {
        [Post("/find/v1/manual/{requestCode}")]
        IObservable<HttpResponseMessage> MarkOfferAsNotFound([Header("User-Agent")] string userAgent,
            [Header("X-Api-Key")] string apiKey,
            string requestCode);
        
        [Post("/find/v1/manual/{requestCode}")]
        Task<HttpResponseMessage> MarkOfferAsNotFoundAsync([Header("User-Agent")] string userAgent,
            [Header("X-Api-Key")] string apiKey,
            string requestCode);
    }
}