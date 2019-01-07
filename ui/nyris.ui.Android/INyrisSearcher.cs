using System;
using Android.App;
using Android.Support.Annotation;
using Nyris.Sdk.Network.API.XOptions;
using Nyris.Sdk.Network.Model;
using FragmentOld = Android.App.Fragment;
using FragmentV4 = Android.Support.V4.App.Fragment;

namespace Nyris.Ui.Android
{
    public interface INyrisSearcher
    {
        INyrisSearcher OutputFormat([NonNull] string outputFormat);

        INyrisSearcher Language([NonNull] string language);

        INyrisSearcher Limit(uint limit);

        INyrisSearcher Exact(Action<ExactOptions> options = null);

        INyrisSearcher Similarity(Action<SimilarityOptions> options = null);

        INyrisSearcher Ocr(Action<OcrOptions> options = null);

        INyrisSearcher Regroup(Action<RegroupOptions> options = null);

        INyrisSearcher Recommendation(Action<RecommendationOptions> options = null);

        INyrisSearcher CategoryPrediction(Action<CategoryPredictionOptions> options = null);

        void Start<FromContext>([NonNull] FromContext context);

        void Start([NonNull] Activity activity);

        void Start([NonNull] FragmentV4 fragment);

        void Start([NonNull] FragmentOld fragment);
    }
}
