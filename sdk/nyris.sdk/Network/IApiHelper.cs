using Io.Nyris.Sdk.Network.API.ImageMatching;
using Io.Nyris.Sdk.Network.API.ObjectProposal;

namespace Io.Nyris.Sdk.Network
{
    public interface IApiHelper
    {
        string ApiKey { get; set; }
        
        IImageMatchingApi ImageMatchingAPi { get; }
        
        IObjectProposalApi ObjectProposalApi { get; }
    }
}