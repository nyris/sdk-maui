using Microsoft.Maui.Platform;
#if __ANDROID__
        
#elif __IOS__
using Nyris.UI.iOS;
using UIKit;
#endif

namespace Nyris.UI.Maui;

public class ThemeConfig
{
    public string CaptureButtonImagePath;
    public string CropButtonImagePath;
    public string BackButtonImagePath;
    public string FlashLightOnButtonImagePath;
    public string FlashLightOffButtonImagePath;
    public Color CaptureButtonTint;
    public Color CropButtonTint;
    public Color BackButtonTint;
    public Color CaptureLabelColor;

    public ThemeConfig()
    {
#if __ANDROID__
        
#elif __IOS__
        var brandingColor = Color.FromRgb(227,26,95);
        CaptureButtonImagePath = "capture_icon.png";
        CropButtonImagePath = "validate_icon.png";
        BackButtonImagePath = "close_icon.png";
        FlashLightOnButtonImagePath = "torch_on_icon.png";
        FlashLightOffButtonImagePath = "torch_off_icon.png";
        CaptureButtonTint =  brandingColor;
        CropButtonTint = brandingColor;
        BackButtonTint = brandingColor;
        CaptureLabelColor = brandingColor;
#endif
    }
#if __ANDROID__
    public void ToPlatform()
    {
        var themeConfig = new ThemeConfig();
    }
#elif __IOS__
    public AppearanceConfiguration ToPlatform()
    {
        var config = new AppearanceConfiguration
        {
            CaptureLabelColor = CaptureLabelColor?.ToPlatform(),
            CaptureButtonTint = CaptureButtonTint?.ToPlatform(),
            CropButtonTint = CropButtonTint?.ToPlatform(),
            BackButtonTint = BackButtonTint?.ToPlatform(),
            CaptureButtonImage = UIImage.FromBundle(CaptureButtonImagePath),
            CropButtonImage = UIImage.FromBundle(CropButtonImagePath),
            FlashLightOffButtonImage = UIImage.FromBundle(FlashLightOffButtonImagePath),
            FlashLightOnButtonImage = UIImage.FromBundle(FlashLightOnButtonImagePath),
            BackButtonImage = UIImage.FromBundle(BackButtonImagePath)
        };

        return config;
    }
#endif
}