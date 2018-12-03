using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Nyris.Sdk.Network.Model;
using Refit;

namespace Nyris.Sdk.Network.Service
{
    public interface IImageMatchingService
    {
        [Post("/find/v1")]
        [Headers("X-Api-Key:{headers}",
            "User-Agent:{headers}")]
        Task<OfferResponseBody> Match(Dictionary<string, string> headers, StreamPart image);
    }
}