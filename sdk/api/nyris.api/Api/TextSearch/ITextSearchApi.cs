using JetBrains.Annotations;
using Nyris.Api.Model;

namespace Nyris.Api.Api.TextSearch;

public interface ITextSearchApi : IMatchResultFormat<ITextSearchApi>
{
    [NotNull]
    IObservable<OfferResponseDto> Search([NotNull] string keyword);

    [NotNull]
    Task<OfferResponseDto> SearchAsync([NotNull] string keyword);
}