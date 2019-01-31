using System;

using UIKit;
using Foundation;
using Nyris.UI.iOS.EventArgs;
using Nyris.UI.iOS;

namespace Nyris.UI.iOS.Demo
{
    public partial class ViewController : UIViewController
    {
        private INyrisSearcher searchService;

        private UIImage _captureButtonImage;
        private UIImage _cropButtonImage;

        protected ViewController(IntPtr handle) : base(handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
  
            _captureButtonImage = UIImage.FromBundle("capture_icon_demo");
            _cropButtonImage = UIImage.FromBundle("crop_image_demo");
            var theme = new AppearanceConfiguration
            {
                CaptureLabelColor = UIColor.Red,
                CropButtonTint = UIColor.Yellow,
                BackButtonTinte = UIColor.Red,
                CaptureButtonTint = UIColor.Green,
                CaptureButtonImage = _captureButtonImage,
                CropButtonImage = _cropButtonImage,
            };

            searchService = NyrisSearcher
                .Builder("API-KEY", this)
                .Theme(theme)
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
