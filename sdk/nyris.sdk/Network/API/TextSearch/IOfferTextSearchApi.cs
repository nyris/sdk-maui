using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Nyris.Sdk.Network.API.XOptions;
using Nyris.Sdk.Network.Model;

namespace Nyris.Sdk.Network.API.TextSearch
{
    public interface IOfferTextSearchApi
    {
        IOfferTextSearchApi OutputFormat(string outputFormat);

        IOfferTextSearchApi Language(string language);

        IOfferTextSearchApi Limit(uint limit);

        IOfferTextSearchApi Regroup(Action<RegroupOptions> options = null);

        IObservable<OfferResponseDto> SearchOffers([NotNull] string keyword);

        IObservable<T> SearchOffers<T>([NotNull] string keyword) where T : INyrisResponse;

        Task<OfferResponseDto> SearchOffersAsync([NotNull] string keyword);

        Task<T> SearchOffersAsync<T>([NotNull] string keyword) where T : INyrisResponse;
    }
}