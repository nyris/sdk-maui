using System.Reactive.Linq;
using System.Text;
using JetBrains.Annotations;
using Nyris.Api.Api.RequestOptions;
using Nyris.Api.Model;
using Nyris.Api.Service;
using Nyris.Api.Utils;

namespace Nyris.Api.Api.TextSearch;

internal class TextSearchApi : ApiBase, ITextSearchApi
{
    private readonly IOfferTextSearchService _offerTextSearchService;

    private int _limit = OptionDefaults.DefaultLimit;

    /// <summary>
    ///     Initializes a new instance of the <see cref="TextSearchApi" /> class.
    /// </summary>
    /// <param name="service">The text search service to use.</param>
    /// <param name="apiHeader">The HTTP headers.</param>
    internal TextSearchApi([NotNull] IOfferTextSearchService service, [NotNull] ApiHeader apiHeader)
        : base(apiHeader)
    {
        _offerTextSearchService = service ?? throw new ArgumentNullException(nameof(service));
    }

    /// <inheritdoc cref="IMatchResultFormat{T}.Language" />
    public ITextSearchApi Language(string language)
    {
        if (language != null) ApiHeader.Language = language;

        return this;
    }

    /// <inheritdoc cref="IMatchResultFormat{T}.Limit" />
    public ITextSearchApi Limit(int limit)
    {
        _limit = limit <= 0 ? OptionDefaults.DefaultLimit : limit;
        return this;
    }

    /// <inheritdoc cref="ITextSearchApi.Search" />
    public IObservable<OfferResponseDto> Search(string keyword)
    {
        var xOptions = BuildRequestOptions();
        var stringContent = new StringContent(keyword, Encoding.UTF8, "text/plain");
        var obs1 = _offerTextSearchService.SearchOffers(Constants.DefaultResultFormat,
            ApiHeader.UserAgent,
            ApiHeader.ApiKey,
            ApiHeader.Language,
            xOptions,
            stringContent);

        var obs2 = Observable.Return(string.Empty);
        return obs1.CombineLatest(obs2, (apiResponse, dummy) => CastToNyrisResponse(apiResponse));
    }

    /// <inheritdoc cref="ITextSearchApi.SearchAsync" />
    public async Task<OfferResponseDto> SearchAsync(string keyword)
    {
        var xOptions = BuildRequestOptions();
        var stringContent = new StringContent(keyword, Encoding.UTF8, "text/plain");
        var apiResponse = await _offerTextSearchService.SearchOffersAsync(Constants.DefaultResultFormat,
            ApiHeader.UserAgent,
            ApiHeader.ApiKey,
            ApiHeader.Language,
            xOptions,
            stringContent);

        return CastToNyrisResponse(apiResponse);
    }

    /// <inheritdoc cref="ApiBase.BuildRequestOptions" />
    protected override string BuildRequestOptions()
    {
        var xOptions = string.Empty;
        if (_limit != OptionDefaults.DefaultLimit) xOptions += $" limit={_limit}";
        Reset();
        return xOptions;
    }

    private void Reset()
    {
        _limit = OptionDefaults.DefaultLimit;
    }
}