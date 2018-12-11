using System;
using System.Net.Http;
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
        private int _limit = Options.DEFAULT_LIMIT;

        private readonly RegroupOptions _regroupOptions;

        internal OfferTextSearchApi(IOfferTextSearchService offerTextSearchService, ApiHeader apiHeader) :
            base(apiHeader)
        {
            _offerTextSearchService = offerTextSearchService;

            _regroupOptions = new RegroupOptions();
        }

        public IOfferTextSearchApi OutputFormat(string outputFormat)
        {
            _apiHeader.OutputFormat = outputFormat;
            return this;
        }

        public IOfferTextSearchApi Language(string language)
        {
            _apiHeader.Language = language;
            return this;
        }

        public IOfferTextSearchApi Limit(int limit)
        {
            _limit = limit;
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

        public IObservable<OfferResponse> SearchOffers(string keyword)
        {
            return SearchOffers<OfferResponse>(keyword);
        }

        public IObservable<T> SearchOffers<T>(string keyword)
        {
            var xOptions = BuildXOptions();
            var stringContent = new StringContent(keyword, Encoding.UTF8, "text/plain");
            return _offerTextSearchService.SearchOffers<T>(accept: _apiHeader.OutputFormat,
                userAgent: _apiHeader.UserAgent,
                apiKey: _apiHeader.ApiKey,
                acceptLanguage: _apiHeader.Language,
                xOptions: xOptions,
                stringContent: stringContent);
        }

        public Task<OfferResponse> SearchOffersAsync(string keyword)
        {
            return SearchOffersAsync<OfferResponse>(keyword);
        }

        public Task<T> SearchOffersAsync<T>(string keyword)
        {
            var xOptions = BuildXOptions();
            var stringContent = new StringContent(keyword, Encoding.UTF8, "text/plain");
            return _offerTextSearchService.SearchOffersAsync<T>(accept: _apiHeader.OutputFormat,
                userAgent: _apiHeader.UserAgent,
                apiKey: _apiHeader.ApiKey,
                acceptLanguage: _apiHeader.Language,
                xOptions: xOptions,
                stringContent: stringContent);
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