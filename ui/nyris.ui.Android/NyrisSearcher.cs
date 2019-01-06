using System;
using Android.App;
using Android.Content;
using Android.Support.Annotation;
using Newtonsoft.Json;
using Nyris.Sdk.Network.API.XOptions;
using Nyris.Sdk.Network.Model;
using FragmentOld = Android.App.Fragment;
using FragmentV4 = Android.Support.V4.App.Fragment;

namespace Nyris.Ui.Android
{
    public class NyrisSearcher : INyrisSearcher
    {
        public static readonly int REQUEST_CODE = 20160401;

        internal static readonly string CONFIG_KEY = "CONFIG_KEY";

        private NyrisSearcherConfig _config;

        private NyrisSearcher(bool debug)
        {
            _config = new NyrisSearcherConfig
            {
                IsDebug = debug
            };
        }

        public static INyrisSearcher Instance(bool debug = false)
        {
            return new NyrisSearcher(debug);
        }

        public INyrisSearcher OutputFormat([NonNull] string outputFormat)
        {
            _config.OutputFormat = outputFormat ?? throw new ArgumentException("outputFormat is null");
            return this;
        }

        public INyrisSearcher Language([NonNull] string language)
        {
            _config.Language = language ?? throw new ArgumentException("language is null"); ;
            return this;
        }

        public INyrisSearcher Limit(uint limit)
        {
            _config.Limit = limit;
            return this;
        }

        public INyrisSearcher Exact(Action<ExactOptions> options = null)
        {
            if (options == null)
            {
                return this;
            }
            _config.ExactOptions = new ExactOptions();
            options(_config.ExactOptions);
            return this;
        }

        public INyrisSearcher Similarity(Action<SimilarityOptions> options = null)
        {
            if (options == null)
            {
                return this;
            }
            _config.SimilarityOptions = new SimilarityOptions();
            options(_config.SimilarityOptions);
            return this;
        }

        public INyrisSearcher Ocr(Action<OcrOptions> options = null)
        {
            if (options == null)
            {
                return this;
            }
            _config.OcrOptions = new OcrOptions();
            options(_config.OcrOptions);
            return this;
        }

        public INyrisSearcher Regroup(Action<RegroupOptions> options = null)
        {
            if (options == null)
            {
                return this;
            }
            _config.RegroupOptions = new RegroupOptions();
            options(_config.RegroupOptions);
            return this;
        }

        public INyrisSearcher Recommendation(Action<RecommendationOptions> options = null)
        {
            if (options == null)
            {
                return this;
            }
            _config.RecommendationOptions = new RecommendationOptions();
            options(_config.RecommendationOptions);
            return this;
        }

        public INyrisSearcher CategoryPrediction(Action<CategoryPredictionOptions> options = null)
        {
            if (options == null)
            {
                return this;
            }
            _config.CategoryPredictionOptions = new CategoryPredictionOptions();
            options(_config.CategoryPredictionOptions);
            return this;
        }

        public void Show<FromContext>([NonNull] FromContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var configJson = JsonConvert.SerializeObject(_config);
            if (context is Activity)
            {
                Activity activity = context as Activity;
                var intent = new Intent(activity, typeof(NyrisSearcherActivity));
                intent.PutExtra(CONFIG_KEY, configJson);
                activity.StartActivityForResult(intent, REQUEST_CODE);
            }
            else if (context is FragmentV4)
            {
                FragmentV4 fragmentV4 = context as FragmentV4;
                var intent = new Intent(fragmentV4.Context, typeof(NyrisSearcherActivity));
                intent.PutExtra(CONFIG_KEY, configJson);
                fragmentV4.StartActivityForResult(intent, REQUEST_CODE);
            }
            else if (context is FragmentOld)
            {
                FragmentOld fragmentOld = context as FragmentOld;
                var intent = new Intent(fragmentOld.Context, typeof(NyrisSearcherActivity));
                intent.PutExtra(CONFIG_KEY, configJson);
                fragmentOld.StartActivityForResult(intent, REQUEST_CODE);
            }
            else
            {
                throw new ArgumentException("Internal Error : You need to handle casting here");
            }
        }

        public INyrisSearcher AsJson()
        {
            _config.ResponseType = typeof(JsonResponse);
            return this;
        }

        public INyrisSearcher AsObject()
        {
            _config.ResponseType = typeof(OfferResponse);
            return this;
        }

        public void Show([NonNull] Activity activity)
        {
            Show<Activity>(activity);
        }

        public void Show([NonNull] FragmentV4 fragment)
        {
            Show<FragmentV4>(fragment);
        }

        public void Show([NonNull] FragmentOld fragment)
        {
            Show<FragmentOld>(fragment);
        }
    }
}
