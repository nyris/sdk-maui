using Nyris.Sdk.Network;
using Nyris.Sdk.Network.API.ImageMatching;
using Nyris.Sdk.Network.API.ObjectProposal;
using Nyris.Sdk.Utils;

namespace Nyris.Sdk
{
    public class NyrisApi : INyris
    {
        private readonly IApiHelper _apiHelper;

        public IImageMatchingApi ImageMatchingAPi => _apiHelper.ImageMatchingAPi;
        public IObjectProposalApi ObjectProposalApi => _apiHelper.ObjectProposalApi;

        private NyrisApi(string apiKey, Platform platform, bool isDebug) =>
            _apiHelper = new ApiHelper(apiKey, platform, isDebug);

        public string ApiKey
        {
            get => _apiHelper.ApiKey;
            set => _apiHelper.ApiKey = value;
        }
        
        public static INyris CreateInstance(string apiKey, Platform platform, bool isDebug = false) =>
            new NyrisApi(apiKey, platform, isDebug);
    }
}