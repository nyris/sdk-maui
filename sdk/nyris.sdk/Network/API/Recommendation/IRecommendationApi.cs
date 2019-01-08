using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Nyris.Sdk.Network.Model;

namespace Nyris.Sdk.Network.API.Recommendation
{
    public interface IRecommendationApi
    {
        IRecommendationApi OutputFormat(string outputFormat);

        IRecommendationApi Language(string language);

        IRecommendationApi Limit(uint limit);

        IObservable<OfferResponseDto> GetOffersBySku([NotNull] string sku);

        IObservable<T> GetOffersBySku<T>([NotNull] string sku);

        Task<OfferResponseDto> GetOffersBySkuAsync([NotNull] string sku);

        Task<T> GetOffersBySkuAsync<T>([NotNull] string sku);
    }
}