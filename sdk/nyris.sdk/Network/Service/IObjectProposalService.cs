
using System;
using System.Net.Http;
using Refit;

namespace Io.Nyris.Sdk.Network.Service
{
    public interface IObjectProposalService
    {
        [Post("/find/v1/regions")]
        IObservable<T> ExtractObjects<T>([Header("Accept")] string accept,
            [Header("User-Agent")] string userAgent,
            [Header("X-Api-Key")] string apiKey,
            [Header("Content-Type")] string contentType,
            [Header("Content-Length")] string contentLength,
            ByteArrayContent image);
    }
}