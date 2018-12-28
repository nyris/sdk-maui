using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Nyris.Sdk.Network.Service;

namespace Nyris.Sdk.Network.API.ManualSearch
{
    public class MarkForManualSearchApi : Api, IMarkForManualSearchApi
    {
        private readonly IMarkForManualSearchService _markForManualSearchService;
        
        internal MarkForManualSearchApi(IMarkForManualSearchService markForManualSearchService, ApiHeader apiHeader) :
            base(apiHeader)
        {
            _markForManualSearchService = markForManualSearchService;
        }
        
        public IObservable<HttpResponseMessage> MarkOfferAsNotFound(string requestCode)
        {
            return _markForManualSearchService.MarkOfferAsNotFound(userAgent: _apiHeader.UserAgent,
                apiKey: _apiHeader.ApiKey,
                requestCode: requestCode);
        }

        public Task<HttpResponseMessage> MarkOfferAsNotFoundAsync(string requestCode)
        {
            return _markForManualSearchService.MarkOfferAsNotFoundAsync(userAgent: _apiHeader.UserAgent,
                apiKey: _apiHeader.ApiKey,
                requestCode: requestCode);
        }
    }
}