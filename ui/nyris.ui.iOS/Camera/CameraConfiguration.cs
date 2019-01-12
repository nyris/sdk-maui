using System;
using Foundation;
using AVFoundation;
using Nyris.UI.iOS.Camera.Enum;

namespace Nyris.UI.iOS.Camera
{
    public struct CameraConfiguration
    {
        public AVCaptureFocusMode FocusMode;
        public bool AllowTapToFocus;
        
        // light
        public TorchMode TorchMode;
        public TorchMode FlashMode;
        public NSString Preset;
    }
}
