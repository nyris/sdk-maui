using Microsoft.Maui.Platform;
#if __ANDROID__
        
#elif __IOS__
using Nyris.UI.iOS;
using UIKit;
#endif

namespace Nyris.UI.Maui;

public class ThemeConfig
{
    // tint for buttons
    public Color PrimaryTintColor;
    public Color PrimaryDarkTintColor;
    // system elements (back button, status etc)
    public Color AccentTintColor;

    public ThemeConfig()
    {
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
            CaptureLabelColor = PrimaryTintColor?.ToPlatform(),
            CaptureButtonTint = PrimaryTintColor?.ToPlatform(),
            CropButtonTint = PrimaryTintColor?.ToPlatform(),
            BackButtonTint = AccentTintColor?.ToPlatform(),
        };

        return config;
    }
#endif
}