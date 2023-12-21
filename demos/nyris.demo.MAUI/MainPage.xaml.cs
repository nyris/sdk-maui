using Nyris.UI.Maui;

namespace Nyris.Demo.MAUI;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
    }
    
    private void OnSearcherStart(object sender, EventArgs e)
    {
        NyrisSearcher
            .Builder("", true)
            .Theme(themeConfig =>
            {
                themeConfig.CaptureLabelColor = Colors.Red;
                // these images are bundled with iOS version of the SDK
                // replace with your own EmbeddedResource images
                themeConfig.CropButtonImagePath = "validate_icon.png";
                themeConfig.CaptureButtonImagePath = "capture_icon.png";
            })
            .Start(result =>
            {
                if (result == null)
                {
                    ResultLabel.Text =
                        "the searcher is canceled or an exception is raised which forces the result to be null";
                }
                else
                {
                    ResultLabel.Text = 
                        $"Nyris searcher found ({result.Offers.Count}) offers, with request id: {result.RequestCode})";
                }
            });
    }
}