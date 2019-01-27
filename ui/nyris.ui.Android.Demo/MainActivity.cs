using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Java.Interop;
using Nyris.UI.Android.Models;

namespace Nyris.UI.Android.Demo
{
    [Activity(Label = "demo", MainLauncher = true, ScreenOrientation = ScreenOrientation.Portrait)]
    public class MainActivity : Activity
    {
        TextView _tvResult;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Main);
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
                .ShouldShowPermissionMessage("Should show message after second permission request")
                .DialogErrorTitle("Error Title")
                .PositiveButtonText("My OK")
                .CategoryPrediction()
                //.ResultAsJson()
                .Start(loadLastState: true);
            //.Start();
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

