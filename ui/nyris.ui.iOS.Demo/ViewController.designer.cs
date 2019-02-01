// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;

namespace Nyris.UI.iOS.Demo
{
    [Register ("ViewController")]
    partial class ViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel OfferNumberLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIImageView screenshotImageView { get; set; }

        [Action ("OpenNewSearcherSession:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void OpenNewSearcherSession (UIKit.UIButton sender);

        [Action ("RestoreSearcherSession:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void RestoreSearcherSession (UIKit.UIButton sender);

        void ReleaseDesignerOutlets ()
        {
            if (OfferNumberLabel != null) {
                OfferNumberLabel.Dispose ();
                OfferNumberLabel = null;
            }

            if (screenshotImageView != null) {
                screenshotImageView.Dispose ();
                screenshotImageView = null;
            }
        }
    }
}