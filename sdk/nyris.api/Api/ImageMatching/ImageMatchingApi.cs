using System;
using System.Net.Http;
using System.Reactive.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Nyris.Api.Api.RequestOptions;
using Nyris.Api.Model;
using Nyris.Api.Service;
using Nyris.Api.Utils;

namespace Nyris.Api.Api.ImageMatching
{
    internal sealed class ImageMatchingApi : ApiBase, IImageMatchingApi
    {
        private readonly IImageMatchingService _service;
        private int _limit = OptionDefaults.DefaultLimit;

        private readonly ExactOptions _exactOptions;
        private readonly SimilarityOptions _similarityOptions;
        private readonly OcrOptions _ocrOptions;
        private readonly RegroupOptions _regroupOptions;
        private readonly RecommendationModeOptions _recommendationModeOptions;
        private readonly CategoryPredictionOptions _categoryPredictionOptions;

        /// <summary>
        /// Initializes a new instance of the <see cref="ImageMatchingApi"/> class.
        /// </summary>
        /// <param name="service">The image matching service to use.</param>
        /// <param name="apiHeader">The HTTP headers.</param>
        internal ImageMatchingApi([NotNull] IImageMatchingService service, [NotNull] ApiHeader apiHeader)
            : base(apiHeader)
        {
            _service = service;

            _exactOptions = new ExactOptions();
            _similarityOptions = new SimilarityOptions();
            _ocrOptions = new OcrOptions();
            _regroupOptions = new RegroupOptions();
            _recommendationModeOptions = new RecommendationModeOptions();
            _categoryPredictionOptions = new CategoryPredictionOptions();
        }

        /// <inheritdoc cref="IMatchResultFormat{T}.OutputFormat"/>
        public IImageMatchingApi OutputFormat(string outputFormat)
        {
            if (outputFormat != null)
            {
                ApiHeader.ResultFormat = outputFormat;
            }
            return this;
        }

        /// <inheritdoc cref="IMatchResultFormat{T}.Language"/>
        public IImageMatchingApi Language(string language)
        {
            if (language != null)
            {
                ApiHeader.Language = language;
            }
            return this;
        }

        /// <inheritdoc cref="IMatchResultFormat{T}.Limit"/>
        public IImageMatchingApi Limit(int limit)
        {
            _limit = limit <= 0 ? OptionDefaults.DefaultLimit : limit;
            return this;
        }

        /// <inheritdoc cref="IImageMatching{T}.Exact"/>
        public IImageMatchingApi Exact(Action<ExactOptions> options = null)
        {
            if (options == null)
            {
                options = opt => { opt.Enabled = true; };
            }

            options(_exactOptions);
            return this;
        }

        /// <inheritdoc cref="IImageMatching{T}.Similarity"/>
        public IImageMatchingApi Similarity(Action<SimilarityOptions> options = null)
        {
            if (options == null)
            {
                options = opt => { opt.Enabled = true; };
            }

            options(_similarityOptions);
            return this;
        }

        /// <inheritdoc cref="IImageMatching{T}.Ocr"/>
        public IImageMatchingApi Ocr(Action<OcrOptions> options = null)
        {
            if (options == null)
            {
                options = opt => { opt.Enabled = true; };
            }

            options(_ocrOptions);
            return this;
        }

        /// <inheritdoc cref="IRegrouping{T}.Regroup"/>
        public IImageMatchingApi Regroup(Action<RegroupOptions> options = null)
        {
            if (options == null)
            {
                options = opt => { opt.Enabled = true; };
            }

            options(_regroupOptions);
            return this;
        }

        /// <inheritdoc cref="IImageMatching{T}.RecommendationMode"/>
        public IImageMatchingApi RecommendationMode(Action<RecommendationModeOptions> options = null)
        {
            if (options == null)
            {
                options = opt => { opt.Enabled = true; };
            }

            options(_recommendationModeOptions);
            return this;
        }

