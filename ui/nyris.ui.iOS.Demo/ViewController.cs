using System;

using UIKit;
using Foundation;
using Nyris.UI.iOS;
using Nyris.UI.iOS.EventArgs;


namespace Nyris.UI.iOS.Demo
{
    public partial class ViewController : UIViewController
    {
        private INyrisSearcher searchService;
        protected ViewController(IntPtr handle) : base(handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            searchService = NyrisSearcher
                .Builder("API-KEY", this)
                .CaptureLabelText("My Capture label.")
                .CameraPermissionDeniedErrorMessage("You can not use this component until you activate the camera permission!")
                .CameraPermissionRequestIfDeniedMessage("Please authorize camera usage in settings.")
                .ConfigurationFailedErrorMessage("Camera setup failed")
                .DialogErrorTitle("Error Title")
                .AgreeButtonTitle("OK")
                .CancelButtonTitle("Cancel")
                .CategoryPrediction();
            searchService.OfferAvailable += SearchServiceOnOfferAvailable;
            searchService.RequestFailed += SearchServiceOnRequestFailed;
        }

        private void SearchServiceOnRequestFailed(object sender, Exception e)
        {
            throw new NotImplementedException();
        }

        private void SearchServiceOnOfferAvailable(object sender, OfferResponseEventArgs e)
        {
            throw new NotImplementedException();
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
            searchService.Start();
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }
    }
}
