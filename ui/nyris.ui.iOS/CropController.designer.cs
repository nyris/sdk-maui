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

namespace Nyris.Ui.iOS
{
    [Register ("CropController")]
    partial class CropController
    {
        [Action ("CaptureTapped:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void CaptureTapped (UIKit.UIButton sender);

        [Action ("CloseTapped:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void CloseTapped (UIKit.UIButton sender);

        void ReleaseDesignerOutlets ()
        {
            if (activityIndicator != null) {
                activityIndicator.Dispose ();
                activityIndicator = null;
            }

            if (cameraView != null) {
                cameraView.Dispose ();
                cameraView = null;
            }

            if (captureButton != null) {
                captureButton.Dispose ();
                captureButton = null;
            }

            if (captureLable != null) {
                captureLable.Dispose ();
                captureLable = null;
            }

            if (CloseButton != null) {
                CloseButton.Dispose ();
                CloseButton = null;
            }

            if (darkView != null) {
                darkView.Dispose ();
                darkView = null;
            }

            if (networkStatusLable != null) {
                networkStatusLable.Dispose ();
                networkStatusLable = null;
            }
        }
    }
}