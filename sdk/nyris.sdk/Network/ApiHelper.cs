using Nyris.Sdk.Network.API;
using Nyris.Sdk.Network.API.ImageMatching;
using Nyris.Sdk.Network.Service;

namespace Nyris.Sdk.Network
{
    public class ApiHelper : IApiHelper
    {
        public string ApiKey { get; set; }
        public IImageMatchingApi ImageMatchingAPi { get; }

        public ApiHelper(string apiKey, bool isDebug)
        {
            ApiKey = apiKey;
            ImageMatchingAPi = new ImageMatchingApi();
        }
    }
}