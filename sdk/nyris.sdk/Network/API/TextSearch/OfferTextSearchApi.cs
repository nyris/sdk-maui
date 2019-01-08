using System;
using System.Net.Http;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Nyris.Sdk.Network.API.XOptions;
using Nyris.Sdk.Network.Model;
using Nyris.Sdk.Network.Service;

namespace Nyris.Sdk.Network.API.TextSearch
{
    internal class OfferTextSearchApi : Api, IOfferTextSearchApi
    {
        private readonly IOfferTextSearchService _offerTextSearchService;
        private uint _limit = Options.DEFAULT_LIMIT;

        private readonly RegroupOptions _regroupOptions;

        internal OfferTextSearchApi(IOfferTextSearchService offerTextSearchService, ApiHeader apiHeader) :
            base(apiHeader)
        {
            _offerTextSearchService = offerTextSearchService;

            _regroupOptions = new RegroupOptions();
        }

        public IOfferTextSearchApi OutputFormat(string outputFormat)
        {
            if (outputFormat != null)
            {
                _apiHeader.OutputFormat = outputFormat;
            }
            return this;
        }

        public IOfferTextSearchApi Language(string language)
        {
            if (language != null)
            {
                _apiHeader.Language = language;
            }
            return this;
        }

        public IOfferTextSearchApi Limit(uint limit)
        {
            _limit = limit == 0 ? Options.DEFAULT_LIMIT : limit;
            return this;
        }

        public IOfferTextSearchApi Regroup(Action<RegroupOptions> options = null)
        {
            if (options == null)
            {
                options = opt => { opt.Enabled = false; };
            }

            options(_regroupOptions);
            return this;
        }

        public IObservable<OfferResponseDto> SearchOffers(string keyword)
        {
            return SearchOffers<OfferResponseDto>(keyword);
        }

        public IObservable<T> SearchOffers<T>(string keyword) where T : INyrisResponse
        {
            var xOptions = BuildXOptions();
            var stringContent = new StringContent(keyword, Encoding.UTF8, "text/plain");
            var obs1 = _offerTextSearchService.SearchOffers(accept: _apiHeader.OutputFormat,
                userAgent: _apiHeader.UserAgent,
                apiKey: _apiHeader.ApiKey,
                acceptLanguage: _apiHeader.Language,
                xOptions: xOptions,
                stringContent: stringContent);

            var obs2 = Observable.Return(string.Empty);
            return obs1.CombineLatest(obs2, (apiResponse, dummy) => CastToNyrisResponse<T>(apiResponse));
        }

        public Task<OfferResponseDto> SearchOffersAsync(string keyword)
        {
            return SearchOffersAsync<OfferResponseDto>(keyword);
        }

        public async Task<T> SearchOffersAsync<T>(string keyword) where T : INyrisResponse
        {
            var xOptions = BuildXOptions();
            var stringContent = new StringContent(keyword, Encoding.UTF8, "text/plain");
            var apiResponse = await _offerTextSearchService.SearchOffersAsync(accept: _apiHeader.OutputFormat,
                userAgent: _apiHeader.UserAgent,
                apiKey: _apiHeader.ApiKey,
                acceptLanguage: _apiHeader.Language,
                xOptions: xOptions,
                stringContent: stringContent);

            return CastToNyrisResponse<T>(apiResponse);
        }

        protected override string BuildXOptions()
        {
            var xOptions = "";

            if (_regroupOptions.Enabled) xOptions += " +regroup";
            if (_regroupOptions.Enabled && _regroupOptions.Threshold != Options.UNDEFINED_THRESHOLD)
            {
                xOptions += $" regroup.threshold={_regroupOptions.Threshold}";
            }

            if (_limit != Options.DEFAULT_LIMIT) xOptions += $" limit={_limit}";

            Reset();
            return xOptions;
        }

        private void Reset()
        {
            _limit = Options.DEFAULT_LIMIT;
            _regroupOptions.Reset();
        }
    }
}