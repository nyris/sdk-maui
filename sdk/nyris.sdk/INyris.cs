using Nyris.Sdk.Network;
using Nyris.Sdk.Network.API;
using Nyris.Sdk.Network.API.ImageMatching;

namespace Nyris.Sdk
{
    public abstract class INyris
    {
        string ApiKey { get; set; }

        IImageMatchingApi ImageMatchingAPi { get; }

        public static INyris CreateInstance(string apiKey, bool isDebug = false)
        {
            return new Nyris(apiKey, isDebug);
        }
    }
}