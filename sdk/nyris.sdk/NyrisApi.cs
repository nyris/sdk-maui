using JetBrains.Annotations;
using Nyris.Sdk.Network;
using Nyris.Sdk.Network.API.ImageMatching;
using Nyris.Sdk.Network.API.ManualSearch;
using Nyris.Sdk.Network.API.ObjectProposal;
using Nyris.Sdk.Network.API.Recommendation;
using Nyris.Sdk.Network.API.TextSearch;
using Nyris.Sdk.Utils;

namespace Nyris.Sdk
{
    public class NyrisApi : INyris
    {
        private readonly INyris _apiHelper;
        private NyrisApi(string apiKey, Platform platform, bool isDebug) =>
            _apiHelper = new ApiHelper(apiKey, platform, isDebug);

        public static INyris CreateInstance([NotNull] string apiKey, Platform platform, bool isDebug = false) =>
            new NyrisApi(apiKey, platform, isDebug);


        [NotNull]
        public string ApiKey
        {
            get => _apiHelper.ApiKey;
            set => _apiHelper.ApiKey = value;
        }
        
        public IImageMatchingApi ImageMatchingAPi => _apiHelper.ImageMatchingAPi;
        
        public IObjectProposalApi ObjectProposalApi => _apiHelper.ObjectProposalApi;
        
        public IOfferTextSearchApi OfferTextSearchApi => _apiHelper.OfferTextSearchApi;
        
        public IRecommendationApi RecommendationApi => _apiHelper.RecommendationApi;
        
        public IMarkForManualSearchApi MarkForManualSearchApi => _apiHelper.MarkForManualSearchApi;
    }
}