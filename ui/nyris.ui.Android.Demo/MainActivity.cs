using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using Java.Interop;
using Nyris.UI.Android;
using Nyris.UI.Android.Models;

namespace Nyris.UI.Android.Demo
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true, Exported = true)]
    public class MainActivity : AppCompatActivity
    {
        TextView _tvResult;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);
            _tvResult = FindViewById<TextView>(Resource.Id.tvResult);
        }

        [Export("onBtnClick")]
        public void OnValidateClick(View v)
        {
            NyrisSearcher
                .Builder("Your API Key Here", this)
                .CaptureLabelText("My Capture label.")
                .CameraPermissionDeniedErrorMessage("You can not use this componenet until you activate the camera permission!")
                .ExternalStoragePermissionDeniedErrorMessage("You can not use this componenet until you activate the access to external storage permission!")
                .DialogErrorTitle("Error Title")
                .PositiveButtonText("My OK")
                .CategoryPrediction()
                //.ResultAsJson()
                //.Start(loadLastState: true);
                .Start();
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            if (resultCode == Result.Ok)
            {
                if (requestCode == NyrisSearcher.REQUEST_CODE)
                {
                    try
                    {
                        var offerResponse = data.GetParcelableExtra(NyrisSearcher.SEARCH_RESULT_KEY) as OfferResponse;
                        _tvResult.Text = $"Image Path = {offerResponse.TakenImagePath} \n" +
                            $"Found ({offerResponse.Offers.Count}) offers, Categories : ({offerResponse.PredictedCategories.Count})";
                    }
                    catch
                    {
                        var offerResponse = data.GetParcelableExtra(NyrisSearcher.SEARCH_RESULT_KEY) as JsonResponse;
                        _tvResult.Text = $"Image Path = {offerResponse.TakenImagePath} \n" +
                            $"Response : {offerResponse.Content}";
                    }
                }
            }
            else
            {
                //do something else
            }
        }
    }
}
