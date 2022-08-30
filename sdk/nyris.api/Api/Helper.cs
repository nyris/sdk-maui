using System;
using System.Net.Http;
using JetBrains.Annotations;
using Nyris.Api.Api.Feedback;
using Nyris.Api.Api.ImageMatching;
using Nyris.Api.Api.ObjectProposal;
using Nyris.Api.Api.Recommendation;
using Nyris.Api.Api.TextSearch;
using Nyris.Api.Service;
using Nyris.Api.Utils;
using Refit;

namespace Nyris.Api.Api
{
    public class Helper : INyrisApi
    {
        private readonly ApiHeader _apiHeader;
        private string _apiKey;

        public Helper([NotNull] string apiKey, Platform platform, HttpClientHandler httpClientHandler, bool isDebug)
        {
            _apiKey = apiKey;
            _apiHeader = new ApiHeader(apiKey, platform.ToString());

            var httpHandler = isDebug
                ? new HttpLoggingAndRetryHandler(httpClientHandler)
                : new HttpRetryHandler(httpClientHandler);

            var httpClient = new HttpClient(httpHandler)
            {
                BaseAddress = new Uri(Constants.DefaultHostUrl),
                Timeout = TimeSpan.FromSeconds(Constants.DefaultNetworkConnectionTimeout)
            };

            var imageMatchingService = RestService.For<IImageMatchingService>(httpClient);
            ImageMatching = new ImageMatchingApi(imageMatchingService, _apiHeader);

            var objectProposalService = RestService.For<IObjectProposalService>(httpClient);
            ObjectProposal = new ObjectProposalApi(objectProposalService, _apiHeader);

            var offerTextSearchService = RestService.For<IOfferTextSearchService>(httpClient);
            TextSearch = new TextSearchApi(offerTextSearchService, _apiHeader);

            var recommendationService = RestService.For<IRecommendationService>(httpClient);
            Recommendations = new RecommendationApi(recommendationService, _apiHeader);

            var markForManualSearchService = RestService.For<IManualSearchService>(httpClient);
            Feedback = new FeedbackApi(markForManualSearchService, _apiHeader);
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

        public IImageMatchingApi ImageMatching { get; }

        public IObjectProposalApi ObjectProposal { get; }

        public ITextSearchApi TextSearch { get; }

        public IRecommendationApi Recommendations { get; }

        public IFeedbackApi Feedback { get; }
    }
}
