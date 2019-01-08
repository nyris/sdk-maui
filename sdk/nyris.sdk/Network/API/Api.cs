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
            // When default outputformat is applied and client want to cast OfferResponseDto.
            var requestCode = apiResponse.Headers.ToDictionary(l => l.Key, k => k.Value)["X-Matching-Request"].FirstOrDefault();
            OfferResponseDto offerResponse = null;
            try
            {
                offerResponse = JsonConvert.DeserializeObject<OfferResponseDto>(apiResponse.Content);
                offerResponse.RequestCode = requestCode;
                if (typeof(T) == typeof(OfferResponseDto))
                {
                    return (T)Convert.ChangeType(offerResponse, typeof(T));
                }
            }
            catch
            {
            }

            // When default outputformat is not applied and client want to cast to his format.
            if (offerResponse == null && typeof(T) == typeof(OfferResponseDto))
            {
                throw new InvalidCastException("Can not cast response to OfferResponseDto, please check your Outpuformat or switch casting to JsonResponseDto");
            }

            // When default outputformat is applied and client want to cast to JsonResponseDto.
            if (offerResponse != null && typeof(T) == typeof(JsonResponseDto))
            {
                var json = JsonConvert.SerializeObject(offerResponse);
                var jsonResponse = new JsonResponseDto(json);
                return (T)Convert.ChangeType(jsonResponse, typeof(T));
            }

            // When default outputformat is not applied and client want to cast to JsonResponseDto.
            var response = new
            {
                RequestCode = requestCode,
                Result = apiResponse.Content
            };
            var jsonObj = JsonConvert.SerializeObject(response);
            var jsponReponse = new JsonResponseDto(jsonObj);
            return (T)Convert.ChangeType(jsponReponse, typeof(T));
        }
    }
}