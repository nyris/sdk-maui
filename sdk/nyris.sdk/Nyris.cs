using Nyris.Sdk.Network;
using Nyris.Sdk.Network.API;
using Nyris.Sdk.Network.API.ImageMatching;

namespace Nyris.Sdk
{
    internal class Nyris : INyris
    {
        private readonly IApiHelper _apiHelper;

        public string ApiKey
        {
            get => _apiHelper.ApiKey;
            set => _apiHelper.ApiKey = value;
        }

        public IImageMatchingApi ImageMatchingAPi => _apiHelper.ImageMatchingAPi;

        public Nyris(string apiKey, bool isDebug)
        {
            _apiHelper = new ApiHelper(apiKey, isDebug);
        }
    }
}