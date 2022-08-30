using System.Net.Http;
using JetBrains.Annotations;
using Nyris.Api.Api;
using Nyris.Api.Api.Feedback;
using Nyris.Api.Api.ImageMatching;
using Nyris.Api.Api.ObjectProposal;
using Nyris.Api.Api.Recommendation;
using Nyris.Api.Api.TextSearch;

namespace Nyris.Api
{
    public sealed class NyrisApi : INyrisApi
    {
        private readonly INyrisApi _helper;

        private NyrisApi([NotNull] string apiKey, Platform platform, HttpClientHandler httpClientHandler, bool isDebug)
        {
            _helper = new Helper(apiKey, platform, httpClientHandler, isDebug);
        }

        [NotNull]
        public static INyrisApi CreateInstance([NotNull] string apiKey, Platform platform, HttpClientHandler httpClientHandler = null, bool isDebug = false)
            => new NyrisApi(apiKey, platform, httpClientHandler, isDebug);


        public string ApiKey
        {
            get => _helper.ApiKey;
            set => _helper.ApiKey = value;
        }

        public IImageMatchingApi ImageMatching => _helper.ImageMatching;

        public IObjectProposalApi ObjectProposal => _helper.ObjectProposal;

        public ITextSearchApi TextSearch => _helper.TextSearch;

        public IRecommendationApi Recommendations => _helper.Recommendations;

        public IFeedbackApi Feedback => _helper.Feedback;
    }
}
