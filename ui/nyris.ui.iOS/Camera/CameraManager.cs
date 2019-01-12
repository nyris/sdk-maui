using System;
using System.Linq;
using AVFoundation;
using CoreAnimation;
using CoreFoundation;
using CoreGraphics;
using CoreVideo;
using Foundation;
using Nyris.UI.iOS.Camera.Enum;
using Nyris.UI.iOS.Camera.EventArgs;
using UIKit;

namespace Nyris.UI.iOS.Camera
{
    public class CameraManager : AVCaptureVideoDataOutputSampleBufferDelegate
    {
        public  DispatchQueue SessionQueue { get; private set; }
        private readonly AVCaptureSession _captureSession;
        private AVCaptureDeviceInput _input;
        private readonly AVCaptureVideoDataOutput _videoOutput;
        private AVCaptureVideoPreviewLayer _videoPreviewLayer;
        private UITapGestureRecognizer _focusTapGesture;
        private CAShapeLayer _circleShape;
        [Weak]
        private UIView _displayView;
        private CameraConfiguration _cameraConfiguration;

        public SessionSetupResult SetupResult { get; private set; } = SessionSetupResult.NotAuthorized;
        public CameraAuthorizationResult AuthorizationResult { get; private set; } = CameraAuthorizationResult.NotDetermined;

        public AVCaptureVideoPreviewLayer VideoPreviewLayer => _videoPreviewLayer;
        private AVCaptureDevice CaptureDevice { get; set; }

        public event EventHandler<DidTapCameraPreviewLayerEventArgs> DidTapCameraPreview ;

        public bool IsRunning => _captureSession?.Running ?? false;

        public CameraManager()
        {
            SessionQueue = new DispatchQueue(label: "session queue");
            _captureSession = new AVCaptureSession();
            _videoOutput = new AVCaptureVideoDataOutput();
            _focusTapGesture = new UITapGestureRecognizer(FocusWhenTapped);
        }

        public void Setup()
        {
	        SessionQueue.DispatchSync(() =>
	        {
		        var config = new CameraConfiguration();
		        config.Preset = AVCaptureSession.PresetMedium;
		        config.AllowTapToFocus = true;
		        ConfigureSession (config);
	        });
        }
        
        void ConfigureSession (CameraConfiguration configuration)
        {
	        _cameraConfiguration = configuration;

            CheckCameraPermission();
            if (AuthorizationResult != CameraAuthorizationResult.Authorized)
			{
                // request camera authorization
				return;
			}

			NSError error = null;

			_captureSession.BeginConfiguration ();
			_captureSession.SessionPreset = _cameraConfiguration.Preset;
	
			// Add video input.
			CaptureDevice = AVCaptureDevice.GetDefaultDevice(AVMediaType.Video);
			if (CaptureDevice == null) {
				// Video capture not supported, abort
				SetupResult = SessionSetupResult.ConfigurationFailed;
				_captureSession.CommitConfiguration();
				throw new Exception(message:"Video recording not supported on this device");
			}
			
			_input = AVCaptureDeviceInput.FromDevice (CaptureDevice, out error);
			if (_input == null)
			{
				SetupResult = SessionSetupResult.ConfigurationFailed;
				_captureSession.CommitConfiguration();
				throw new Exception(message:"Unable to gain input from capture device.");
			}
			
			if (!_captureSession.CanAddInput (_input)) {
				SetupResult = SessionSetupResult.ConfigurationFailed;
				_captureSession.CommitConfiguration();
				throw new Exception(message:@"Could not add video device input to the session.");
			}
	
			_captureSession.AddInput (_input);

			if (!_captureSession.CanSetSessionPreset(_cameraConfiguration.Preset))
			{
				throw new Exception(message:$"can't set {_cameraConfiguration.Preset} as session preset");
			}
			_captureSession.SessionPreset = _cameraConfiguration.Preset;

            var settings = new AVVideoSettingsUncompressed { PixelFormatType = CVPixelFormatType.CV32BGRA };
            _videoOutput.WeakVideoSettings = settings.Dictionary;
            _videoOutput.AlwaysDiscardsLateVideoFrames = true;

            if (_captureSession.CanAddOutput(_videoOutput))
            {
                _captureSession.AddOutput(_videoOutput);
                _videoOutput.SetSampleBufferDelegateQueue(this, SessionQueue);
            }

            _videoPreviewLayer = AVCaptureVideoPreviewLayer.FromSession(_captureSession);
			_captureSession.CommitConfiguration ();
		}

