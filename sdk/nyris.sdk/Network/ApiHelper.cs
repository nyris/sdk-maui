using System;
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
        private readonly ApiHeader _apiHeader;
        private string _apiKey;

        public string ApiKey
        {
            get => _apiKey;
            set
            {
                _apiKey = value;
                _apiHeader.ApiKey = value;
            }
        }

        public IImageMatchingApi ImageMatchingAPi { get; }

        public ApiHelper(string apiKey, Platform platform, bool isDebug)
        {
            var sdkId = Constants.SDK_ID;
            var sdkVersion = Constants.SDK_VERSION;
            _apiKey = apiKey;
            _apiHeader = new ApiHeader(apiKey, sdkId, sdkVersion, platform.ToString());

            var httpClient = isDebug
                ? new HttpClient(new HttpLoggingAndRetryHandler(sdkId, Constants.DEFAULT_HTTP_RETRY_COUNT, isDebug))
                : new HttpClient();
            httpClient.BaseAddress = new Uri(Constants.DEFAULT_HOST_URL);
            httpClient.Timeout = TimeSpan.FromSeconds(Constants.DEFAULT_NETWORK_CONNECTION_TIMEOUT);

            var imageMatchingService = RestService.For<IImageMatchingService>(httpClient);
            ImageMatchingAPi = new ImageMatchingApi(imageMatchingService, _apiHeader);
        }
    }
}