using Android.Content;
using Android.Graphics;
using Newtonsoft.Json;

namespace Nyris.UI.Android;

public class AndroidThemeConfig
{
    public Color? PrimaryColor;
    public Color? PrimaryDarkColor;
    public Color? AccentColor;
}

public static class AndroidThemeConfigExt
{
    public static String ToJson(this AndroidThemeConfig? theme)
    {
        var colorInts = new 
            { 
                PrimaryColor = theme?.PrimaryColor?.ToArgb(), 
                PrimaryDarkColor = theme?.PrimaryDarkColor?.ToArgb(), 
                AccentColor = theme?.AccentColor?.ToArgb() 
            };
        return JsonConvert.SerializeObject(colorInts);
    }
    
    public static AndroidThemeConfig? ToTheme(int? primaryColor, int? primaryDarkColor, int? accentColor)
    {
        if (primaryColor == null && primaryDarkColor == null && accentColor == null) return null;
        return new AndroidThemeConfig
        {
            PrimaryColor = primaryColor?.ToColor(),
            PrimaryDarkColor = primaryDarkColor?.ToColor(),
            AccentColor = accentColor?.ToColor()
        };
    }

    private static Color ToColor(this int color)
    {
        var alpha = (color >> 24) & 0xff;
        var red = (color >> 16) & 0xff;
        var green = (color >> 8) & 0xff;
        var blue = color & 0xff;
        
        return Color.Argb(alpha, red, green, blue);
    }
}