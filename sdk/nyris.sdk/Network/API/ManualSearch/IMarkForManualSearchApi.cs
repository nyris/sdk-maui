using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Nyris.Sdk.Network.API.ManualSearch
{
    public interface IMarkForManualSearchApi
    {
        IObservable<HttpResponseMessage> MarkOfferAsNotFound(string requestCode);
        
        Task<HttpResponseMessage> MarkOfferAsNotFoundAsync(string requestCode);
    }
}