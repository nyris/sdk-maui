using Refit;

namespace Nyris.Api.Service;

public interface IManualSearchService
{
    [Post("/find/v1/manual/{requestCode}")]
    IObservable<HttpResponseMessage> MarkAsNotFound([Header("User-Agent")] string userAgent,
        [Header("X-Api-Key")] string apiKey,
        string requestCode);

    [Post("/find/v1/manual/{requestCode}")]
    Task<HttpResponseMessage> MarkAsNotFoundAsync([Header("User-Agent")] string userAgent,
        [Header("X-Api-Key")] string apiKey,
        string requestCode);
}