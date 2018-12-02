using System;
using System.Threading.Tasks;
using Nyris.Sdk.Network.Model;

namespace Nyris.Sdk.Network.API.ImageMatching
{
    public interface IImageMatchingApi
    {
        IImageMatchingApi OutputFormat(string outputFormat);

        IImageMatchingApi Language(string language);

        IImageMatchingApi Exact(Action<ExactOptions> options = null);

        IImageMatchingApi Similarity(Action<SimilarityOptions> options = null);

        IImageMatchingApi Ocr(Action<OcrOptions> options = null);

        IImageMatchingApi Limit(int limit);

        IImageMatchingApi Regroup(Action<RegroupOptions> options = null);

        IImageMatchingApi Recommendation(Action<RecommendationOptions> options = null);

        IImageMatchingApi CategoryPrediction(Action<CategoryPredictionOptions> options = null);

        Task<OfferResponseBody> Match(byte[] image);
        
        Task<OfferResponseBody> Match(float[] image);
        
        Task<T> Match<T>(byte[] image);
        
        Task<T> Match<T>(float[] image);
    }
}