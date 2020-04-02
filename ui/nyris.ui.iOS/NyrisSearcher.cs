using System;
using CoreGraphics;
using Foundation;
using Nyris.Api.Api.RequestOptions;
using Nyris.UI.Common;
using Nyris.UI.iOS.EventArgs;
using Nyris.UI.iOS.Models;
using UIKit;

namespace Nyris.UI.iOS
{
    public interface INyrisSearcher : Nyris.UI.Common.INyrisSearcher<INyrisSearcher>
    {

        event EventHandler<OfferResponseEventArgs> OfferAvailable;

        INyrisSearcher DialogErrorTitle(string title);

        INyrisSearcher AgreeButtonTitle(string title);

        INyrisSearcher CancelButtonTitle(string title);

        INyrisSearcher CameraPermissionDeniedErrorMessage(string message);

        INyrisSearcher CameraPermissionRequestIfDeniedMessage(string message);

        INyrisSearcher ConfigurationFailedErrorMessage(string message);

        INyrisSearcher CaptureLabelText(string label);

        INyrisSearcher Theme(AppearanceConfiguration theme);
    }

    public class NyrisSearcher : INyrisSearcher
    {
        internal class CaptureSessionParametres
        {
            public UIImage Screenshot;
            public CGRect CroppingFrame;
        }

        [Weak] UIViewController _presenterController;
        [Weak] CropController _cropController;
        private CaptureSessionParametres _previousSessionParametres;
        private NyrisSearcherConfig _config;
        private AppearanceConfiguration _controllerAppearance;

        public event EventHandler<OfferResponseEventArgs> OfferAvailable;
        public event EventHandler<Exception> RequestFailed;
        
        private NyrisSearcher(string apiKey, UIViewController presenterController,bool debug)
        {
            _config = new NyrisSearcherConfig
            {
                ApiKey = apiKey,
                IsDebug = debug
            };
            this._presenterController = presenterController;
            _previousSessionParametres = new CaptureSessionParametres();
        }

        public static INyrisSearcher Builder(string apiKey, UIViewController presenterController, bool debug = false)
        {
            return new NyrisSearcher(apiKey, presenterController, debug);
        }


        public INyrisSearcher CancelButtonTitle(string title)
        {
            _config.NegativeButtonText = title;
            return this;
        }

        public INyrisSearcher CameraPermissionDeniedErrorMessage(string message = "")
        {
            _config.CameraPermissionDeniedErrorMessage = message;
            return this;
        }

        public INyrisSearcher CameraPermissionRequestIfDeniedMessage(string message)
        {            
            _config.CameraPermissionRequestIfDeniedMessage = message;
            return this;
        }

        public INyrisSearcher ConfigurationFailedErrorMessage(string message)
        {
            _config.ConfigurationFailedErrorMessage = message;
            return this;
        }

        public INyrisSearcher CaptureLabelText(string label)
        {
            _config.CaptureLabelText = label;
            return this;
        }

        public INyrisSearcher DialogErrorTitle(string title)
        {
            _config.DialogErrorTitle = title;
            return this;
        }

        public INyrisSearcher AgreeButtonTitle(string title)
        {
            _config.PositiveButtonText = title;
            return this;
        }

        public INyrisSearcher ApiKey(string apiKey)
        {
            _config.ApiKey = apiKey;
            return this;
        }

        public INyrisSearcher OutputFormat(string outputFormat)
        {
            _config.OutputFormat = outputFormat ?? throw new ArgumentException("outputFormat is null");
            return this;
        }

        public INyrisSearcher Language(string language)
        {
            _config.Language = language ?? throw new ArgumentException("language is null");
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

        public INyrisSearcher Theme(AppearanceConfiguration theme)
        {
            _controllerAppearance = theme;
            return this;
        }

        public void Start(bool loadLastState = false)
        {
            _cropController?.Dispose();
            var bundle = NSBundle.FromClass(new ObjCRuntime.Class(typeof(CameraController)));
            var storyboard = UIStoryboard.FromName("CameraController", bundle);

            if (_presenterController == null)
            {
                throw new ArgumentNullException(nameof(_presenterController), "Presenter view controller is null");
            }


            var controller = storyboard.InstantiateInitialViewController();
            if (!(controller is CropController))
            {
                throw new ArgumentNullException(nameof(controller), "Controller is not a type of CropController");
            }
            else
            {
                _cropController = controller as CropController;
            }

            _config.LoadLastState = loadLastState;
            _cropController.Configure(_config, _controllerAppearance);
            _cropController.OfferAvailable += OnOfferAvailable;
            _cropController.RequestFailed += (sender, exception) => RequestFailed?.Invoke(this, exception);

            if (loadLastState)
            {
                _cropController.ScreenshotImage = _previousSessionParametres.Screenshot;
                _cropController.CroppingFrame = _previousSessionParametres.CroppingFrame;
            }
            else
            {
                _cropController.ScreenshotImage?.Dispose();
                _cropController.ScreenshotImage = null;
                GC.Collect();
            }
            _cropController.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;
            _presenterController.PresentViewController(_cropController, true, null);
        }

        void OnOfferAvailable(object sender, OfferResponseEventArgs e)
        {
            OfferAvailable?.Invoke(this, e);
            _previousSessionParametres.Screenshot = e.Screenshot;
            _previousSessionParametres.CroppingFrame = e.CroppingFrame;
            e.Screenshot = null;

        }

    }
}
