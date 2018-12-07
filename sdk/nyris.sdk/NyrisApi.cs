using Io.Nyris.Sdk.Network;
using Io.Nyris.Sdk.Network.API.ImageMatching;
using Io.Nyris.Sdk.Network.API.ObjectProposal;
using Io.Nyris.Sdk.Utils;

namespace Nyris.Sdk
{
    public class NyrisApi : INyris
    {
        private readonly IApiHelper _apiHelper;

        public string ApiKey
        {
            get => _apiHelper.ApiKey;
            set => _apiHelper.ApiKey = value;
        }

        public IImageMatchingApi ImageMatchingAPi => _apiHelper.ImageMatchingAPi;
        public IObjectProposalApi ObjectProposalApi => _apiHelper.ObjectProposalApi;

        private NyrisApi(string apiKey, Platform platform, bool isDebug) =>
            _apiHelper = new ApiHelper(apiKey, platform, isDebug);

        public static INyris CreateInstance(string apiKey, Platform platform, bool isDebug = false) =>
            new NyrisApi(apiKey, platform, isDebug);
    }
}