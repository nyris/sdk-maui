using Nyris.Sdk.Network;
using Nyris.Sdk.Network.API;
using Nyris.Sdk.Network.API.ImageMatching;

namespace Nyris.Sdk
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

        private Nyris(string apiKey, bool isDebug)
        {
            _apiHelper = new ApiHelper(apiKey, isDebug);
        }
        
        
        public static INyris CreateInstance(string apiKey, bool isDebug = false)
        {
            return new Nyris(apiKey, isDebug);
        }
    }
}