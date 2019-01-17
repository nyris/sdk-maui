using Foundation;
using System;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reflection;
using CoreGraphics;
using Nyris.Api;
using Nyris.Api.Model;
using Nyris.UI.Common;
using ObjCRuntime;
using UIKit;
using Nyris.UI.iOS.Camera.Enum;
using Nyris.UI.iOS.Crop;
using Nyris.UI.iOS.ImageOperations;
using Nyris.UI.iOS.Models;
using Platform = Nyris.Api.Platform;
using System.Threading.Tasks;
using Nyris.UI.iOS.EventArgs;

namespace Nyris.UI.iOS
{
    internal enum CameraControllerState
    {
        Capture, Crop, Fetch
    }
    
    public partial class CropController : CameraController
    {
        private CameraControllerState _currentState = CameraControllerState.Capture;
        
        private CropOverlayView _cropBoundingBox;
        private UIImage _captureButtonImage;
        private UIImage _cropButtonImage;
        private UIImageView _screenshotImageView;
        private UIImage _screenshotImage;
        private INyrisApi _nyrisApi;

        public EventHandler<OfferResponseEventArgs> OfferAvailable ;
        public EventHandler<Exception> RequestFailed ;

        public CropController (IntPtr handle) : base (handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            ActivityIndicator.HidesWhenStopped = true;
            LoadImages();
            SetCaptureState();
            _screenshotImageView = new UIImageView(frame: View.Bounds);
            _screenshotImageView.ContentMode = UIViewContentMode.ScaleAspectFit;
        }

        public override void Configure(NyrisSearcherConfig config)
        {
            base.Configure(config);
            _nyrisApi = NyrisApi.CreateInstance(Config.ApiKey, Platform.iOS, Config.IsDebug);
            MapConfig();
        }

        private void MapConfig()
        {
            _nyrisApi.ImageMatching.OutputFormat(Config.OutputFormat);
            _nyrisApi.ImageMatching.Language(Config.Language);
            _nyrisApi.ImageMatching.Limit(Config.Limit);

            if (Config.ExactOptions != null)
            {
                _nyrisApi.ImageMatching.Exact(obj =>
                {
                    obj.Enabled = Config.ExactOptions.Enabled;
                });
            }

            if (Config.SimilarityOptions != null)
            {
                _nyrisApi.ImageMatching.Similarity(obj =>
                {
                    obj.Enabled = Config.SimilarityOptions.Enabled;
                    obj.Limit = Config.SimilarityOptions.Limit;
                    obj.Threshold = Config.SimilarityOptions.Threshold;
                });
            }

            if (Config.OcrOptions != null)
            {
                _nyrisApi.ImageMatching.Ocr(obj =>
                {
                    obj.Enabled = Config.OcrOptions.Enabled;
                });
            }

            if (Config.RegroupOptions != null)
            {
                _nyrisApi.ImageMatching.Regroup(obj =>
                {
                    obj.Enabled = Config.RegroupOptions.Enabled;
                    obj.Threshold = Config.RegroupOptions.Threshold;
                });
            }

            if (Config.RecommendationModeOptions != null)
            {
                _nyrisApi.ImageMatching.RecommendationMode(obj =>
                {
                    obj.Enabled = Config.RecommendationModeOptions.Enabled;
                });
            }

            if (Config.CategoryPredictionOptions != null)
            {
                _nyrisApi.ImageMatching.CategoryPrediction(obj =>
                {
                    obj.Enabled = Config.CategoryPredictionOptions.Enabled;
                    obj.Limit = Config.CategoryPredictionOptions.Limit;
                    obj.Threshold = Config.CategoryPredictionOptions.Threshold;
                });
            }
        }
        
        private void LoadImages()
        {
            
            var bundle = NSBundle.FromClass(new ObjCRuntime.Class(typeof(CropController)));
            _captureButtonImage = UIImage.FromBundle("capture_icon", bundle, null);
            _cropButtonImage = UIImage.FromBundle("validate_icon", bundle, null);
        }

        private void SetCaptureState()
        {
            ActivityIndicator.StopAnimating();
            DarkView.Hidden = true;
            _currentState = CameraControllerState.Capture;
            CaptureButton.Enabled = true;
            CaptureButton.Hidden = false;
            CaptureLabel.Hidden = false;
            CaptureButton.SetImage(_captureButtonImage, UIControlState.Normal);
            CaptureButton.TouchUpInside -= OnCropTapped;
            CaptureButton.TouchUpInside += OnCaptureTapped;
            if (_cropBoundingBox != null)
            {
                _cropBoundingBox.Hidden = true;
            }
            _screenshotImageView?.RemoveFromSuperview();
            _screenshotImage?.Dispose();
            _screenshotImage = null;

            if (CameraManager.SetupResult == SessionSetupResult.Success &&
             CameraManager.AuthorizationResult == CameraAuthorizationResult.Authorized &&
             CameraManager.IsRunning == false)
            {
                CameraManager.Start();
            }
        }

