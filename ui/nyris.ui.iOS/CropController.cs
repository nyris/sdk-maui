using Foundation;
using System;
using CoreGraphics;
using Nyris.Api;
using Nyris.Api.Model;
using Nyris.UI.Common;
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
        [Outlet] private UIImageView ScreenshotImageView { get; set; }
        private INyrisApi _nyrisApi;

        public CGRect CroppingFrame = CGRect.Empty;
        public UIImage ScreenshotImage;
        public EventHandler<OfferResponseEventArgs> OfferAvailable ;
        public EventHandler<Exception> RequestFailed ;

        public CropController (IntPtr handle) : base (handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            ActivityIndicator.HidesWhenStopped = true;
            SetupTheme();

            ScreenshotImageView.ContentMode = UIViewContentMode.ScaleAspectFit;

            if (Config.LoadLastState && ScreenshotImage != null)
            {
                ScreenshotImageView.Image = ScreenshotImage;
                SetCropState();
            }
            else
            {
                SetCaptureState();
            }
        }

        private void SetupTheme()
        {
            if (_theme == null)
            {
                LoadImages();
                return;
            }

            _captureButtonImage = _theme.CaptureButtonImage;
            _cropButtonImage = _theme.CropButtonImage;
            if (_theme.BackButtonImage != null)
            {
                CloseButton.SetImage(_theme.BackButtonImage, UIControlState.Normal);
            }
            CloseButton.TintColor = _theme.BackButtonTint;
            CloseLabel.TextColor = _theme.BackButtonTint;
            CaptureLabel.TextColor = _theme.CaptureLabelColor;

            if (_theme.FlashLightOnButtonImage != null)
            {
                FlashLightButton.SetImage(_theme.FlashLightOnButtonImage, UIControlState.Selected);
            }
            if (_theme.FlashLightOffButtonImage != null)
            {
                FlashLightButton.SetImage(_theme.FlashLightOffButtonImage, UIControlState.Normal);
            }
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
            if(Config.LoadLastState && _currentState != CameraControllerState.Capture)
            {
                CameraManager.Stop();
            }
        }

        public override void Configure(NyrisSearcherConfig config, AppearanceConfiguration theme)
        {
            base.Configure(config, theme);
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
            // If we pass null directly to UIImage.FromBundle it will raise an ambiguous error
            // in ios 13 sdk there is a new overload which take image configuration.
            // the compiler can't decide which UIImage.FromBundle to call as it can't infer type of null
            // hence declaring an explicit UITraitCollection variable and setting it to null to help the compiler.
            UITraitCollection trait = null;
            _captureButtonImage = UIImage.FromBundle("capture_icon.png", bundle, trait);
            _cropButtonImage = UIImage.FromBundle("validate_icon.png", bundle, trait);
        }

        private void SetCaptureState()
        {
            ScreenshotImage = null;
            ActivityIndicator.StopAnimating();
            DarkView.Hidden = true;
            _currentState = CameraControllerState.Capture;
            ScreenshotImageView.Image = null;
            ScreenshotImageView.Hidden = true;
            CaptureButton.Enabled = true;
            CaptureButton.Hidden = false;
            CaptureLabel.Hidden = false;
            CaptureButton.SetImage(_captureButtonImage, UIControlState.Normal);
            CaptureButton.TouchUpInside -= OnCropTapped;
            CaptureButton.TouchUpInside += OnCaptureTapped;

            if (_theme?.CaptureButtonTint != null)
            {
                CaptureButton.TintColor = _theme.CaptureButtonTint;
            }


            if (_cropBoundingBox != null)
            {
                _cropBoundingBox.Hidden = true;
            }

            if (CameraManager.SetupResult == SessionSetupResult.Success &&
             CameraManager.AuthorizationResult == CameraAuthorizationResult.Authorized &&
             CameraManager.IsRunning == false)
            {
                CameraManager.Start();
            }
        }

        private void SetCropState()
        {

            CameraManager.Stop();
            LoadBoundingBox();
            _currentState = CameraControllerState.Crop;
            ScreenshotImageView.Hidden = false;
            CaptureButton.Enabled = true;
            CaptureButton.Hidden = false;
            CaptureLabel.Hidden = true;
            CaptureButton.SetImage(_cropButtonImage, UIControlState.Normal);
            CaptureButton.TouchUpInside -= OnCaptureTapped;
            CaptureButton.TouchUpInside += OnCropTapped;
            if (_theme?.CropButtonTint != null)
            {
                CaptureButton.TintColor = _theme.CropButtonTint;
            }

            _cropBoundingBox.Hidden = false;
            DarkView.Hidden = true;
        }

        void LoadBoundingBox()
        {
            CGRect frame;
            if(CroppingFrame.IsEmpty)
            {
                var boxHeight = View.Frame.Height / 2;
                var boxY = (boxHeight) - (boxHeight / 2);
                frame = new CGRect(15, boxY, View.Frame.Width - 30, boxHeight);
            }
            else
            {
                frame = CroppingFrame;
            }

            if (_cropBoundingBox == null)
            {
                _cropBoundingBox = new CropOverlayView(frame)
                {
                    IsMovable = true,
                    IsResizable = true
                };
                View.AddSubview(_cropBoundingBox);
                View.InsertSubviewBelow(_cropBoundingBox, CaptureButton);
            }
            else
            {
                _cropBoundingBox.Frame = frame;
            }
        }

        private async Task SetFetchState(UIImage screenshot, UIImage croppedImage)
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
            _cropBoundingBox.Hidden = true;
            
            var bytes = croppedImage.AsJPEG().ToArray();
            var matchingService = _nyrisApi.ImageMatching.Limit(Config.Limit);
            OfferResponseEventArgs offerEventArgs = null;
            try
            {
                if (Config.ResponseType == typeof(JsonResponse))
                {
                    var jsonOfferDto = await matchingService.MatchAsync<JsonResponseDto>(bytes);
                    var offerResponse = new JsonResponse()
                    {
                        Content =  jsonOfferDto.Content
                    };
                    offerEventArgs = new OfferResponseEventArgs(screenshot, _cropBoundingBox.Frame, offerResponse);
                }
                else
                {
                    var offer = await matchingService.MatchAsync(bytes);
                    var offerResponse = new OfferResponse(offer);
                    offerEventArgs = new OfferResponseEventArgs(screenshot, _cropBoundingBox.Frame, offerResponse);
                }

            }
            catch (Exception e)
            {
                ShowError(Config.DialogErrorTitle, e.Message, Config.PositiveButtonText, null, (action) =>
                {
                    SetCaptureState();
                });

                croppedImage?.Dispose();
                screenshot?.Dispose();
                bytes = null;
                return;
            }

            OnOfferAvailable(this, offerEventArgs);
            croppedImage?.Dispose();
            bytes = null;
            ScreenshotImage = null;
            Dismiss();

        }

        protected override void ProcessImage(UIImage image)
        {
            base.ProcessImage(image);
            ScreenshotImage?.Dispose();
            ScreenshotImage = image;
        }

        private void OnCaptureTapped(object sender, System.EventArgs e)
        {
            captureFrameAction((UIButton)sender);
            CameraManager.Stop();
            SetCropState();
         }
        
        private void OnCropTapped(object sender, System.EventArgs e)
        {
            if (ScreenshotImage == null || _cropBoundingBox?.Frame == null)
            {
                return;
            }

            var croppedImage = ImageHelper.Crop(ScreenshotImage, _cropBoundingBox.Frame, View.Frame);
            if (croppedImage == null)
            {
                return;
            }

            var resizedCroppedImage = ImageHelper.ResizeWithRatio(croppedImage, new CGSize(1024, 1024));
            croppedImage.Dispose();
            SetFetchState(ScreenshotImage, resizedCroppedImage);
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
            switch(_currentState)
            {
                case CameraControllerState.Capture:
                    Dismiss();
                    break;
                case CameraControllerState.Crop:
                    SetCaptureState();
                    break;
                case CameraControllerState.Fetch:
                    break;
            }
        }

        protected override void Dismiss()
        {
            base.Dismiss();
            Dispose();
            GC.Collect();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (!disposing)
            {
                return;
            }

            _cropBoundingBox?.Dispose();
            _cropBoundingBox = null;
            CameraManager.Stop();
            _captureButtonImage = null;
            _cropButtonImage = null;
            ScreenshotImage?.Dispose();
            ScreenshotImage = null;
            ReleaseDesignerOutlets();
        }

        private void OnRequestFailed(object sender, Exception exception) => RequestFailed?.Invoke(this, exception);
                
        private void OnOfferAvailable(object sender, OfferResponseEventArgs offerArgs) => OfferAvailable?.Invoke(this, offerArgs);

    }
}