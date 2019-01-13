using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Nyris.Api.Model;

namespace Nyris.Api.Api.Recommendation
{
    public interface IRecommendationApi : IMatchResultFormat<IRecommendationApi>
    {
        [NotNull]
        IObservable<OfferResponseDto> RecommendBySku([NotNull] string sku);

        [NotNull]
        IObservable<T> RecommendBySku<T>([NotNull] string sku);

        [NotNull]
        Task<OfferResponseDto> RecommendBySkuAsync([NotNull] string sku);

        [NotNull]
        Task<T> RecommendBySkuAsync<T>([NotNull] string sku);
    }
}
