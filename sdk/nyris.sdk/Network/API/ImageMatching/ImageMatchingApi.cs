using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Nyris.Sdk.Network.Model;
using Nyris.Sdk.Network.Service;
using Nyris.Sdk.Utils;
using Refit;

namespace Nyris.Sdk.Network.API.ImageMatching
{
    internal sealed class ImageMatchingApi : IImageMatchingApi
    {
        private readonly IImageMatchingService _imageMatchingService;
        private ApiHeader _apiHeader;
        private int _limit;

        private readonly ExactOptions _exactOptions;
        private readonly SimilarityOptions _similarityOptions;
        private readonly OcrOptions _ocrOptions;
        private readonly RegroupOptions _regroupOptions;
        private readonly RecommendationOptions _recommendationOptions;
        private readonly CategoryPredictionOptions _categoryPredictionOptions;

        public ImageMatchingApi(
            IImageMatchingService imageMatchingService,
            ApiHeader apiHeader)
        {
            _imageMatchingService = imageMatchingService;
            _apiHeader = apiHeader;
            _limit = Constants.DEFAULT_LIMIT;

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

        public IObservable<OfferResponse> Match(byte[] image)
        {
            var byteContent = new ByteArrayContent(image);
            return _imageMatchingService.Match(accept: _apiHeader.OutputFormat,
                userAgent: _apiHeader.UserAgent,
                apiKey: _apiHeader.ApiKey,
                acceptLanguage: _apiHeader.Language,
                xOptions: "",
                contentType: "image/jpg",
                contentLength: image.Length.ToString(),
                image: byteContent);
        }

        public Task<OfferResponse> Match(float[] image)
        {
            throw new NotImplementedException();
        }

        public Task<T> Match<T>(byte[] image)
        {
            throw new NotImplementedException();
        }

        public Task<T> Match<T>(float[] image)
        {
            throw new NotImplementedException();
        }
    }
}