using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Newtonsoft.Json;

namespace Nyris.Ui.Android
{
    [Activity(Label = "NyrisSearcherActivity")]
    class NyrisSearcherActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            var extraJson = Intent.GetStringExtra(NyrisSearcher.CONFIG_KEY);
            var nyrisSearcherConfig = JsonConvert.DeserializeObject<NyrisSearcherConfig>(extraJson);
        }
    }
}
