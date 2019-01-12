using Foundation;
using System;
using AVFoundation;
using Nyris.UI.iOS.Camera;
using Nyris.UI.iOS.Camera.Enum;
using UIKit;

namespace Nyris.UI.iOS
{
    public partial class CameraController : UIViewController
    {
		private CameraManager _cameraManager;
        public CameraController (IntPtr handle) : base (handle)
        {
        }
        
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			_cameraManager = new CameraManager();
            try
            {
                _cameraManager.Setup();
            }
            catch (Exception ex)
            {
                throw ex;
            }
			AddObservers();
		}

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
            if (_cameraManager.AuthorizationResult == CameraAuthorizationResult.Authorized)
            {
                _cameraManager.Show(cameraView);
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
			
			NSNotificationCenter.DefaultCenter.AddObserver (UIApplication.WillResignActiveNotification, ApplicationSuspended, this);
			NSNotificationCenter.DefaultCenter.AddObserver (UIApplication.DidBecomeActiveNotification, ApplicationActivated, this);
			NSNotificationCenter.DefaultCenter.AddObserver (UIApplication.DidEnterBackgroundNotification, ApplicationSuspended, this);
			NSNotificationCenter.DefaultCenter.AddObserver (UIApplication.WillTerminateNotification, ApplicationSuspended, this);
			NSNotificationCenter.DefaultCenter.AddObserver (UIApplication.WillEnterForegroundNotification, ApplicationActivated, this);
		}

		private void RemoveObservers ()
		{
			NSNotificationCenter.DefaultCenter.RemoveObserver(this);
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
		
		void ApplicationActivated(NSNotification notification) {
			if (!_cameraManager.IsRunning ) {
				_cameraManager.Start();
			}
		}
    
		/// properly shutdown/stop camera service when the app is in the background or will be terminated
		void ApplicationSuspended(NSNotification notification) {
        
			if (_cameraManager.IsRunning ) {
				_cameraManager.stop();
			}
        }

        partial void CaptureTapped(UIButton sender)
        {
            throw new NotImplementedException();
        }
    }
}