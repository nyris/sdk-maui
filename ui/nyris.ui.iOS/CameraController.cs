using Foundation;
using System;
using AVFoundation;
using CoreFoundation;
using CoreGraphics;
using CoreImage;
using CoreMedia;
using CoreVideo;
using Nyris.UI.iOS.Camera;
using Nyris.UI.iOS.Camera.Enum;
using Nyris.UI.iOS.Camera.EventArgs;
using UIKit;
using ObjCRuntime;
using Nyris.Ui.iOS.Crop;

namespace Nyris.UI.iOS
{
    public partial class CameraController : UIViewController
    {
	    [Outlet]
	    protected UIKit.UIActivityIndicatorView activityIndicator { get; set; }

	    [Outlet]
	    protected UIKit.UIView cameraView { get; set; }

	    [Outlet]
	    protected UIKit.UIButton captureButton { get; set; }

	    [Outlet]
	    protected UIKit.UILabel captureLable { get; set; }

	    [Outlet]
	    protected UIKit.UIButton CloseButton { get; set; }

	    [Outlet]
	    protected UIKit.UIView darkView { get; set; }

	    [Outlet]
	    protected UIKit.UILabel networkStatusLable { get; set; }
	    
		protected CameraManager _cameraManager;
        [Weak]
		private CVImageBuffer _videoFramePixelBuffer;

		public UIButton CaptureButton => this.captureButton;
        public CameraController (IntPtr handle) : base (handle)
        {
        }
        
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			_cameraManager = new CameraManager();
            _cameraManager.OnAuthorizationChange += CameraManagerOnOnAuthorizationChange;
            _cameraManager.OnFrameCapture += CameraManagerOnOnFrameCapture;
        }

