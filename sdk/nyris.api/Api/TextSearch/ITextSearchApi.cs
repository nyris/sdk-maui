using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Nyris.Api.Model;

namespace Nyris.Api.Api.TextSearch
{
    public interface ITextSearchApi : IMatchResultFormat<ITextSearchApi>, IRegrouping<ITextSearchApi>
    {
        [NotNull]
        IObservable<OfferResponseDto> Search([NotNull] string keyword);

        [NotNull]
        IObservable<T> Search<T>([NotNull] string keyword) where T : INyrisResponse;

        [NotNull]
        Task<OfferResponseDto> SearchAsync([NotNull] string keyword);

        [NotNull, ItemNotNull]
        Task<T> SearchAsync<T>([NotNull] string keyword) where T : INyrisResponse;
    }
}
