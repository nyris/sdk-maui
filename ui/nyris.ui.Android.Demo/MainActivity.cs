using Android.App;
using Android.OS;
using Nyris.Ui.Android.Demo.Resources;

namespace Nyris.Ui.Android.Demo
{
    [Activity(Label = "demo", MainLauncher = true)]
    public class MainActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            NyrisSearcher
                .Instance(true)
                .CategoryPrediction(opt =>
                {
                    opt.Enabled = true;
                    opt.Limit = 100;
                })
                .Show(this);
        }
    }
}

