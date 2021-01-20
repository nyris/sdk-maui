using System;
using AVFoundation;
using CoreFoundation;
using CoreGraphics;
using CoreImage;
using CoreMedia;
using CoreVideo;
using Foundation;
using Nyris.UI.Common;
using Nyris.UI.iOS;
using Nyris.UI.iOS.Camera;
using Nyris.UI.iOS.Camera.Enum;
using Nyris.UI.iOS.Camera.EventArgs;
using Nyris.UI.iOS.Crop;
using ObjCRuntime;
using UIKit;

namespace Nyris.UI.iOS
{
    public partial class CameraController : UIViewController
    {
        [Outlet] protected UIKit.UIActivityIndicatorView ActivityIndicator { get; set; }

        [Outlet] protected UIKit.UIView CameraView { get; set; }

        [Outlet] protected UIKit.UIButton CaptureButton { get; set; }

        [Outlet] protected UIKit.UILabel CaptureLabel { get; set; }

        [Outlet] protected UIKit.UIButton CloseButton { get; set; }

        [Outlet] protected UIKit.UILabel CloseLabel { get; set; }

        [Outlet] protected UIKit.UIButton FlashLightButton { get; set; }

        [Outlet] protected UIKit.UIView DarkView { get; set; }

        [Outlet] protected UIKit.UILabel NetworkStatusLabel { get; set; }

        protected CameraManager CameraManager;

        [Weak] private CVImageBuffer _videoFramePixelBuffer;

        protected AppearanceConfiguration _theme;

        public NyrisSearcherConfig Config { get; private set; }

        public CameraController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            CameraManager = new CameraManager();
            CameraManager.OnAuthorizationChange += CameraManagerOnOnAuthorizationChange;
            CameraManager.OnFrameCapture += CameraManagerOnOnFrameCapture;
            if (Config != null)
            {
                CaptureLabel.Text = Config.CaptureLabelText;
                CloseLabel.Text = Config.BackLabelText;
            }
        }

        public virtual void Configure(NyrisSearcherConfig config, AppearanceConfiguration theme)
        {
            Config = config;
            _theme = theme;
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
            AddObservers();
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            CameraManager.CheckCameraPermission();
        }

        public override void ViewDidDisappear(bool animated)
        {
            base.ViewDidDisappear(animated);
            RemoveObservers();
        }

        private void CameraManagerOnOnFrameCapture(object sender, FrameCaptureEventArgs e)
        {
            _videoFramePixelBuffer?.Dispose();
            _videoFramePixelBuffer = e.FrameBuffer;
        }

        protected void ShowError(string errorTitle, string message, string okTitle, string cancelTitle,
            Action<UIAlertAction> okAction, Action<UIAlertAction> cancelAction = null)
        {
            DispatchQueue.MainQueue.DispatchAsync(() =>
            {
                var alertController = UIAlertController.Create(errorTitle, message, UIAlertControllerStyle.Alert);
                if (!string.IsNullOrEmpty(cancelTitle))
                {
                    var cancelActionButton = UIAlertAction.Create(cancelTitle, UIAlertActionStyle.Cancel, cancelAction);
                    alertController.AddAction(cancelActionButton);
                }

                if (!string.IsNullOrEmpty(okTitle))
                {
                    var settingsAction = UIAlertAction.Create(okTitle, UIAlertActionStyle.Default, okAction);
                    alertController.AddAction(settingsAction);
                }

                PresentViewController(alertController, true, null);
            });
        }

        private void CameraManagerOnOnAuthorizationChange(object sender, CameraAuthorizationEventArgs e)
        {
            if (e.Authorization != CameraAuthorizationResult.Authorized)
            {
                var errorTitle = Config.DialogErrorTitle ?? "Authorization Error";
                var message = Config.CameraPermissionRequestIfDeniedMessage ??
                              "Please authorize camera access to use this app";
                var cancelTitle = Config.NegativeButtonText ?? "Cancel";
                var settingsTitle = NSBundle.MainBundle.GetLocalizedString(@"Settings", @"Settings");

                ShowError(errorTitle, message, settingsTitle, cancelTitle,
                    (action) =>
                    {
                        UIApplication.SharedApplication.OpenUrl(new NSUrl(UIApplication.OpenSettingsUrlString));
                    }, (action) => { Dismiss(); });
                return;
            }

            if (CameraManager.SetupResult != SessionSetupResult.Success)
            {
                try
                {
                    CameraManager.Setup(useDeviceOrientation: true);
                }
                catch (Exception)
                {
                    DispatchQueue.MainQueue.DispatchAsync(() =>
                    {
                        var errorTitle = Config.DialogErrorTitle ?? "Configuration Error";
                        var message = Config.ConfigurationFailedErrorMessage ?? "Unable to capture media";
                        var okTitle = Config.PositiveButtonText ?? "Ok";

                        ShowError(errorTitle, message, okTitle, null, (action) => { Dismiss(); });
                    });
                }
            }

            if (!CameraManager.IsRunning)
            {
                CameraManager.Show(CameraView);
            }
        }

