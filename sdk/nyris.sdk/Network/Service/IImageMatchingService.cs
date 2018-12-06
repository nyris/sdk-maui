using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Nyris.Sdk.Network.Model;
using Refit;

namespace Nyris.Sdk.Network.Service
{
    public interface IImageMatchingService
    {
        [Post("/find/v1")]
        IObservable<OfferResponse> Match([Header("Accept")] string accept,
            [Header("Accept-Language")] string acceptLanguage,
            [Header("X-Options")] string xOptions,
            [Header("Content-Type")] string contentType,
            [Header("Content-Length")] string contentLength,
            ByteArrayContent image);
    }
}