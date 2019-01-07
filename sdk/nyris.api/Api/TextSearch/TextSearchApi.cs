using System;
using System.Net.Http;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Nyris.Api.Api.RequestOptions;
using Nyris.Api.Model;
using Nyris.Api.Service;

namespace Nyris.Api.Api.TextSearch
{
    internal class TextSearchApi : ApiBase, ITextSearchApi
    {
        private readonly IOfferTextSearchService _offerTextSearchService;
        private int _limit = OptionDefaults.DefaultLimit;

        private readonly RegroupOptions _regroupOptions = new RegroupOptions();

        /// <summary>
        /// Initializes a new instance of the <see cref="TextSearchApi"/> class.
        /// </summary>
        /// <param name="service">The text search service to use.</param>
        /// <param name="apiHeader">The HTTP headers.</param>
        internal TextSearchApi([NotNull] IOfferTextSearchService service, [NotNull] ApiHeader apiHeader)
            : base(apiHeader)
        {
            _offerTextSearchService = service ?? throw new ArgumentNullException(nameof(service));
        }

        /// <inheritdoc cref="IMatchResultFormat{T}.OutputFormat"/>
        public ITextSearchApi OutputFormat(string outputFormat)
        {
            if (outputFormat != null)
            {
                ApiHeader.ResultFormat = outputFormat;
            }

            return this;
        }

        /// <inheritdoc cref="IMatchResultFormat{T}.Language"/>
        public ITextSearchApi Language(string language)
        {
            if (language != null)
            {
                ApiHeader.Language = language;
            }

            return this;
        }

        /// <inheritdoc cref="IMatchResultFormat{T}.Limit"/>
        public ITextSearchApi Limit(int limit)
        {
            _limit = limit <= 0 ? OptionDefaults.DefaultLimit : limit;
            return this;
        }

        /// <inheritdoc cref="IRegrouping{T}.Regroup"/>
        public ITextSearchApi Regroup(Action<RegroupOptions> options = null)
        {
            if (options == null)
            {
                options = opt => { opt.Enabled = false; };
            }

            options(_regroupOptions);
            return this;
        }

        /// <inheritdoc cref="ITextSearchApi.Search"/>
        public IObservable<OfferResponseDto> Search(string keyword)
            => Search<OfferResponseDto>(keyword);

        /// <inheritdoc cref="ITextSearchApi.Search{T}"/>
        public IObservable<T> Search<T>(string keyword) where T : INyrisResponse
        {
            var xOptions = BuildRequestOptions();
            var stringContent = new StringContent(keyword, Encoding.UTF8, "text/plain");
            var obs1 = _offerTextSearchService.SearchOffers(accept: ApiHeader.ResultFormat,
                userAgent: ApiHeader.UserAgent,
                apiKey: ApiHeader.ApiKey,
                acceptLanguage: ApiHeader.Language,
                xOptions: xOptions,
                stringContent: stringContent);

            var obs2 = Observable.Return(string.Empty);
            return obs1.CombineLatest(obs2, (apiResponse, dummy) => CastToNyrisResponse<T>(apiResponse));
        }

        /// <inheritdoc cref="ITextSearchApi.SearchAsync"/>
        public Task<OfferResponseDto> SearchAsync(string keyword)
            => SearchAsync<OfferResponseDto>(keyword);

        /// <inheritdoc cref="ITextSearchApi.SearchAsync{T}"/>
        public async Task<T> SearchAsync<T>(string keyword) where T : INyrisResponse
        {
            var xOptions = BuildRequestOptions();
            var stringContent = new StringContent(keyword, Encoding.UTF8, "text/plain");
            var apiResponse = await _offerTextSearchService.SearchOffersAsync(accept: ApiHeader.ResultFormat,
                userAgent: ApiHeader.UserAgent,
                apiKey: ApiHeader.ApiKey,
                acceptLanguage: ApiHeader.Language,
                xOptions: xOptions,
                stringContent: stringContent);

            return CastToNyrisResponse<T>(apiResponse);
        }

        /// <inheritdoc cref="ApiBase.BuildRequestOptions"/>
        protected override string BuildRequestOptions()
        {
            var xOptions = string.Empty;
            if (_regroupOptions.Enabled)
            {
                xOptions += " +regroup";

                // ReSharper disable once CompareOfFloatsByEqualityOperator
                if (_regroupOptions.Threshold != OptionDefaults.UndefinedThreshold)
                {
                    xOptions += $" regroup.threshold={_regroupOptions.Threshold}";
                }
            }

            if (_limit != OptionDefaults.DefaultLimit) xOptions += $" limit={_limit}";

            Reset();
            return xOptions;
        }

        private void Reset()
        {
            _limit = OptionDefaults.DefaultLimit;
            _regroupOptions.Reset();
        }
    }
}
