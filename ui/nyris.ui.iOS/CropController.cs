using Foundation;
using System;
using CoreGraphics;
using Nyris.UI.iOS;
using ObjCRuntime;
using UIKit;
using Nyris.UI.iOS.Camera.Enum;
using Nyris.UI.iOS.Crop;
using Nyris.UI.iOS.ImageOperations;

namespace Nyris.UI.iOS
{
    enum CameraControllerState
    {
        Capture, Crop
    }
    
    public partial class CropController : CameraController
    {
        private CameraControllerState _currentState = CameraControllerState.Capture;
        
        private CropOverlayView _cropBoundingBox;
        private UIImage _captureButtonImage;
        private UIImage _cropButtonImage;
        private UIImageView _screenshotImageView;
        private UIImage _screenshotImage;
            
        public CropController (IntPtr handle) : base (handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            LoadImages();
            SetCaptureState();
            _screenshotImageView = new UIImageView(frame: View.Bounds);
            _screenshotImageView.ContentMode = UIViewContentMode.ScaleAspectFit;
        }

        private void LoadImages()
        {
            
            var bundle = NSBundle.FromClass(new ObjCRuntime.Class(typeof(CropController)));
            _captureButtonImage = UIImage.FromBundle("capture_icon", bundle, null);
            _cropButtonImage = UIImage.FromBundle("validate_icon", bundle, null);
        }
        
        void SetCaptureState()
        {
            _currentState = CameraControllerState.Capture;
            CaptureButton.Enabled = true;
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
        }

        void SetCropState()
        {
            var boxHeight = View.Frame.Height / 2;
            var boxY = (boxHeight) - (boxHeight / 2);
            var frame = new CGRect(15, boxY, View.Frame.Width - 30, boxHeight);
            if (_cropBoundingBox == null) {
                _cropBoundingBox = new CropOverlayView(frame) {
                    IsMovable = true,
                    IsResizable = true
                };
                _cropBoundingBox.BackgroundColor = UIColor.Red.ColorWithAlpha(0.5f);
                View.AddSubview(_cropBoundingBox);
            }
            else
            {
                _cropBoundingBox.Frame = frame;
            }
            _currentState = CameraControllerState.Crop;
            CaptureButton.Enabled = true;
            CaptureButton.SetImage(_cropButtonImage, UIControlState.Normal);
            CaptureButton.TouchUpInside -= OnCaptureTapped;
            CaptureButton.TouchUpInside += OnCropTapped;
            _cropBoundingBox.Hidden = false;
        }

        protected override void ProcessImage(UIImage image)
        {
            base.ProcessImage(image);
            _screenshotImage = image;
        }

        private void OnCaptureTapped(object sender, EventArgs e)
        {
            captureFrameAction((UIButton)sender);
            _cameraManager.Stop();
            SetCropState();
         }
        
        private void OnCropTapped(object sender, EventArgs e)
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

            croppedImage?.SaveToPhotosAlbum((image, error) =>
            {
                Console.WriteLine(1);
            });
            var resizedCroppedImage = ImageHelper.ResizeWithRatio(croppedImage, new CGSize(512, 512));
            resizedCroppedImage?.SaveToPhotosAlbum((image, error) =>
            {
                Console.WriteLine(1);
            });

            SetCaptureState();
            _cameraManager.Start();
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
    }
}