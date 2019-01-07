using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Nyris.Api.Model;
using Nyris.Api.Service;

namespace Nyris.Api.Api.Recommendation
{
    public class RecommendationApi : ApiBase, IRecommendationApi
    {
        [NotNull]
        private readonly IRecommendationService _service;
        private int _limit = OptionDefaults.DefaultLimit;

        /// <summary>
        /// Initializes a new instance of the <see cref="RecommendationApi"/> class.
        /// </summary>
        /// <param name="service">The recommendation service to use.</param>
        /// <param name="apiHeader">The HTTP headers.</param>
        internal RecommendationApi([NotNull] IRecommendationService service, [NotNull] ApiHeader apiHeader)
            : base(apiHeader)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        /// <inheritdoc cref="IMatchResultFormat{T}.OutputFormat"/>
        public IRecommendationApi OutputFormat(string outputFormat)
        {
            if (outputFormat != null)
            {
                ApiHeader.ResultFormat = outputFormat;
            }

            return this;
        }

        /// <inheritdoc cref="IMatchResultFormat{T}.Language"/>
        public IRecommendationApi Language(string language)
        {
            if (language != null)
            {
                ApiHeader.Language = language;
            }

            return this;
        }

        /// <inheritdoc cref="IMatchResultFormat{T}.Limit"/>
        public IRecommendationApi Limit(int limit)
        {
            _limit = limit <= 0 ? OptionDefaults.DefaultLimit : limit;
            return this;
        }

        /// <inheritdoc cref="IRecommendationApi.RecommendBySku"/>
        public IObservable<OfferResponseDto> RecommendBySku(string sku)
            => RecommendBySku<OfferResponseDto>(sku);

        /// <inheritdoc cref="IRecommendationApi.RecommendBySku{T}"/>
        public IObservable<T> RecommendBySku<T>(string sku)
        {
            return _service.GetOffersBySku<T>(accept: ApiHeader.ResultFormat,
                userAgent: ApiHeader.UserAgent,
                apiKey: ApiHeader.ApiKey,
                acceptLanguage: ApiHeader.Language,
                sku: sku);
        }

        /// <inheritdoc cref="IRecommendationApi.RecommendBySkuAsync"/>
        public Task<OfferResponseDto> RecommendBySkuAsync(string sku)
            => RecommendBySkuAsync<OfferResponseDto>(sku);

        /// <inheritdoc cref="IRecommendationApi.RecommendBySkuAsync{T}"/>
        public Task<T> RecommendBySkuAsync<T>(string sku)
        {
            return _service.GetOffersBySkuAsync<T>(accept: ApiHeader.ResultFormat,
                userAgent: ApiHeader.UserAgent,
                apiKey: ApiHeader.ApiKey,
                acceptLanguage: ApiHeader.Language,
                sku: sku);
        }

        /// <inheritdoc cref="ApiBase.BuildRequestOptions"/>
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
}
