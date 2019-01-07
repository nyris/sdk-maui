using System;
using System.Linq;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Nyris.Api.Model;
using Refit;

namespace Nyris.Api.Api
{
    /// <summary>
    /// Base class for API implementations.
    /// </summary>
    public abstract class ApiBase
    {
        protected ApiBase([NotNull] ApiHeader apiHeader)
        {
            ApiHeader = apiHeader;
        }

        [NotNull]
        protected ApiHeader ApiHeader { get; }

        [NotNull]
        protected virtual string BuildRequestOptions()
        {
            return string.Empty;
        }

        protected static T CastToNyrisResponse<T>([NotNull] ApiResponse<string> apiResponse)
            where T : INyrisResponse
        {
            // When default output format is applied and client want to cast OfferResponseDto.
            var requestCode = apiResponse.Headers.ToDictionary(l => l.Key, k => k.Value)["X-Matching-Request"]
                .FirstOrDefault();
            OfferResponseDto offerResponse = null;
            try
            {
                offerResponse = JsonConvert.DeserializeObject<OfferResponseDto>(apiResponse.Content);
                offerResponse.RequestCode = requestCode;
                if (typeof(T) == typeof(OfferResponseDto))
                {
                    return (T) Convert.ChangeType(offerResponse, typeof(T));
                }
            }
            catch
            {
                // ignored
            }

            // When default output format is not applied and client want to cast to his format.
            if (offerResponse == null && typeof(T) == typeof(OfferResponseDto))
            {
                throw new InvalidCastException("Can not cast response to OfferResponseDto. " +
                                               "Please check your output format or switch casting to JsonResponseDto.");
            }

            // When default output format is applied and client want to cast to JsonResponseDto.
            if (offerResponse != null && typeof(T) == typeof(JsonResponseDto))
            {
                var json = JsonConvert.SerializeObject(offerResponse);
                return (T) Convert.ChangeType(new JsonResponseDto(json), typeof(T));
            }

            // When default output format is not applied and client want to cast to JsonResponseDto.
            var response = new
            {
                RequestCode = requestCode,
                Result = apiResponse.Content
            };
            var jsonObj = JsonConvert.SerializeObject(response);
            return (T) Convert.ChangeType(new JsonResponseDto(jsonObj), typeof(T));
        }
    }
}
