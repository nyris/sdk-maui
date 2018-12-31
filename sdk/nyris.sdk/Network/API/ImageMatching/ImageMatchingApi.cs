using System;
using System.Net.Http;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Nyris.Sdk.Network.API.XOptions;
using Nyris.Sdk.Network.Model;
using Nyris.Sdk.Network.Service;
using Nyris.Sdk.Utils;

namespace Nyris.Sdk.Network.API.ImageMatching
{
    internal sealed class ImageMatchingApi : Api, IImageMatchingApi
    {
        private readonly IImageMatchingService _imageMatchingService;
        private int _limit = Options.DEFAULT_LIMIT;

        private readonly ExactOptions _exactOptions;
        private readonly SimilarityOptions _similarityOptions;
        private readonly OcrOptions _ocrOptions;
        private readonly RegroupOptions _regroupOptions;
        private readonly RecommendationOptions _recommendationOptions;
        private readonly CategoryPredictionOptions _categoryPredictionOptions;

        internal ImageMatchingApi(
            IImageMatchingService imageMatchingService,
            ApiHeader apiHeader) : base(apiHeader)
        {
            _imageMatchingService = imageMatchingService;

            _exactOptions = new ExactOptions();
            _similarityOptions = new SimilarityOptions();
            _ocrOptions = new OcrOptions();
            _regroupOptions = new RegroupOptions();
            _recommendationOptions = new RecommendationOptions();
            _categoryPredictionOptions = new CategoryPredictionOptions();
        }

        public IImageMatchingApi OutputFormat(string outputFormat)
        {
            _apiHeader.OutputFormat = outputFormat;
            return this;
        }

        public IImageMatchingApi Language(string language)
        {
            _apiHeader.Language = language;
            return this;
        }

        public IImageMatchingApi Exact(Action<ExactOptions> options = null)
        {
            if (options == null)
            {
                options = opt => { opt.Enabled = true; };
            }

            options(_exactOptions);
            return this;
        }

        public IImageMatchingApi Similarity(Action<SimilarityOptions> options = null)
        {
            if (options == null)
            {
                options = opt => { opt.Enabled = true; };
            }

            options(_similarityOptions);
            return this;
        }

        public IImageMatchingApi Ocr(Action<OcrOptions> options = null)
        {
            if (options == null)
            {
                options = opt => { opt.Enabled = true; };
            }

            options(_ocrOptions);
            return this;
        }

        public IImageMatchingApi Limit(int limit)
        {
            _limit = limit;
            return this;
        }

        public IImageMatchingApi Regroup(Action<RegroupOptions> options = null)
        {
            if (options == null)
            {
                options = opt => { opt.Enabled = false; };
            }

            options(_regroupOptions);
            return this;
        }

        public IImageMatchingApi Recommendation(Action<RecommendationOptions> options = null)
        {
            if (options == null)
            {
                options = opt => { opt.Enabled = false; };
            }

            options(_recommendationOptions);
            return this;
        }

        public IImageMatchingApi CategoryPrediction(Action<CategoryPredictionOptions> options = null)
        {
            if (options == null)
            {
                options = opt => { opt.Enabled = false; };
            }

            options(_categoryPredictionOptions);
            return this;
        }

        public IObservable<OfferResponse> Match(byte[] image) => Match<OfferResponse>(image);

        public IObservable<T> Match<T>(byte[] image) where T : INyrisResponse
        {
            var byteContent = new ByteArrayContent(image);
            var xOptions = BuildXOptions();
            var obs1 = _imageMatchingService.Match(accept: _apiHeader.OutputFormat,
                userAgent: _apiHeader.UserAgent,
                apiKey: _apiHeader.ApiKey,
                acceptLanguage: _apiHeader.Language,
                xOptions: xOptions,
                contentType: "image/jpg",
                image: byteContent);
            
            var obs2 = Observable.Return(string.Empty);
            return obs1.CombineLatest(obs2, (apiResponse, dummy) => CastToNyrisResponse<T>(apiResponse));
        }

        public Task<OfferResponse> MatchAsync(byte[] image) => MatchAsync<OfferResponse>(image);

        public async Task<T> MatchAsync<T>(byte[] image) where T : INyrisResponse
        {
            var byteContent = new ByteArrayContent(image);
            var xOptions = BuildXOptions();
            var apiResponse = await _imageMatchingService.MatchAsync(accept: _apiHeader.OutputFormat,
                userAgent: _apiHeader.UserAgent,
                apiKey: _apiHeader.ApiKey,
                acceptLanguage: _apiHeader.Language,
                xOptions: xOptions,
                contentType: "image/jpg",
                image: byteContent);
            
            return CastToNyrisResponse<T>(apiResponse);
        }

        protected override string BuildXOptions()
        {
            var xOptions = "";
            if (_exactOptions.Enabled && xOptions.IsEmpty()) xOptions = "exact";

            if (_similarityOptions.Enabled && xOptions.IsEmpty()) xOptions = "similarity";
            else if (_similarityOptions.Enabled) xOptions += " +similarity";

            if (_ocrOptions.Enabled && xOptions.IsEmpty()) xOptions = "ocr";
            else if (_ocrOptions.Enabled) xOptions += " +ocr";

            if (_similarityOptions.Enabled && _similarityOptions.Limit != Options.UNDEFINED_LIMIT)
            {
                xOptions += $" similarity.limit={_similarityOptions.Limit}";
            }

            if (_similarityOptions.Enabled && _similarityOptions.Threshold != Options.UNDEFINED_THRESHOLD)
            {
                xOptions += $" similarity.threshold={_similarityOptions.Threshold}";
            }

            if (_regroupOptions.Enabled) xOptions += " +regroup";
            if (_regroupOptions.Enabled && _regroupOptions.Threshold != Options.UNDEFINED_THRESHOLD)
            {
                xOptions += $" regroup.threshold={_regroupOptions.Threshold}";
            }

            if (_limit != Options.DEFAULT_LIMIT) xOptions += $" limit={_limit}";

            if (_recommendationOptions.Enabled) xOptions += " +recommendations";

            if (_categoryPredictionOptions.Enabled) xOptions += " +category-prediction";
            if (_categoryPredictionOptions.Enabled && _categoryPredictionOptions.Limit != Options.UNDEFINED_LIMIT)
            {
                xOptions += $" category-prediction.limit={_categoryPredictionOptions.Limit}";
            }

            if (_categoryPredictionOptions.Enabled &&
                _categoryPredictionOptions.Threshold != Options.UNDEFINED_THRESHOLD)
            {
                xOptions += $" category-prediction.threshold={_categoryPredictionOptions.Threshold}";
            }

            Reset();
            return xOptions;
        }

        private void Reset()
        {
            _limit = Options.DEFAULT_LIMIT;
            _exactOptions.Reset();
            _similarityOptions.Reset();
            _ocrOptions.Reset();
            _regroupOptions.Reset();
            _recommendationOptions.Reset();
            _categoryPredictionOptions.Reset();
        }
    }
}