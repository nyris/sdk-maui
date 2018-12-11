using System;
using System.Net.Http;
using JetBrains.Annotations;
using Nyris.Sdk.Network.API;
using Nyris.Sdk.Network.API.ImageMatching;
using Nyris.Sdk.Network.API.ObjectProposal;
using Nyris.Sdk.Network.API.TextSearch;
using Nyris.Sdk.Network.Service;
using Nyris.Sdk.Utils;
using Refit;

namespace Nyris.Sdk.Network
{
    public class ApiHelper : IApiHelper
    {
        private readonly ApiHeader _apiHeader;
        private string _apiKey;

        public ApiHelper([NotNull] string apiKey, Platform platform, bool isDebug)
        {
            _apiKey = apiKey;
            _apiHeader = new ApiHeader(apiKey, platform.ToString());

            var httpClient = isDebug
                ? new HttpClient(new HttpLoggingAndRetryHandler(isDebug))
                : new HttpClient();
            httpClient.BaseAddress = new Uri(Constants.DEFAULT_HOST_URL);
            httpClient.Timeout = TimeSpan.FromSeconds(Constants.DEFAULT_NETWORK_CONNECTION_TIMEOUT);

            var imageMatchingService = RestService.For<IImageMatchingService>(httpClient);
            ImageMatchingAPi = new ImageMatchingApi(imageMatchingService, _apiHeader);

            var objectProposalService = RestService.For<IObjectProposalService>(httpClient);
            ObjectProposalApi = new ObjectProposalApi(objectProposalService, _apiHeader);

            var offerTextSearchService = RestService.For<IOfferTextSearchService>(httpClient);
            OfferTextSearchApi = new OfferTextSearchApi(offerTextSearchService, _apiHeader);
        }

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

        public IOfferTextSearchApi OfferTextSearchApi { get; }
    }
}