using System;
using System.Linq;
using Newtonsoft.Json;
using Nyris.Sdk.Network.Model;
using Refit;

namespace Nyris.Sdk.Network.API
{
    public class Api
    {
        protected readonly ApiHeader _apiHeader;

        protected Api(ApiHeader apiHeader) =>
            _apiHeader = apiHeader;

        protected virtual string BuildXOptions()
        {
            return string.Empty;
        }

        protected T CastToNyrisResponse<T>(ApiResponse<string> apiResponse) where T : INyrisResponse
        {
            var requestCode = apiResponse.Headers.ToDictionary(l => l.Key, k => k.Value)["X-Matching-Request"].FirstOrDefault();
            var offerResponse = JsonConvert.DeserializeObject<OfferResponse>(apiResponse.Content);
            offerResponse.RequestCode = requestCode;
            if (typeof(T) == typeof(OfferResponse))
            {
                return (T) Convert.ChangeType(offerResponse, typeof(T));
            }
            var json = JsonConvert.SerializeObject(offerResponse);
            var jsonResponse = new JsonResponse(json);
            return (T) Convert.ChangeType(jsonResponse, typeof(T));
        }
    }
}