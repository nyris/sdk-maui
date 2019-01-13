using System;
using System.Net.Http;
using System.Threading.Tasks;
using Refit;

namespace Nyris.Api.Service
{
    internal interface IObjectProposalService
    {
        [Post("/find/v1/regions")]
        IObservable<T> ExtractObjects<T>([Header("Accept")] string accept,
            [Header("User-Agent")] string userAgent,
            [Header("X-Api-Key")] string apiKey,
            [Header("Content-Type")] string contentType,
            [Header("Content-Length")] string contentLength,
            ByteArrayContent image);

        [Post("/find/v1/regions")]
        Task<T> ExtractObjectsAsync<T>([Header("Accept")] string accept,
            [Header("User-Agent")] string userAgent,
            [Header("X-Api-Key")] string apiKey,
            [Header("Content-Type")] string contentType,
            [Header("Content-Length")] string contentLength,
            ByteArrayContent image);
    }
}