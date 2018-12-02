using Nyris.Sdk.Network.API;
using Nyris.Sdk.Network.API.ImageMatching;
using Nyris.Sdk.Network.Service;

namespace Nyris.Sdk.Network
{
    public interface IApiHelper
    {
        string ApiKey { get; set; }
        
        IImageMatchingApi ImageMatchingAPi { get; }
    }
}