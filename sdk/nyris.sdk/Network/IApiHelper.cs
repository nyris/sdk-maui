using Nyris.Sdk.Network.API.ImageMatching;
using Nyris.Sdk.Network.API.ObjectProposal;
using Nyris.Sdk.Network.API.TextSearch;

namespace Nyris.Sdk.Network
{
    public interface IApiHelper
    {
        string ApiKey { get; set; }

        IImageMatchingApi ImageMatchingAPi { get; }

        IObjectProposalApi ObjectProposalApi { get; }

        IOfferTextSearchApi OfferTextSearchApi { get; }
    }
}