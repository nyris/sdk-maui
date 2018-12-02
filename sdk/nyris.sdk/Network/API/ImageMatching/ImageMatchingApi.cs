using System;
using System.Threading.Tasks;
using Nyris.Sdk.Network.Model;

namespace Nyris.Sdk.Network.API.ImageMatching
{
    internal sealed class ImageMatchingApi : IImageMatchingApi
    {
        public ImageMatchingApi()
        {
            
        }

        public IImageMatchingApi OutputFormat(string outputFormat)
        {
            throw new System.NotImplementedException();
        }

        public IImageMatchingApi Language(string language)
        {
            throw new System.NotImplementedException();
        }

        public IImageMatchingApi Exact(Action<ExactOptions> options = null)
        {
            throw new NotImplementedException();
        }

        public IImageMatchingApi Similarity(Action<SimilarityOptions> options = null)
        {
            throw new NotImplementedException();
        }

        public IImageMatchingApi Ocr(Action<OcrOptions> options = null)
        {
            throw new NotImplementedException();
        }

        public IImageMatchingApi Limit(int limit)
        {
            throw new NotImplementedException();
        }

        public IImageMatchingApi Regroup(Action<RegroupOptions> options = null)
        {
            throw new NotImplementedException();
        }

        public IImageMatchingApi Recommendation(Action<RecommendationOptions> options = null)
        {
            throw new NotImplementedException();
        }

        public IImageMatchingApi CategoryPrediction(Action<CategoryPredictionOptions> options = null)
        {
            throw new NotImplementedException();
        }

        public Task<OfferResponseBody> Match(byte[] image)
        {
            throw new NotImplementedException();
        }

        public Task<OfferResponseBody> Match(float[] image)
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