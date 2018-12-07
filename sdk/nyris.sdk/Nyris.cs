using Io.Nyris.Sdk.Network;
using Io.Nyris.Sdk.Network.API.ImageMatching;
using Io.Nyris.Sdk.Utils;

namespace Io.Nyris.Sdk
{
    public class Nyris : INyris
    {
        private readonly IApiHelper _apiHelper;

        public string ApiKey
        {
            get => _apiHelper.ApiKey;
            set => _apiHelper.ApiKey = value;
        }

        public IImageMatchingApi ImageMatchingAPi => _apiHelper.ImageMatchingAPi;

        private Nyris(string apiKey, Platform platform, bool isDebug)
        {
            _apiHelper = new ApiHelper(apiKey, platform, isDebug);
        }

        public static INyris CreateInstance(string apiKey, Platform platform, bool isDebug = false)
        {
            return new Nyris(apiKey, platform, isDebug);
        }
    }
}