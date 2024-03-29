using CoreFoundation;
using Nyris.Api;
using Nyris.Api.Model;
using Nyris.UI.Common;
using Nyris.UI.Common.Extensions;
using Nyris.UI.Common.Models;
using Nyris.UI.iOS.Camera.Enum;
using Nyris.UI.iOS.Crop;
using Nyris.UI.iOS.ImageOperations;
using Platform = Nyris.Api.Platform;
using Nyris.UI.iOS.EventArgs;
using ObjCRuntime;

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
        public UIImage? ScreenshotImage;
        public EventHandler<OfferResponseEventArgs> OfferAvailable ;
        public EventHandler<Exception> RequestFailed ;

        public CropController (IntPtr handle) : base (handle)
        {
        }

        protected CropController(NativeHandle handle) : base(handle)
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

        public override void TraitCollectionDidChange(UITraitCollection? previousTraitCollection)
        {
            base.TraitCollectionDidChange(previousTraitCollection);
            // Reset crop view frame to adapt to device orientation change.
            if (_cropBoundingBox != null)
            {
                var size = new CGSize( View.Frame.Width / 1.2, View.Frame.Height / 2  );
                _cropBoundingBox.Frame = new CGRect(0, 0, size.Width, size.Height);
                _cropBoundingBox.Center = View.Center;
            }
        }
        
        private void SetupTheme()
        {
            LoadImages();
            
            if (_theme == null)
            {
                return;
            }

            _captureButtonImage = _theme switch
            {
                { CaptureButtonImage: not null, CaptureButtonTint: not null, } => _theme.CaptureButtonImage.ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate).ApplyTintColor(_theme.CaptureButtonTint),
                { CaptureButtonImage: not null } => _theme.CaptureButtonImage,
                { CaptureButtonImage: null, CaptureButtonTint: not null, } => _captureButtonImage.ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate).ApplyTintColor(_theme.CaptureButtonTint),
                _ => _captureButtonImage
            };          
            _cropButtonImage = _theme switch
            {
                { CropButtonImage: not null, CropButtonTint: not null, } => _theme.CropButtonImage.ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate).ApplyTintColor(_theme.CropButtonTint),
                { CropButtonImage: not null } => _theme.CropButtonImage,              
                { CropButtonImage: null, CropButtonTint: not null, } => _cropButtonImage.ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate).ApplyTintColor(_theme.CropButtonTint),

                _ => _cropButtonImage
            };

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
            _nyrisApi = NyrisApi.CreateInstance(Config.ApiKey, Platform.iOS, isDebug: Config.IsDebug);
            MapConfig();
        }

        private void MapConfig()
        {
            _nyrisApi.ImageMatching.Language(Config.Language);
            _nyrisApi.ImageMatching.Limit(Config.Limit);
            
            if (Config.NyrisFilterOption != null)
            {
                _nyrisApi.ImageMatching.Filters(obj =>
                {
                    obj.Enabled = Config.NyrisFilterOption.Enabled;
                    Config.NyrisFilterOption.Filters.ForEach(filter =>
                    {
                        obj.AddFilter(filter.Type, filter.Values);
                    });
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
#pragma warning disable CS8601 // Possible null reference assignment.
            _captureButtonImage = UIImage.FromBundle("nyris_custom_capture_icon.png", bundle, trait) ?? UIImage.FromBundle("capture_icon.png", bundle, trait);
            _cropButtonImage = UIImage.FromBundle("nyris_custom_validate_icon.png", bundle, trait) ?? UIImage.FromBundle("validate_icon.png", bundle, trait);
#pragma warning restore CS8601 // Possible null reference assignment.
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

        private void LoadBoundingBox()
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
            try
            {
                var responseDto = await Task.Run(() => matchingService.MatchAsync(bytes).Result);
                InvokeOnMainThread(() =>
                {
                    var nyrisSearcherResult = responseDto.ToNyrisSearcherResult(bytes);
                    var offerEventArgs = new OfferResponseEventArgs(screenshot, _cropBoundingBox.Frame, nyrisSearcherResult);
                    OnOfferAvailable(this, offerEventArgs);
                });
            }
            catch (Exception exception)
            {
                ShowError(Config.DialogErrorTitle, exception.Message, Config.AgreeButtonTitle, null,
                    (action) => { SetCaptureState(); });
                return;
            }
            finally
            {
                InvokeOnMainThread(() =>
                {
                    croppedImage?.Dispose();
                    bytes = null;
                    ScreenshotImage = null;
                    Dismiss();
                });
            }
        }

        protected override void ProcessImage(UIImage image)
        {
            base.ProcessImage(image);
            ScreenshotImage?.Dispose();
            ScreenshotImage = image;
        }

        private void OnCaptureTapped(object? sender, System.EventArgs e)
        {
            if(sender == null) return;
            captureFrameAction((UIButton)sender);
            CameraManager.Stop();
            SetCropState();
         }
        
        private void OnCropTapped(object? sender, System.EventArgs e)
        {
            if(sender == null) return;
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