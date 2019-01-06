using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Nyris.Sdk.Network.API.XOptions;
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

        IImageMatchingApi Limit(uint limit);

        IImageMatchingApi Regroup(Action<RegroupOptions> options = null);

        IImageMatchingApi Recommendation(Action<RecommendationOptions> options = null);

        IImageMatchingApi CategoryPrediction(Action<CategoryPredictionOptions> options = null);

        IObservable<OfferResponse> Match([NotNull] byte[] image);

        IObservable<T> Match<T>([NotNull] byte[] image) where T : INyrisResponse;

        Task<OfferResponse> MatchAsync([NotNull] byte[] image);

        Task<T> MatchAsync<T>([NotNull] byte[] image) where T : INyrisResponse;
    }
}