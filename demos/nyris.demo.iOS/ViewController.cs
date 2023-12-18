using System;
using Foundation;
using Nyris.Api.Model;
using Nyris.UI.Common;
using Nyris.UI.iOS;
using Nyris.UI.iOS.EventArgs;
using ObjCRuntime;
using UIKit;

namespace Nyris.Demo.iOS;

[Register("ViewController")]
public partial class ViewController : UIViewController
{
    
    private INyrisSearcher searchService;
    private UIImage _captureButtonImage;
    private UIImage _cropButtonImage;
    
    public ViewController() : base()
    {
    }

    public ViewController(IntPtr handle) : base(handle)
    {
        
    }
    
    protected ViewController (NativeHandle handle)
        : base (handle)
    {
    }
    
    public override void ViewDidLoad()
    {
        base.ViewDidLoad();
        
         _captureButtonImage = UIImage.FromFile("capture_custom");
        _cropButtonImage = UIImage.FromBundle("crop_custom");

        var theme = new AppearanceConfiguration
        {
            CaptureLabelColor = UIColor.Red,
            CropButtonTint = UIColor.Yellow,
            BackButtonTint = UIColor.Red,
            CaptureButtonTint = UIColor.Green,
            CaptureButtonImage = _captureButtonImage,
            CropButtonImage = _cropButtonImage,
            //FlashLightOffButtonImage = _captureButtonImage,
            //FlashLightOnButtonImage = _captureButtonImage,
        };

        var apiKey = Environment.GetEnvironmentVariable("API_KEY") ?? "";
        searchService = NyrisSearcher
            .Builder(apiKey, this)
            .Theme(theme)
            .CaptureLabelText("My Capture label.")
            .CameraPermissionDeniedErrorMessage(
                "You can not use this component until you activate the camera permission!")
            .CameraPermissionRequestIfDeniedMessage("Please authorize camera usage in settings.")
            .ConfigurationFailedErrorMessage("Camera setup failed")
            .BackLabelText("go back")
            .DialogErrorTitle("Error Title")
            .AgreeButtonTitle("OK")
            .CancelButtonTitle("Cancel");

        
    }

    private void SearchServiceOnOfferAvailable(object? sender, OfferResponseEventArgs e)
    {
        throw new NotImplementedException();
    }
    
    partial void RestoreSearcherSession(Foundation.NSObject sender)
    {
        searchService.LoadLastState(true);
        searchService
            .CategoryPrediction()
            .Start(Show);
    }
    
    partial void OpenNewSearcherSession(Foundation.NSObject sender)
    {
        searchService
            .CategoryPrediction()
            .Start(Show);
    }
    
    public override void ViewDidAppear(bool animated)
    {
        base.ViewDidAppear(animated);
        OfferNumberLabel.Hidden = false;
    }
    
    private void Show(NyrisSearcherResult result)
    {
        OfferNumberLabel.Text = $"There is {result.Offers.Count.ToString()} offers available";
    }

}