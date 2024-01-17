using JetBrains.Annotations;
using Microsoft.Maui.Platform;

#if __ANDROID__
using Nyris.UI.Android;
#elif __IOS__
using Nyris.UI.iOS;
using UIKit;
#endif

namespace Nyris.UI.Maui;

public class ThemeConfig
{
    // tint for buttons
    [CanBeNull] public Color PrimaryTintColor;
    [CanBeNull] public Color PrimaryDarkTintColor;
    // system elements (back button, status etc)
    [CanBeNull] public Color AccentTintColor;

    public ThemeConfig()
    {
    }
#if __ANDROID__
    [CanBeNull]
    public AndroidThemeConfig ToPlatform()
    {
        if (AccentTintColor == null && PrimaryTintColor == null && PrimaryDarkTintColor == null) return null;
        return new AndroidThemeConfig
        {
            AccentColor = AccentTintColor?.ToPlatform(),
            PrimaryColor = PrimaryTintColor?.ToPlatform(),
            PrimaryDarkColor = PrimaryDarkTintColor?.ToPlatform()
        };
    }
        
#elif __IOS__
    [CanBeNull]
    public AppearanceConfiguration ToPlatform()
    {
        if (AccentTintColor == null && PrimaryTintColor == null && PrimaryDarkTintColor == null) return null;
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