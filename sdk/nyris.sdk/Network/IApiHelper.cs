using Nyris.Sdk.Network.API.ImageMatching;
using Nyris.Sdk.Network.API.ObjectProposal;

namespace Nyris.Sdk.Network
{
    public interface IApiHelper
    {
        string ApiKey { get; set; }
        
        IImageMatchingApi ImageMatchingAPi { get; }
        
        IObjectProposalApi ObjectProposalApi { get; }
    }
}