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
    [Register ("CropController")]
    partial class CropController
    {
        [Action ("CaptureTapped:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void CaptureTapped (UIKit.UIButton sender);

        [Action ("CloseTapped:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void CloseTapped (UIKit.UIButton sender);

        [Action ("FlashLightTaped:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void FlashLightTaped (UIKit.UIButton sender);

        void ReleaseDesignerOutlets ()
        {
            if (CloseLabel != null) {
                CloseLabel.Dispose ();
                CloseLabel = null;
            }
        }
    }
}