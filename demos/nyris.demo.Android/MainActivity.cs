using Android.Views;
using AndroidX.AppCompat.App;
using Java.Interop;
using Nyris.UI.Android;

namespace Nyris.Demo.Android;

[Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true, Exported = true)]
public class MainActivity : AppCompatActivity
{
    TextView _tvResult;
    protected override void OnCreate(Bundle savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
        Xamarin.Essentials.Platform.Init(this, savedInstanceState);
        SetContentView(Resource.Layout.activity_main);
        _tvResult = FindViewById<TextView>(Resource.Id.tvResult);
    }
    

    [Export("onBtnClick")]
    public void OnValidateClick(View v)
    {
        NyrisSearcher
            .Builder("", this, true)
            .CaptureLabelText("My Capture label.")
            .CameraPermissionDeniedErrorMessage("You can not use this component until you activate the camera permission!")
            .ExternalStoragePermissionDeniedErrorMessage("You can not use this component until you activate the access to external storage permission!")
            .DialogErrorTitle("Error Title")
            .AgreeButtonTitle("My OK")
            //.LoadLastState(true)
            .CategoryPrediction()
            .Start(result =>
            {
                if (result == null)
                {
                    _tvResult.Text =
                        "the searcher is canceled or an exception is raised which forces the result to be null";
                }
                else
                {
                    _tvResult.Text = $"Found ({result.Offers.Count}) offers, " +
                                     $"Predicted Categories ({result.PredictedCategories?.Count}), " +
                                     $"with request id: {result.RequestCode})";
                }
            });
    }
}