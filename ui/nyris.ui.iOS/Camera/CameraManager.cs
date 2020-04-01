using System;
using System.Linq;
using AVFoundation;
using CoreAnimation;
using CoreFoundation;
using CoreGraphics;
using CoreMedia;
using CoreVideo;
using Foundation;
using Nyris.UI.iOS.Camera.Enum;
using Nyris.UI.iOS.Camera.EventArgs;
using ObjCRuntime;
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
        private readonly UITapGestureRecognizer _focusTapGesture;
        private CAShapeLayer _circleShape;
        [Weak]
        private UIView _displayView;
        private CameraConfiguration _cameraConfiguration;
        public AVCaptureStillImageOutput StillImageOutput { get; set; }
        
        public SessionSetupResult SetupResult { get; private set; } = SessionSetupResult.NotAuthorized;
        private CameraAuthorizationResult _authorizationResult = CameraAuthorizationResult.NotDetermined;
        public CameraAuthorizationResult AuthorizationResult
        {
	        get => _authorizationResult;
	        private set
	        {
		        _authorizationResult = value;
		        OnAuthorizationChanged(value);
	        }
        }
        private AVCaptureDevice CaptureDevice { get; set; }
        private readonly CameraOrientation cameraOrientationService = new CameraOrientation();

        public event EventHandler<DidTapCameraPreviewLayerEventArgs> DidTapCameraPreview ;
        public event EventHandler<CameraAuthorizationEventArgs> OnAuthorizationChange ;
        public event EventHandler<FrameCaptureEventArgs> OnFrameCapture ;

        public bool IsRunning => _captureSession?.Running ?? false;

        public bool ShouldUseDeviceOrientation
        {
            get
            {
                return cameraOrientationService.ShouldUseDeviceOrientation;
            }
            private set
            {
                cameraOrientationService.ShouldUseDeviceOrientation = value;
            }

        }

        public CameraManager()
        {
            SessionQueue = new DispatchQueue(label: "session queue");
            _captureSession = new AVCaptureSession();
            _videoOutput = new AVCaptureVideoDataOutput();
            _focusTapGesture = new UITapGestureRecognizer(FocusWhenTapped);
            StillImageOutput = new AVCaptureStillImageOutput ();
        }

        public void Setup(bool useDeviceOrientation = false)
        {
            var config = new CameraConfiguration { Preset = AVCaptureSession.PresetHigh, AllowTapToFocus = true };
            ConfigureSession(config);
            ShouldUseDeviceOrientation = useDeviceOrientation;
            SubscribeToDeviceOrientation();
        }
        
        void ConfigureSession (CameraConfiguration configuration)
        {
	        _cameraConfiguration = configuration;
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
			
			_input = AVCaptureDeviceInput.FromDevice (CaptureDevice, out var error);
			if (_input == null || error != null)
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
				SetupResult = SessionSetupResult.ConfigurationFailed;
				_captureSession.CommitConfiguration();
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
                var videoOutputConnection = _videoOutput.ConnectionFromMediaType(AVMediaType.Video);
                if (videoOutputConnection == null || !videoOutputConnection.SupportsVideoOrientation)
                {
                    SetupResult = SessionSetupResult.ConfigurationFailed;
                    _captureSession.CommitConfiguration();
                    throw new Exception(message: "Video connection doesn't support orientation.");
                }
                videoOutputConnection.VideoOrientation = cameraOrientationService.GetVideoOrientation();
            }

            _videoPreviewLayer = AVCaptureVideoPreviewLayer.FromSession(_captureSession);
            _videoPreviewLayer.Session = _captureSession;

            var availableCodec = StillImageOutput.AvailableImageDataCodecTypes.FirstOrDefault();
            if (availableCodec != null)
            {
	            var dict = new NSMutableDictionary ();
	            dict[AVVideo.CodecKey] = new NSString(availableCodec);
	            StillImageOutput.OutputSettings = dict;

	            if(_captureSession.CanAddOutput(StillImageOutput))
	            {
		            _captureSession.AddOutput(StillImageOutput);
	            }
            }
            
            _captureSession.CommitConfiguration ();
			SetupResult = SessionSetupResult.Success;
		}

        public void Show(UIView view)
        {
	        if (_captureSession == null)
	        {
		        throw new Exception(message:"Invalid capture session, make sure Setup is called before displaying a preview");
	        }

            DispatchQueue.MainQueue.DispatchAsync(() =>
            {

                if (_videoPreviewLayer == null)
                {
                    throw new Exception(message: "Invalid videoPreviewLayer");
                }
                _displayView = view;
                _videoPreviewLayer.VideoGravity = AVLayerVideoGravity.ResizeAspectFill;
                _videoPreviewLayer.Frame = UIScreen.MainScreen.Bounds;
                _videoPreviewLayer.RemoveFromSuperLayer();
                if (_videoPreviewLayer.Connection.SupportsVideoOrientation)
                {
                    _videoPreviewLayer.Connection.VideoOrientation = cameraOrientationService.GetPreviewLayerOrientation();
                }
                _displayView.Layer.AddSublayer(_videoPreviewLayer);

                if (_displayView.GestureRecognizers == null || !_displayView.GestureRecognizers.Contains(_focusTapGesture))
                {
	                _displayView.UserInteractionEnabled = true;
	                _displayView.AddGestureRecognizer(_focusTapGesture);
                }

                if (!_captureSession.Running)
                {

	                this.Start();
                }
            });
        }
        
        
        public void Start()
        {
            //_captureSession?.StartRunning();

            SessionQueue.DispatchAsync(() => { _captureSession?.StartRunning(); });
        }
    
        public void Stop()
        {
            //_captureSession?.StopRunning();
            SessionQueue.DispatchAsync(() => { _captureSession?.StopRunning(); });
        }
        
        
        public void AddFocusCircle(UIView view, CGPoint point)
        {
	        var circle = GenerateFocusCircle(point);
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
        
        private static CAShapeLayer GenerateFocusCircle(CGPoint location)
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
        
        private void OnCameraLayerTapped(CGPoint touchLocation)
        {
	        var args = new DidTapCameraPreviewLayerEventArgs(touchLocation);
	        DidTapCameraPreview?.Invoke(this, args);
        }
        
        private void OnAuthorizationChanged(CameraAuthorizationResult authorization)
        {
	        var args = new CameraAuthorizationEventArgs(authorization);
	        OnAuthorizationChange?.Invoke(this, args);
        }
        
        private void FocusWhenTapped(UITapGestureRecognizer sender)
        {
	        if (_displayView == null)
	        {
		        return;
	        }

	        var touchPoint = sender.LocationOfTouch(0, _displayView);
	        OnCameraLayerTapped(touchPoint);
        }

        public void CheckCameraPermission()
        {
	        switch (AVCaptureDevice.GetAuthorizationStatus (AVMediaType.Video ))
	        {
		        case AVAuthorizationStatus.Authorized:
			        this.AuthorizationResult = CameraAuthorizationResult.Authorized;
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
				        this.AuthorizationResult = !granted ? CameraAuthorizationResult.NotAuthorized : CameraAuthorizationResult.Authorized;
                        
				        SessionQueue.Resume ();
			        });
			        break;
		        case AVAuthorizationStatus.Restricted:
			        this.AuthorizationResult = CameraAuthorizationResult.Restricted;
			        break;
		        default:
		        {
			        // The user has previously denied access. 
			        this.AuthorizationResult = CameraAuthorizationResult.NotAuthorized;
			        break;
		        }
	        }
        }

        public override void DidOutputSampleBuffer(AVCaptureOutput captureOutput, CMSampleBuffer sampleBuffer, AVCaptureConnection connection)
        {

            try
            {

                var buffer = sampleBuffer.GetImageBuffer();
                var frameEventArgs = new FrameCaptureEventArgs(buffer);
                OnFrameCapture?.Invoke(this, frameEventArgs);

                sampleBuffer.Dispose();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }


        private void SubscribeToDeviceOrientation()
        {
            if (ShouldUseDeviceOrientation == false)
            {
                return;
            }
            NSNotificationCenter.DefaultCenter.AddObserver(UIDevice.OrientationDidChangeNotification, DeviceOrientationDidChange);
            cameraOrientationService.Start();
        }

        private void DeviceOrientationDidChange(NSNotification notification)
        {
            if (_videoPreviewLayer == null || 
                _videoPreviewLayer.Connection == null || 
                _videoPreviewLayer.Connection.SupportsVideoOrientation == false)
            {
                return;
            }
            var videoOutputConnection = _videoOutput.ConnectionFromMediaType(AVMediaType.Video);
            if (videoOutputConnection == null)
            {
                return;
            }
            videoOutputConnection.VideoOrientation = cameraOrientationService.GetVideoOrientation();
            var deviceOrientation = UIDevice.CurrentDevice.Orientation;
            switch (deviceOrientation)
            {
                case UIDeviceOrientation.Portrait:
                    updatePreviewLayer(_videoPreviewLayer.Connection, AVCaptureVideoOrientation.Portrait);
                    break;
                case UIDeviceOrientation.LandscapeRight:
                    updatePreviewLayer(_videoPreviewLayer.Connection, AVCaptureVideoOrientation.LandscapeLeft);
                    break;
                case UIDeviceOrientation.LandscapeLeft:
                    updatePreviewLayer(_videoPreviewLayer.Connection, AVCaptureVideoOrientation.LandscapeRight);
                    break;
                case UIDeviceOrientation.PortraitUpsideDown:
                    updatePreviewLayer(_videoPreviewLayer.Connection, AVCaptureVideoOrientation.PortraitUpsideDown);
                    break;
                default:
                    updatePreviewLayer(_videoPreviewLayer.Connection, AVCaptureVideoOrientation.Portrait);
                    break;
            }
        }

        private void updatePreviewLayer(AVCaptureConnection layer, AVCaptureVideoOrientation newOrientation)
        {
            if (_displayView == null || _videoPreviewLayer == null )
            {
                return;
            }

            if(layer.SupportsVideoOrientation == false)
            {
                layer.VideoOrientation = AVCaptureVideoOrientation.Portrait;
            }
            else
            {
                layer.VideoOrientation = newOrientation;
            }
            _videoPreviewLayer.Frame = _displayView.Bounds;
        }

        public void UnsubscribeFromDeviceOrientation()
        {
            if (ShouldUseDeviceOrientation == false)
            {
                return;
            }
            cameraOrientationService.Stop();
            NSNotificationCenter.DefaultCenter.RemoveObserver(this);
        }
    }
}