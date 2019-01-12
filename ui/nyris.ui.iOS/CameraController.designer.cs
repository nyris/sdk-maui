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
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIActivityIndicatorView activityIndicator { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView cameraView { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton captureButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel captureLable { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView darkView { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel networkStatusLable { get; set; }

        [Action ("CaptureTapped:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void CaptureTapped (UIKit.UIButton sender);

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