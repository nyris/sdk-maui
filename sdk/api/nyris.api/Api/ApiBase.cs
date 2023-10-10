using System.Diagnostics;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Nyris.Api.Model;
using Refit;

namespace Nyris.Api.Api;

/// <summary>
///     Base class for API implementations.
/// </summary>
public abstract class ApiBase
{
    protected ApiBase([NotNull] ApiHeader apiHeader)
    {
        ApiHeader = apiHeader;
    }

    [NotNull] protected ApiHeader ApiHeader { get; }

    [NotNull]
    protected virtual string BuildRequestOptions()
    {
        return string.Empty;
    }

    protected static OfferResponseDto CastToNyrisResponse([NotNull] ApiResponse<string> apiResponse)
    {
        if (!apiResponse.IsSuccessStatusCode)
        {
            var definition = new
            {
                fault = new
                {
                    faultstring = ""
                }
            };
            var error = JsonConvert.DeserializeAnonymousType(apiResponse.Error.Content, definition);
            throw new ApiException($"{error?.fault.faultstring} \nHTTP Error code: {(int)apiResponse.StatusCode}");
        }

        // When default output format is applied and client wants to cast OfferResponseDto.
        // Note that according to RFC 2616, HTTP headers are case insensitive.
        var requestCode = apiResponse.Headers?.GetValues("X-Matching-Request")?.FirstOrDefault();
        OfferResponseDto offerResponse = null;
        try
        {
            offerResponse = JsonConvert.DeserializeObject<OfferResponseDto>(apiResponse.Content);
            //TODO: add safety check for null
            offerResponse.RequestCode = requestCode;
            return (OfferResponseDto)Convert.ChangeType(offerResponse, typeof(OfferResponseDto));
        }
        catch (Exception e)
        {
            // ignored
            Debug.WriteLine($"Error while deserializing response: {e.Message}", "Nyris.Api");
            throw new ApiException($"rror while deserializing response: {e.Message}");
        }
    }
}