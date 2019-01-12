using System;
using Foundation;
using AVFoundation;
using Nyris.Ui.iOS.Camera.Enum;

namespace Nyris.Ui.iOS.Camera
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
