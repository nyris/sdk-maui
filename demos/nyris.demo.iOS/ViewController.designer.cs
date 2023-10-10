// WARNING
//
// This file has been generated automatically by Rider IDE
//   to store outlets and actions made in Xcode.
// If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace Nyris.Demo.iOS
{
	partial class ViewController
	{
		[Outlet]
		UIKit.UILabel OfferNumberLabel { get; set; }

		[Outlet]
		UIKit.UIImageView screenshotImageView { get; set; }

		[Action ("OpenNewSearcherSession:")]
		partial void OpenNewSearcherSession (Foundation.NSObject sender);

		[Action ("RestoreSearcherSession:")]
		partial void RestoreSearcherSession (Foundation.NSObject sender);

		void ReleaseDesignerOutlets ()
		{
			if (screenshotImageView != null) {
				screenshotImageView.Dispose ();
				screenshotImageView = null;
			}

			if (OfferNumberLabel != null) {
				OfferNumberLabel.Dispose ();
				OfferNumberLabel = null;
			}

		}
	}
}
