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
        }

        private void SearchServiceOnOfferAvailable(object sender, OfferResponseEventArgs e)
        {
            if (e == null || (e.OfferResponse == null && e.OfferJson == null) )
            {
                OfferNumberLabel.Text = "No offers found";
                return;
            }

            if (e.OfferResponse != null)
            {
                OfferNumberLabel.Text = $"Offers found {e.OfferResponse.Offers.Count}";
            }
            
            if (e.OfferJson != null)
            {
                OfferNumberLabel.Text = $"Offers found as Json content";
            }
            screenshotImageView.Image = e.Screenshot;
        }

        partial void RestoreSearcherSession(UIButton sender)
        {
            searchService.Start(true);
        }

        partial void OpenNewSearcherSession(UIButton sender)
        {
            searchService.Start();
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
            OfferNumberLabel.Hidden = false;
        }

    }
}
