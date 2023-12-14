using System.Reactive.Linq;
using JetBrains.Annotations;
using Nyris.Api.Api.RequestOptions;
using Nyris.Api.Model;
using Nyris.Api.Service;
using Nyris.Api.Utils;

namespace Nyris.Api.Api.ImageMatching;

internal sealed class ImageMatchingApi : ApiBase, IImageMatchingApi
{
    private readonly NyrisFilterOption _filtersOptions;
    private readonly IImageMatchingService _service;
    private int _limit = OptionDefaults.DefaultLimit;
    private readonly CategoryPredictionOptions _categoryPredictionOptions;

    /// <summary>
    ///     Initializes a new instance of the <see cref="ImageMatchingApi" /> class.
    /// </summary>
    /// <param name="service">The image matching service to use.</param>
    /// <param name="apiHeader">The HTTP headers.</param>
    internal ImageMatchingApi([NotNull] IImageMatchingService service, [NotNull] ApiHeader apiHeader)
        : base(apiHeader)
    {
        _service = service;
        _filtersOptions = new NyrisFilterOption();
        _categoryPredictionOptions = new CategoryPredictionOptions();
    }

    /// <inheritdoc cref="IMatchResultFormat{T}.Language" />
    public IImageMatchingApi Language(string language)
    {
        if (language != null) ApiHeader.Language = language;
        return this;
    }

    /// <inheritdoc cref="IMatchResultFormat{T}.Limit" />
    public IImageMatchingApi Limit(int limit)
    {
        _limit = limit <= 0 ? OptionDefaults.DefaultLimit : limit;
        return this;
    }

    public IImageMatchingApi CategoryPrediction(Action<CategoryPredictionOptions>? options = null)
    {
        options ??= opt => { opt.Enabled = true; };

        options(_categoryPredictionOptions);
        return this;
    }

    /// <inheritdoc cref="IImageMatching{T}.Filters" />
    public IImageMatchingApi Filters(Action<NyrisFilterOption> options = null)
    {
        if (options == null) options = opt => { opt.Enabled = true; };

        options(_filtersOptions);
        return this;
    }

    /// <inheritdoc cref="IImageMatchingApi.Match" />
    public IObservable<OfferResponseDto> Match(byte[] image, string imageName = "image.jpg", string contentType = "image/jpeg")
    {
        var xOptions = BuildRequestOptions();
        var multipart = new ImageMatchingMultiPart(imageName, contentType, _filtersOptions, image, boundary:"nyris-xamarin-sdk-boundary");
        var obs1 = _service.Match(Constants.DefaultResultFormat,
            ApiHeader.UserAgent,
            ApiHeader.ApiKey,
            ApiHeader.Language,
            xOptions,
            multipart.ToContent());

        var obs2 = Observable.Return(string.Empty);
        return obs1.CombineLatest(obs2, (apiResponse, dummy) => 
            CastToNyrisResponse(apiResponse)
        );
    }

    /// <inheritdoc cref="IImageMatchingApi.MatchAsync" />
    public async Task<OfferResponseDto> MatchAsync(byte[] image, string imageName = "image.jpg", string contentType = "image/jpeg")
    {
        var xOptions = BuildRequestOptions();
        var multipart = new ImageMatchingMultiPart(imageName, contentType, _filtersOptions, image, boundary:"nyris-xamarin-sdk-boundary");

        var apiResponse = await _service.MatchAsync(Constants.DefaultResultFormat,
            ApiHeader.UserAgent,
            ApiHeader.ApiKey,
            ApiHeader.Language,
            xOptions,
            multipart.ToContent());

        return CastToNyrisResponse(apiResponse);
    }

    /// <inheritdoc cref="ApiBase.BuildRequestOptions" />
    protected override string BuildRequestOptions()
    {
        var xOptions = string.Empty;
        if (_limit != OptionDefaults.DefaultLimit) xOptions += $" limit={_limit}";
        
        if (_categoryPredictionOptions.Enabled)
        {
            xOptions += " +category-prediction";
            if (_categoryPredictionOptions.Limit != OptionDefaults.UndefinedLimit)
            {
                xOptions += $" category-prediction.limit={_categoryPredictionOptions.Limit}";
            }

            if (_categoryPredictionOptions.Threshold != OptionDefaults.UndefinedThreshold)
            {
                xOptions += $" category-prediction.threshold={_categoryPredictionOptions.Threshold}";
            }
        }
        
        Reset();
        return xOptions;
    }

    private void Reset()
    {
        _limit = OptionDefaults.DefaultLimit;
    }
}