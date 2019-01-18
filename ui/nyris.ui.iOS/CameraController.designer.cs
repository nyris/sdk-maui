// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;

namespace Nyris.UI.iOS
{
    [Register ("CameraController")]
    partial class CameraController
    {
        [Action ("CaptureTapped:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void CaptureTapped (UIKit.UIButton sender);

        [Action ("CloseTapped:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void CloseTapped (UIKit.UIButton sender);

        void ReleaseDesignerOutlets ()
        {
            if (ActivityIndicator != null) {
                ActivityIndicator.Dispose ();
                ActivityIndicator = null;
            }

            if (CameraView != null) {
                CameraView.Dispose ();
                CameraView = null;
            }

            if (CaptureButton != null) {
                CaptureButton.Dispose ();
                CaptureButton = null;
            }

            if (CaptureLabel != null) {
                CaptureLabel.Dispose ();
                CaptureLabel = null;
            }

            if (CloseButton != null) {
                CloseButton.Dispose ();
                CloseButton = null;
            }

            if (DarkView != null) {
                DarkView.Dispose ();
                DarkView = null;
            }

            if (NetworkStatusLabel != null) {
                NetworkStatusLabel.Dispose ();
                NetworkStatusLabel = null;
            }
        }
    }
}