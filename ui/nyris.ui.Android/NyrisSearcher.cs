using System;
using Android.App;
using Android.Content;
using Android.Support.Annotation;
using Newtonsoft.Json;
using Nyris.Sdk.Network.API.XOptions;
using Nyris.Ui.Android.Models;
using FragmentOld = Android.App.Fragment;
using FragmentV4 = Android.Support.V4.App.Fragment;

namespace Nyris.Ui.Android
{
    public class NyrisSearcher : INyrisSearcher
    {
        public static readonly int REQUEST_CODE = 20160401;

        public static readonly string SEARCH_RESULT_KEY = "SEARCH_RESULT_KEY";

        internal static readonly string CONFIG_KEY = "CONFIG_KEY";

        private NyrisSearcherConfig _config;
        private Activity _fromActivity;
        private FragmentV4 _fromFragmentV4;
        private FragmentOld _fromFragmentOld;

        private NyrisSearcher(string apiKey, bool debug)
        {
            _config = new NyrisSearcherConfig
            {
                ApiKey = apiKey,
                IsDebug = debug
            };
        }

        private NyrisSearcher([NonNull] string apiKey, [NonNull] Activity activity, bool debug) : this(apiKey, debug)
        {
            _fromActivity = activity;
        }

        private NyrisSearcher([NonNull] string apiKey, [NonNull] FragmentV4 fragmentV4, bool debug) : this(apiKey, debug)
        {
            _fromFragmentV4 = fragmentV4;
        }

        private NyrisSearcher([NonNull]string apiKey, [NonNull] FragmentOld fragmentOld, bool debug) : this(apiKey, debug)
        {
            _fromFragmentOld = fragmentOld;
        }

        public static INyrisSearcher Builder(string apiKey, Activity activity, bool debug = false)
        {
            return new NyrisSearcher(apiKey, activity, debug);
        }

        public static INyrisSearcher Builder(string apiKey, FragmentV4 fragmentV4, bool debug = false)
        {
            return new NyrisSearcher(apiKey, fragmentV4, debug);
        }

        public static INyrisSearcher Builder(string apiKey, FragmentOld fragmentOld, bool debug = false)
        {
            return new NyrisSearcher(apiKey, fragmentOld, debug);
        }

        public INyrisSearcher ApiKey([NonNull] string apiKey)
        {
            _config.ApiKey = apiKey;
            return this;
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

        public void Start()
        {
            var configJson = JsonConvert.SerializeObject(_config);
            if (_fromActivity != null)
            {
                var intent = new Intent(_fromActivity, typeof(NyrisSearcherActivity));
                intent.PutExtra(CONFIG_KEY, configJson);
                _fromActivity.StartActivityForResult(intent, REQUEST_CODE);
            }
            else if (_fromFragmentV4 != null)
            {
                var intent = new Intent(_fromFragmentV4.Context, typeof(NyrisSearcherActivity));
                intent.PutExtra(CONFIG_KEY, configJson);
                _fromFragmentV4.StartActivityForResult(intent, REQUEST_CODE);
            }
            else if (_fromFragmentOld != null)
            {
                var intent = new Intent(_fromFragmentOld.Context, typeof(NyrisSearcherActivity));
                intent.PutExtra(CONFIG_KEY, configJson);
                _fromFragmentOld.StartActivityForResult(intent, REQUEST_CODE);
            }
            else
            {
                throw new ArgumentException("Internal Error : You need to handle casting here");
            }
        }
    }
}
