using System;
using Android.App;
using Android.Content;
using AndroidX.Annotations;
using Newtonsoft.Json;
using Nyris.Api.Api.RequestOptions;
using Nyris.UI.Android.Models;
using Nyris.UI.Common;
using FragmentX = AndroidX.Fragment.App.Fragment;

namespace Nyris.UI.Android
{
    public interface INyrisSearcher : INyrisSearcher<INyrisSearcher>
    {
        INyrisSearcher DialogErrorTitle([NonNull] string title);

        INyrisSearcher PositiveButtonText([NonNull] string text);

        INyrisSearcher CameraPermissionDeniedErrorMessage([NonNull] string message);

        INyrisSearcher ExternalStoragePermissionDeniedErrorMessage([NonNull] string message);

        INyrisSearcher CaptureLabelText([NonNull] string label);
    }

    public class NyrisSearcher : INyrisSearcher
    {
        public static readonly int REQUEST_CODE = 20160401;

        public static readonly string SEARCH_RESULT_KEY = "SEARCH_RESULT_KEY";

        internal static readonly string CONFIG_KEY = "CONFIG_KEY";

        private NyrisSearcherConfig _config;
        private Activity _fromActivity;
        private FragmentX _fromFragmentX;

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

        private NyrisSearcher([NonNull] string apiKey, [NonNull] FragmentX fragmentX, bool debug) : this(apiKey, debug)
        {
            _fromFragmentX = fragmentX;
        }

        public static INyrisSearcher Builder(string apiKey, Activity activity, bool debug = false)
        {
            return new NyrisSearcher(apiKey, activity, debug);
        }

        public static INyrisSearcher Builder(string apiKey, FragmentX fragmentV4, bool debug = false)
        {
            return new NyrisSearcher(apiKey, fragmentV4, debug);
        }

        public INyrisSearcher CameraPermissionDeniedErrorMessage([NonNull] string message)
        {
            _config.CameraPermissionDeniedErrorMessage = message;
            return this;
        }

        public INyrisSearcher ExternalStoragePermissionDeniedErrorMessage([NonNull] string message)
        {
            _config.ExternalStoragePermissionDeniedErrorMessage = message;
            return this;
        }

        public INyrisSearcher CaptureLabelText([NonNull] string label)
        {
            _config.CaptureLabelText = label;
            return this;
        }

        public INyrisSearcher DialogErrorTitle([NonNull] string title)
        {
            _config.DialogErrorTitle = title;
            return this;
        }

        public INyrisSearcher PositiveButtonText([NonNull] string text)
        {
            _config.PositiveButtonText = text;
            return this;
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

        public INyrisSearcher Limit(int limit)
        {
            _config.Limit = limit;
            return this;
        }

        public INyrisSearcher Exact(Action<ExactOptions> options = null)
        {
            if (options == null)
            {
                options = opt => { opt.Enabled = true; };
            }
            _config.ExactOptions = new ExactOptions();
            options(_config.ExactOptions);
            return this;
        }

        public INyrisSearcher Similarity(Action<SimilarityOptions> options = null)
        {
            if (options == null)
            {
                options = opt => { opt.Enabled = true; };
            }
            _config.SimilarityOptions = new SimilarityOptions();
            options(_config.SimilarityOptions);
            return this;
        }

        public INyrisSearcher Ocr(Action<OcrOptions> options = null)
        {
            if (options == null)
            {
                options = opt => { opt.Enabled = true; };
            }
            _config.OcrOptions = new OcrOptions();
            options(_config.OcrOptions);
            return this;
        }

        public INyrisSearcher Regroup(Action<RegroupOptions> options = null)
        {
            if (options == null)
            {
                options = opt => { opt.Enabled = true; };
            }
            _config.RegroupOptions = new RegroupOptions();
            options(_config.RegroupOptions);
            return this;
        }

        public INyrisSearcher RecommendationMode(Action<RecommendationModeOptions> options = null)
        {
            if (options == null)
            {
                options = opt => { opt.Enabled = true; };
            }
            _config.RecommendationModeOptions = new RecommendationModeOptions();
            options(_config.RecommendationModeOptions);
            return this;
        }

        public INyrisSearcher CategoryPrediction(Action<CategoryPredictionOptions> options = null)
        {
            if (options == null)
            {
                options = opt => { opt.Enabled = true; };
            }
            _config.CategoryPredictionOptions = new CategoryPredictionOptions();
            options(_config.CategoryPredictionOptions);
            return this;
        }

        public INyrisSearcher ResultAsJson()
        {
            _config.ResponseType = typeof(JsonResponse);
            return this;
        }

        public INyrisSearcher ResultAsObject()
        {
            _config.ResponseType = typeof(OfferResponse);
            return this;
        }

        public void Start(bool loadLastState = false)
        {
            _config.LoadLastState = loadLastState;
            var configJson = JsonConvert.SerializeObject(_config);
            if (_fromActivity != null)
            {
                var intent = new Intent(_fromActivity, typeof(NyrisSearcherActivity));
                intent.PutExtra(CONFIG_KEY, configJson);
                _fromActivity.StartActivityForResult(intent, REQUEST_CODE);
            }
            else if (_fromFragmentX?.Context != null)
            {
                var intent = new Intent(_fromFragmentX.Context, typeof(NyrisSearcherActivity));
                intent.PutExtra(CONFIG_KEY, configJson);
                _fromFragmentX.StartActivityForResult(intent, REQUEST_CODE);
                _fromFragmentX = null;
            }
            else
            {
                throw new ArgumentException("Internal Error : You need to handle casting here");
            }
        }
    }
}
