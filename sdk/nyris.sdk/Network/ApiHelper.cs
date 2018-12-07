using System;
using System.Net.Http;
using Io.Nyris.Sdk.Network.API;
using Io.Nyris.Sdk.Network.API.ImageMatching;
using Io.Nyris.Sdk.Network.API.ObjectProposal;
using Io.Nyris.Sdk.Network.Service;
using Io.Nyris.Sdk.Utils;
using Refit;

namespace Io.Nyris.Sdk.Network
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
        public IObjectProposalApi ObjectProposalApi { get; }

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
            
            var objectProposalService = RestService.For<IObjectProposalService>(httpClient);
            ObjectProposalApi = new ObjectProposalApi(objectProposalService, _apiHeader);
        }
    }
}