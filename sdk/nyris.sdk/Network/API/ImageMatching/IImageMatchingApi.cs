using System;
using Io.Nyris.Sdk.Network.Model;

namespace Io.Nyris.Sdk.Network.API.ImageMatching
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

        IObservable<OfferResponse> Match(byte[] image);
        
        IObservable<T> Match<T>(byte[] image);
    }
}