        private void SetCropState()
        {
            var boxHeight = View.Frame.Height / 2;
            var boxY = (boxHeight) - (boxHeight / 2);
            var frame = new CGRect(15, boxY, View.Frame.Width - 30, boxHeight);
            if (_cropBoundingBox == null) {
                _cropBoundingBox = new CropOverlayView(frame)
                {
                    IsMovable = true, IsResizable = true
                };
                View.AddSubview(_cropBoundingBox);
                View.InsertSubviewBelow(_cropBoundingBox, CaptureButton);
            }
            else
            {
                _cropBoundingBox.Frame = frame;
            }
            _currentState = CameraControllerState.Crop;
            CaptureButton.Enabled = true;
            CaptureButton.Hidden = false;
            CaptureLabel.Hidden = false;
            CaptureButton.SetImage(_cropButtonImage, UIControlState.Normal);
            CaptureButton.TouchUpInside -= OnCaptureTapped;
            CaptureButton.TouchUpInside += OnCropTapped;
            _cropBoundingBox.Hidden = false;
            DarkView.Hidden = true;
        }

        private async Task SetFetchState(UIImage image)
        {
            _currentState = CameraControllerState.Fetch;
            CaptureButton.Enabled = false;
            CaptureButton.Hidden = true;
            CaptureLabel.Hidden = true;
            CaptureButton.TouchUpInside -= OnCaptureTapped;
            CaptureButton.TouchUpInside -= OnCropTapped;
            ActivityIndicator.StartAnimating();
            ActivityIndicator.Hidden = false;
            DarkView.Hidden = false;
            
            // check for network first ?
            
            // get image bytes
            var bytes = image.AsJPEG().ToArray();
            //_cropBoundingBox.Hidden = false;
            var responseType = Config.ResponseType == typeof(JsonResponse)
                ? typeof(JsonResponseDto)
                : typeof(OfferResponseDto);

            var matchingService = _nyrisApi.ImageMatching.Limit(Config.Limit);
            OfferResponseEventArgs offerEventArgs;
            try
            {
                if (Config.ResponseType == typeof(JsonResponse))
                {
                    var jsonOfferDto = await matchingService.MatchAsync<JsonResponseDto>(bytes);
                    var offerResponse = new JsonResponse()
                    {
                        Content =  jsonOfferDto.Content
                    };
                    offerEventArgs = new OfferResponseEventArgs(offerResponse);
                }
                else
                {
                    var offer = await matchingService.MatchAsync(bytes);
                    var offerResponse = new OfferResponse(offer);
                    offerEventArgs = new OfferResponseEventArgs(offerResponse);
                }
                OnOfferAvailable(this, offerEventArgs);
                Dismiss();

            }
            catch (Exception e)
            {
                // ShowError(Config.DialogErrorTitle, e.Message, Config.PositiveButtonText, null, null);
                OnRequestFailed(this, e);
                SetCaptureState();
            }
            finally
            {
                image?.Dispose();
                bytes = null;
            }
           
        }

        protected override void ProcessImage(UIImage image)
        {
            base.ProcessImage(image);
            _screenshotImage = image;
        }

        private void OnCaptureTapped(object sender, System.EventArgs e)
        {
            captureFrameAction((UIButton)sender);
            CameraManager.Stop();
            SetCropState();
         }
        
        private void OnCropTapped(object sender, System.EventArgs e)
        {
            if (_screenshotImage == null || _cropBoundingBox?.Frame == null)
            {
                return;
            }

            var croppedImage = ImageHelper.Crop(_screenshotImage, _cropBoundingBox.Frame, View.Frame);
            if (croppedImage == null)
            {
                return;
            }

            var resizedCroppedImage = ImageHelper.ResizeWithRatio(croppedImage, new CGSize(512, 512));
            _screenshotImage.Dispose();
            croppedImage.Dispose();
            _screenshotImage = null;
            resizedCroppedImage.SaveToPhotosAlbum((image, error) =>
            {
                Console.WriteLine(1);
            });
            
            SetFetchState(resizedCroppedImage);
        }

        protected override void ApplicationActivated(NSNotification notification)
        {
            if (_currentState == CameraControllerState.Crop)
            {
                return;
            }
            base.ApplicationActivated(notification);
        }

        protected override void ApplicationSuspended(NSNotification notification)
        {
            if (_currentState == CameraControllerState.Crop)
            {
                return;
            }
            base.ApplicationSuspended(notification);
        }

        partial void CloseTapped(UIButton sender)
        {
            Dismiss();
        }

        protected override void Dismiss()
        {
            base.Dismiss();

            //_cropBoundingBox?.Dispose();
            //_cropBoundingBox = null;
            _captureButtonImage?.Dispose();
            _captureButtonImage = null;
            _cropButtonImage?.Dispose();
            _cropButtonImage = null;
            _screenshotImageView?.Dispose();
            _screenshotImageView = null;
            _screenshotImage?.Dispose();
            _screenshotImage = null;
        }
        
        private void OnRequestFailed(object sender, Exception exception)
        {
            try
            {
                RequestFailed?.Invoke(this, exception);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
        
        private void OnOfferAvailable(object sender, OfferResponseEventArgs offerArgs)
        {
            OfferAvailable?.Invoke(this, offerArgs);
        }
    }
}