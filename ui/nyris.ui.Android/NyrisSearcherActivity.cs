using Android.App;
using Android.OS;
using IO.Nyris.Camera;
using Newtonsoft.Json;
using Nyris.Ui.Android.Custom;

namespace Nyris.Ui.Android
{
    [Activity(Label = "NyrisSearcherActivity", Theme = "@style/NyrisSearcherTheme")]
    class NyrisSearcherActivity : Activity
    {
        CameraView _cameraView;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.SearcherLayout);
            _cameraView = FindViewById<CameraView>(Resource.Id.camera);
            var circleView = FindViewById<CircleView>(Resource.Id.cvTakePic);
            circleView.StartAnimation(FindViewById(Resource.Id.vPosCam));

            var extraJson = Intent.GetStringExtra(NyrisSearcher.CONFIG_KEY);
            var nyrisSearcherConfig = JsonConvert.DeserializeObject<NyrisSearcherConfig>(extraJson);
        }

        protected override void OnResume()
        {
            base.OnResume();
            _cameraView?.Start();
        }

        protected override void OnPause()
        {
            base.OnPause();
            _cameraView?.Stop();
        }
    }
}