        public void Show(UIView view)
        {
	        if (_captureSession == null)
	        {
		        throw new Exception(message:"Invalid capture session, make sure Setup is called befor displaying a preview");
	        }

            DispatchQueue.MainQueue.DispatchAsync(() =>
            {

                if (_videoPreviewLayer == null)
                {
                    throw new Exception(message: "Invalid videoPreviewLayer");
                }

                if (_videoPreviewLayer.Connection.SupportsVideoOrientation)
                {
                    var statusBarOrientation = UIApplication.SharedApplication.StatusBarOrientation;
                    var initialVideoOrientation = AVCaptureVideoOrientation.Portrait;
                    if (statusBarOrientation != UIInterfaceOrientation.Unknown)
                    {
                        initialVideoOrientation = (AVCaptureVideoOrientation)statusBarOrientation;
                    }

                    _videoPreviewLayer.Connection.VideoOrientation = (AVCaptureVideoOrientation)statusBarOrientation;
                }
                _displayView = view;
                _videoPreviewLayer.VideoGravity = AVLayerVideoGravity.ResizeAspectFill;
                _videoPreviewLayer.Frame = UIScreen.MainScreen.Bounds;
                view.Layer.AddSublayer(_videoPreviewLayer);

                if (view.GestureRecognizers == null || !view.GestureRecognizers.Contains(_focusTapGesture))
                {
                    view.UserInteractionEnabled = true;
                    view.AddGestureRecognizer(_focusTapGesture);
                }

                if (!_captureSession.Running)
                {
                    _captureSession?.StartRunning();
                }
            });
        }
        
        
        public void Start() {
	        _captureSession?.StartRunning();
        }
    
        public void stop() {
	        _captureSession?.StopRunning();
        }
        
        
        public void AddFocusCircle(UIView view, CGPoint point)
        {
	        var circle = this.GenerateFocusCircle(point);
	        circle.Opacity = 0;
	        view.Layer.AddSublayer(circle);
	        _circleShape = circle;
        
	        CATransaction.Begin();
	        var fadeAnimator = CABasicAnimation.FromKeyPath("opacity");
	        fadeAnimator.From = NSNumber.FromFloat(1);
	        fadeAnimator.To =NSNumber.FromFloat(0);
	        fadeAnimator.Duration = 0.5f;
	        fadeAnimator.RemovedOnCompletion = true;
        
	        // Callback function
	        CATransaction.CompletionBlock = () =>
	        {
		        _circleShape.Opacity = 0;
		        _circleShape.RemoveFromSuperLayer();
	        };

	        _circleShape?.AddAnimation(fadeAnimator, key: "opacity");
	        CATransaction.Commit();
        }
        
        private CAShapeLayer GenerateFocusCircle(CGPoint location)
        {
	        var circlePath = UIBezierPath.FromArc(center:location,
		        radius: 40, 
		        startAngle: 0,
		        endAngle:(float)(Math.PI * 2),
		        clockwise:true);

	        var shapeLayer = new CAShapeLayer();
	        shapeLayer.Path = circlePath.CGPath;
	        shapeLayer.FillColor = UIColor.Clear.CGColor;
	        shapeLayer.StrokeColor = UIColor.White.CGColor;
	        shapeLayer.LineWidth = 3.0f;
	        return shapeLayer;
        }
        
        void OnCameraLayerTapped(CGPoint touchLocation)
        {
	        var args = new DidTapCameraPreviewLayerEventArgs(touchLocation);
	        DidTapCameraPreview?.Invoke(this, args);
        }
        
        void FocusWhenTapped(UITapGestureRecognizer sender)
        {
	        if (_displayView == null)
	        {
		        return;
	        }

	        var touchPoint = sender.LocationOfTouch(0, _displayView);
	        OnCameraLayerTapped(touchPoint);
        }

        void CheckCameraPermission()
        {
	        switch (AVCaptureDevice.GetAuthorizationStatus (AVMediaType.Video ))
	        {
		        case AVAuthorizationStatus.Authorized:
			        AuthorizationResult = CameraAuthorizationResult.Authorized;
			        break;

		        case AVAuthorizationStatus.NotDetermined:
			    case AVAuthorizationStatus.Denied:

			        /*
				        The user has not yet been presented with the option to grant
				        video access. We suspend the session queue to delay session
				        setup until the access request has completed.
				        
				        Note that audio access will be implicitly requested when we
				        create an AVCaptureDeviceInput for audio during session setup.
			        */
			        SessionQueue.Suspend ();
			        AVCaptureDevice.RequestAccessForMediaType (AVMediaType.Video, (bool granted) => {
				        AuthorizationResult = !granted ? CameraAuthorizationResult.NotAuthorized : CameraAuthorizationResult.Authorized;
				        SessionQueue.Resume ();
			        });
			        break;
		        default:
		        {
			        // The user has previously denied access.
			        SetupResult = SessionSetupResult.NotAuthorized;
			        AuthorizationResult = CameraAuthorizationResult.NotAuthorized;
			        break;
		        }
	        }
        }
    }
}