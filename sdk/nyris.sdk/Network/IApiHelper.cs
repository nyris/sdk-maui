using Io.Nyris.Sdk.Network.API.ImageMatching;

namespace Io.Nyris.Sdk.Network
{
    public interface IApiHelper
    {
        string ApiKey { get; set; }
        
        IImageMatchingApi ImageMatchingAPi { get; }
    }
}