		public override void ViewDidAppear(bool animated)
		{

			base.ViewDidAppear(animated);
			_cameraManager.CheckCameraPermission();
			AddObservers();
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
		
		private void CameraManagerOnOnAuthorizationChange(object sender, CameraAuthorizationEventArgs e)
		{
            if (e.Authorization != CameraAuthorizationResult.Authorized)
            {
                DispatchQueue.MainQueue.DispatchAsync(() =>
                {
                    const string errorTitle = "Authorization Error";
                    const string message = "Please authorize camera access to use this app";
                    const string okTitle = "Cancel";
                    var settingsTitle = NSBundle.MainBundle.GetLocalizedString(@"Settings", @"Settings");
                    var alertController = UIAlertController.Create(errorTitle, message, UIAlertControllerStyle.Alert);
                    var cancelAction = UIAlertAction.Create(okTitle, UIAlertActionStyle.Cancel, null);
                    alertController.AddAction(cancelAction);
                    // Provide quick access to Settings.
                    var settingsAction = UIAlertAction.Create(settingsTitle, UIAlertActionStyle.Default, (action) => {
                        UIApplication.SharedApplication.OpenUrl(new NSUrl(UIApplication.OpenSettingsUrlString));
                    });
                    alertController.AddAction(settingsAction);
                    PresentViewController(alertController, true, null);
                });
                return;
            }

            if (_cameraManager.SetupResult != SessionSetupResult.Success)
            {
	            try
	            {
		            _cameraManager.Setup();
	            }
	            catch(Exception ex) 
	            {
		            DispatchQueue.MainQueue.DispatchAsync(() =>
		            {
			            const string errorTitle = "Configuration Error";
			            const string message = "Unable to capture media";
			            const string okTitle = "Ok";
                        Console.WriteLine(ex.Message);
			            var alertController =
				            UIAlertController.Create(errorTitle, message, UIAlertControllerStyle.Alert);
			            var cancelAction = UIAlertAction.Create(okTitle, UIAlertActionStyle.Cancel, null);
			            alertController.AddAction(cancelAction);
			            PresentViewController(alertController, true, null);
		            });
	            }
            }
            
            
            if (!_cameraManager.IsRunning)
            {
	            _cameraManager.Show(cameraView);
            }
	            
        }

		public UIImage GetScreenshoot()
		{

            if (_videoFramePixelBuffer == null)
            {
                return null;
            }
            var pixelBuffer = Runtime.GetINativeObject<CVPixelBuffer>(_videoFramePixelBuffer.Handle, false);
            var bounds = View.Bounds;
            UIGraphics.BeginImageContextWithOptions(bounds.Size, false, UIScreen.MainScreen.Scale);

            var ciImage = new CIImage(pixelBuffer);
            var preview = new UIImage(ciImage: ciImage);
            var imageView = new UIImageView(frame: bounds) { Image = preview };
            imageView.ContentMode = UIViewContentMode.ScaleAspectFit;
            View.AddSubview(imageView);
            View.DrawViewHierarchy(bounds, true);
            var image = UIGraphics.GetImageFromCurrentImageContext();
            UIGraphics.EndImageContext();
            imageView.RemoveFromSuperview();

            ciImage.Dispose();
            imageView.Dispose();
            preview.Dispose();
            _videoFramePixelBuffer.Dispose();
            pixelBuffer.Dispose();
            _videoFramePixelBuffer = null;
            pixelBuffer = null;
            imageView = null;
            preview = null;
            ciImage = null;
            //GC.Collect();
            return image;

        }
		
		private UIImage GetImageFromSampleBuffer(CMSampleBuffer sampleBuffer) {

			// Get a pixel buffer from the sample buffer
			using (var pixelBuffer = sampleBuffer.GetImageBuffer () as CVPixelBuffer) {
				// Lock the base address
				pixelBuffer.Lock (CVOptionFlags.None);

				// Prepare to decode buffer
				var flags = CGBitmapFlags.PremultipliedFirst | CGBitmapFlags.ByteOrder32Little;

				// Decode buffer - Create a new colorspace
				using (var cs = CGColorSpace.CreateDeviceRGB ()) {

					// Create new context from buffer
					using (var context = new CGBitmapContext (pixelBuffer.BaseAddress,
						pixelBuffer.Width,
						pixelBuffer.Height,
						8,
						pixelBuffer.BytesPerRow,
						cs,
						(CGImageAlphaInfo)flags)) {

						// Get the image from the context
						using (var cgImage = context.ToImage ()) {

							// Unlock and return image
							pixelBuffer.Unlock (CVOptionFlags.None);
							return UIImage.FromImage (cgImage);
						}
					}
				}
			}
		}

        private void AddObservers ()
		{
			NSNotificationCenter.DefaultCenter.AddObserver (AVCaptureSession.RuntimeErrorNotification, SessionRuntimeError, this);

			/*
				A session can only run when the app is full screen. It will be interrupted
				in a multi-app layout, introduced in iOS 9, see also the documentation of
				AVCaptureSessionInterruptionReason. Add observers to handle these session
				interruptions and show a preview is paused message. See the documentation
				of AVCaptureSessionWasInterruptedNotification for other interruption reasons.
			*/
			NSNotificationCenter.DefaultCenter.AddObserver (AVCaptureSession.WasInterruptedNotification, SessionWasInterrupted, this);
			NSNotificationCenter.DefaultCenter.AddObserver (AVCaptureSession.InterruptionEndedNotification, SessionInterruptionEnded, this);
			
			NSNotificationCenter.DefaultCenter.AddObserver (UIApplication.DidBecomeActiveNotification, ApplicationActivated);
			NSNotificationCenter.DefaultCenter.AddObserver (UIApplication.DidEnterBackgroundNotification, ApplicationSuspended);
		}

		private void RemoveObservers ()
		{
			NSNotificationCenter.DefaultCenter.RemoveObserver(this);
			_cameraManager.OnAuthorizationChange -= CameraManagerOnOnAuthorizationChange;
		}

		void SessionRuntimeError (NSNotification notification)
		{
			NSError error = notification.UserInfo[AVCaptureSession.ErrorKey] as NSError;
			if (error == null) {
				return;
			}
			Console.WriteLine ($"Capture session runtime error: {error}");

			/*
				Automatically try to restart the session running if media services were
				reset and the last start running succeeded. Otherwise, enable the user
				to try to resume the session running.
			*/
			if (error.Code == (int)AVError.MediaServicesWereReset)
			{
				_cameraManager.SessionQueue.DispatchAsync (() => {
					
				} );
			}
		}

		void SessionWasInterrupted (NSNotification notification)
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

		void SessionInterruptionEnded (NSNotification notification)
		{
			Console.WriteLine (@"Capture session interruption ended");
		}
		
		protected virtual void ApplicationActivated(NSNotification notification) {
            if (!_cameraManager.IsRunning && _cameraManager.SetupResult == SessionSetupResult.Success)
            {
                _cameraManager.CheckCameraPermission();
            }

        }

        /// properly shutdown/stop camera service when the app is in the background or will be terminated
        protected virtual void ApplicationSuspended(NSNotification notification) {
        
			if (_cameraManager.IsRunning ) {
				_cameraManager.Stop();
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
	        if (_cameraManager.AuthorizationResult != CameraAuthorizationResult.Authorized)
	        {
		        _cameraManager.CheckCameraPermission();
		        return;
	        }

	        // saving the picture may take some time, lock to avoid spam the button
	        sender.Enabled = false;

	        var screenshot = GetScreenshoot();
	        ProcessImage(image: screenshot);
        }

        partial void CloseTapped(UIButton sender)
        {
            this.PresentingViewController?.DismissViewController(true, null);
        }
    }
}