using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Java.Interop;
using Nyris.Ui.Android.Models;

namespace Nyris.Ui.Android.Demo
{
    [Activity(Label = "demo", MainLauncher = true)]
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
                .Builder("Your API Key Here", this, true)
                .AsJson()
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
                        _tvResult.Text = $"Found ({offerResponse.Offers.Count}) offers";
                    }
                    catch
                    {
                        var offerResponse = data.GetParcelableExtra(NyrisSearcher.SEARCH_RESULT_KEY) as JsonResponse;
                        _tvResult.Text = $"Response : {offerResponse.Content}";
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

