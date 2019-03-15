using System;
using Foundation;
using AVFoundation;
using CoreMotion;
using UIKit;

namespace Nyris.UI.iOS.Camera
{

    public class CameraOrientation
    {
        public bool ShouldUseDeviceOrientation { get; set; } = false;
        private UIDeviceOrientation? deviceOrientation;
        private CMMotionManager coreMotionManager = new CMMotionManager();

        public CameraOrientation()
        {
            coreMotionManager.AccelerometerUpdateInterval = 0.1f;
        }

        public void Start()
        {
            deviceOrientation = UIDevice.CurrentDevice.Orientation;
            coreMotionManager.StartAccelerometerUpdates(NSOperationQueue.MainQueue, (data, error) =>
           {
               if (data == null)
               {
                   return;
               }
               this.HandleAccelerometerUpdate(data);
           });

        }

        public void Stop()
        {
            coreMotionManager.StopAccelerometerUpdates();
            deviceOrientation = null;
        }

        public UIImageOrientation GetImageOrientation()
        {
            if (ShouldUseDeviceOrientation == false || deviceOrientation == null)
            {
                return UIImageOrientation.Right;
            }
            switch (deviceOrientation)
            {
                case UIDeviceOrientation.LandscapeLeft:
                    return UIImageOrientation.Up;
                case UIDeviceOrientation.LandscapeRight:
                    return UIImageOrientation.Down;
                case UIDeviceOrientation.PortraitUpsideDown:
                    return UIImageOrientation.Left;
                default:
                    return UIImageOrientation.Up;
            }
        }
        public AVCaptureVideoOrientation GetPreviewLayerOrientation()
        {
            switch (UIApplication.SharedApplication.StatusBarOrientation)
            {
                case UIInterfaceOrientation.Portrait:
                case UIInterfaceOrientation.Unknown:
                    return AVCaptureVideoOrientation.Portrait;
                case UIInterfaceOrientation.LandscapeLeft:
                    return AVCaptureVideoOrientation.LandscapeLeft;
                case UIInterfaceOrientation.LandscapeRight:
                    return AVCaptureVideoOrientation.LandscapeRight;
                case UIInterfaceOrientation.PortraitUpsideDown:
                    return AVCaptureVideoOrientation.PortraitUpsideDown;
                default:
                    return AVCaptureVideoOrientation.Portrait;
            }
        }

        public AVCaptureVideoOrientation GetVideoOrientation()
        {
            if (ShouldUseDeviceOrientation == false || deviceOrientation == null)
            {
                return AVCaptureVideoOrientation.Portrait;
            }

            switch (deviceOrientation)
            {

                case UIDeviceOrientation.LandscapeLeft:
                    return AVCaptureVideoOrientation.LandscapeRight;
                case UIDeviceOrientation.LandscapeRight:
                    return AVCaptureVideoOrientation.LandscapeLeft;
                case UIDeviceOrientation.PortraitUpsideDown:
                    return AVCaptureVideoOrientation.PortraitUpsideDown;
                default:
                    return AVCaptureVideoOrientation.Portrait;
            }
        }

        private void HandleAccelerometerUpdate(CMAccelerometerData data)
        {
            var absoluteXAcceleration = Math.Abs(data.Acceleration.X);
            var absoluteYAcceleration = Math.Abs(data.Acceleration.Y);

            if (absoluteYAcceleration < absoluteXAcceleration)
            {
                deviceOrientation = data.Acceleration.X > 0 ? UIDeviceOrientation.LandscapeRight : UIDeviceOrientation.LandscapeLeft;
            }
            else
            {
                deviceOrientation = data.Acceleration.Y > 0 ? UIDeviceOrientation.PortraitUpsideDown : UIDeviceOrientation.Portrait;
            }
        }
    }
}



