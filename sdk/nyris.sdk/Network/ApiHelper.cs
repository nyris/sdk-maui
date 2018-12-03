using System.Net.Http;
using Nyris.Sdk.Network.API;
using Nyris.Sdk.Network.API.ImageMatching;
using Nyris.Sdk.Network.Service;
using Nyris.Sdk.Utils;
using Refit;

namespace Nyris.Sdk.Network
{
    public class ApiHelper : IApiHelper
    {
        public string ApiKey { get; set; }
        public IImageMatchingApi ImageMatchingAPi { get; }

        public ApiHelper(string apiKey, bool isDebug)
        {
            ApiKey = apiKey;
            var outputFormat = Constants.DEFAULT_OUTPUT_FORMAT;
            var language = Constants.DEFAULT_LANGUAGE;
            var sdkId = Constants.SDK_ID;
            var sdkVersion = "";
            var gitCommitHash = "";
            var platformVersion = "";

            var apiHeader = new ApiHeader(apiKey, sdkId, sdkVersion, gitCommitHash, platformVersion);
            var httpClient = new HttpClient(new HttpLoggingHandler()){ BaseAddress = Constants.DEFAULT_HOST_URL}; 
            var imageMatchingService = RestService.For<IImageMatchingService>(httpClient);
            ImageMatchingAPi = new ImageMatchingApi(outputFormat,
                language,
                imageMatchingService,
                apiHeader);
        }
    }
}