        private UIImage GetScreenshot()
        {
            if (_videoFramePixelBuffer == null)
            {
                return null;
            }

            var pixelBuffer = Runtime.GetINativeObject<CVPixelBuffer>(_videoFramePixelBuffer.Handle, false);
            var bounds = UIScreen.MainScreen.Bounds;
            UIGraphics.BeginImageContextWithOptions(bounds.Size, false, UIScreen.MainScreen.Scale);

            var viewContainer = new UIView(bounds);
            var ciImage = new CIImage(pixelBuffer);
            var preview = new UIImage(ciImage: ciImage);
            var imageView = new UIImageView(frame: bounds)
            {
                Image = preview,
                ContentMode = UIViewContentMode.ScaleAspectFill
            };
            viewContainer.BackgroundColor = UIColor.Blue;
            viewContainer.AddSubview(imageView);
            viewContainer.DrawViewHierarchy(bounds, true);
            var image = UIGraphics.GetImageFromCurrentImageContext();
            UIGraphics.EndImageContext();
            imageView.RemoveFromSuperview();
            
            ciImage?.Dispose();
            imageView?.Dispose();
            preview?.Dispose();
            _videoFramePixelBuffer?.Dispose();
            pixelBuffer?.Dispose();
            _videoFramePixelBuffer = null;
            GC.Collect();
            return image;
        }

        private void AddObservers()
        {
            NSNotificationCenter.DefaultCenter.AddObserver(AVCaptureSession.RuntimeErrorNotification,
                SessionRuntimeError, this);

            /*
                      A session can only run when the app is full screen. It will be interrupted
                      in a multi-app layout, introduced in iOS 9, see also the documentation of
                      AVCaptureSessionInterruptionReason. Add observers to handle these session
                      interruptions and show a preview is paused message. See the documentation
                      of AVCaptureSessionWasInterruptedNotification for other interruption reasons.
                  */
            NSNotificationCenter.DefaultCenter.AddObserver(AVCaptureSession.WasInterruptedNotification,
                SessionWasInterrupted, this);
            NSNotificationCenter.DefaultCenter.AddObserver(AVCaptureSession.InterruptionEndedNotification,
                SessionInterruptionEnded, this);

            NSNotificationCenter.DefaultCenter.AddObserver(UIApplication.DidBecomeActiveNotification,
                ApplicationActivated);
            NSNotificationCenter.DefaultCenter.AddObserver(UIApplication.DidEnterBackgroundNotification,
                ApplicationSuspended);
        }

        private void RemoveObservers()
        {
            NSNotificationCenter.DefaultCenter.RemoveObserver(this);
            CameraManager.OnAuthorizationChange -= CameraManagerOnOnAuthorizationChange;
            CameraManager.UnsubscribeFromDeviceOrientation();
        }

        void SessionRuntimeError(NSNotification notification)
        {
            NSError error = notification.UserInfo[AVCaptureSession.ErrorKey] as NSError;
            if (error == null)
            {
                return;
            }

            Console.WriteLine($"Capture session runtime error: {error}");

            /*
                      Automatically try to restart the session running if media services were
                      reset and the last start running succeeded. Otherwise, enable the user
                      to try to resume the session running.
                  */
            if (error.Code == (int) AVError.MediaServicesWereReset)
            {
                CameraManager.SessionQueue.DispatchAsync(() => { });
            }
        }

        void SessionWasInterrupted(NSNotification notification)
        {
            /*
                      In some scenarios we want to enable the user to resume the session running.
                      For example, if music playback is initiated via control center while
                      using AVCam, then the user can let AVCam resume
                      the session running, which will stop music playback. Note that stopping
                      music playback in control center will not automatically resume the session
                      running. Also note that it is not always possible to resume, see -[resumeInterruptedSession:].
                  */
        }

        void SessionInterruptionEnded(NSNotification notification)
        {
            Console.WriteLine(@"Capture session interruption ended");
        }

        protected virtual void ApplicationActivated(NSNotification notification)
        {
            if (!CameraManager.IsRunning && CameraManager.SetupResult == SessionSetupResult.Success)
            {
                CameraManager.CheckCameraPermission();
            }
        }

        /// properly shutdown/stop camera service when the app is in the background or will be terminated
        protected virtual void ApplicationSuspended(NSNotification notification)
        {
            if (CameraManager.IsRunning)
            {
                CameraManager.Stop();
            }
        }

        protected virtual void ProcessImage(UIImage image)
        {
        }

        partial void CaptureTapped(UIButton sender)
        {
        }

        protected void captureFrameAction(UIButton sender)
        {
            if (CameraManager.AuthorizationResult != CameraAuthorizationResult.Authorized)
            {
                CameraManager.CheckCameraPermission();
                return;
            }

            // saving the picture may take some time, lock to avoid spam the button
            sender.Enabled = false;

            var screenshot = GetScreenshot();
            ProcessImage(image: screenshot);
        }

        partial void CloseTapped(UIButton sender)
        {
            this.PresentingViewController?.DismissViewController(true, null);
        }

        protected virtual void Dismiss()
        {
            PresentingViewController?.DismissViewController(true, null);
        }

        partial void FlashLightTaped(UIButton sender)
        {
            CameraManager.ToggleTorch();
            FlashLightButton.Selected = CameraManager.IsTorchActive == AVCaptureTorchMode.On;
        }
    }
}