        /// <inheritdoc cref="IImageMatching{T}.CategoryPrediction"/>
        public IImageMatchingApi CategoryPrediction(Action<CategoryPredictionOptions> options = null)
        {
            if (options == null)
            {
                options = opt => { opt.Enabled = true; };
            }

            options(_categoryPredictionOptions);
            return this;
        }

        /// <inheritdoc cref="IImageMatchingApi.Match"/>
        public IObservable<OfferResponseDto> Match(byte[] image)
            => Match<OfferResponseDto>(image);

        /// <inheritdoc cref="IImageMatchingApi.Match{T}"/>
        public IObservable<T> Match<T>(byte[] image) where T : INyrisResponse
        {
            var byteContent = new ByteArrayContent(image);
            var xOptions = BuildRequestOptions();
            var obs1 = _service.Match(accept: ApiHeader.ResultFormat,
                userAgent: ApiHeader.UserAgent,
                apiKey: ApiHeader.ApiKey,
                acceptLanguage: ApiHeader.Language,
                xOptions: xOptions,
                contentType: "image/jpg",
                image: byteContent);

            var obs2 = Observable.Return(string.Empty);
            return obs1.CombineLatest(obs2, (apiResponse, dummy) => CastToNyrisResponse<T>(apiResponse));
        }

        /// <inheritdoc cref="IImageMatchingApi.MatchAsync"/>
        public Task<OfferResponseDto> MatchAsync(byte[] image)
            => MatchAsync<OfferResponseDto>(image);

        /// <inheritdoc cref="IImageMatchingApi.MatchAsync{T}"/>
        public async Task<T> MatchAsync<T>(byte[] image) where T : INyrisResponse
        {
            var byteContent = new ByteArrayContent(image);
            var xOptions = BuildRequestOptions();
            var apiResponse = await _service.MatchAsync(accept: ApiHeader.ResultFormat,
                userAgent: ApiHeader.UserAgent,
                apiKey: ApiHeader.ApiKey,
                acceptLanguage: ApiHeader.Language,
                xOptions: xOptions,
                contentType: "image/jpg",
                image: byteContent);

            return CastToNyrisResponse<T>(apiResponse);
        }

        /// <inheritdoc cref="ApiBase.BuildRequestOptions"/>
        protected override string BuildRequestOptions()
        {
            var xOptions = string.Empty;
            if (_exactOptions.Enabled && xOptions.IsEmpty()) xOptions = "exact";

            if (_similarityOptions.Enabled && xOptions.IsEmpty()) xOptions = "similarity";
            else if (_similarityOptions.Enabled) xOptions += " +similarity";

            if (_ocrOptions.Enabled && xOptions.IsEmpty()) xOptions = "ocr";
            else if (_ocrOptions.Enabled) xOptions += " +ocr";

            if (_similarityOptions.Enabled && _similarityOptions.Limit != OptionDefaults.UndefinedLimit)
            {
                xOptions += $" similarity.limit={_similarityOptions.Limit}";
            }

            if (_similarityOptions.Enabled && _similarityOptions.Threshold != OptionDefaults.UndefinedThreshold)
            {
                xOptions += $" similarity.threshold={_similarityOptions.Threshold}";
            }

            if (_regroupOptions.Enabled)
            {
                xOptions += " +regroup";
                if (_regroupOptions.Threshold != OptionDefaults.UndefinedThreshold)
                {
                    xOptions += $" regroup.threshold={_regroupOptions.Threshold}";
                }
            }

            if (_limit != OptionDefaults.DefaultLimit) xOptions += $" limit={_limit}";

            if (_recommendationModeOptions.Enabled) xOptions += " +recommendations";

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
            _exactOptions.Reset();
            _similarityOptions.Reset();
            _ocrOptions.Reset();
            _regroupOptions.Reset();
            _recommendationModeOptions.Reset();
            _categoryPredictionOptions.Reset();
        }
    }
}
