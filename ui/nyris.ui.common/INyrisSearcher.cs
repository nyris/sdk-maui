using System;
using Nyris.Sdk.Network.API.XOptions;

namespace Nyris.UI.Common
{
    public interface INyrisSearcher
    {
        INyrisSearcher OutputFormat(string outputFormat);

        INyrisSearcher Language(string language);

        INyrisSearcher Exact(Action<ExactOptions> options = null);

        INyrisSearcher Similarity(Action<SimilarityOptions> options = null);

        INyrisSearcher Ocr(Action<OcrOptions> options = null);

        INyrisSearcher Limit(int limit);

        INyrisSearcher Regroup(Action<RegroupOptions> options = null);

        INyrisSearcher Recommendation(Action<RecommendationOptions> options = null);

        INyrisSearcher CategoryPrediction(Action<CategoryPredictionOptions> options = null);

        INyrisSearcher ResultAsJson();

        INyrisSearcher ResultAsObject();

        void Show();
    }
}