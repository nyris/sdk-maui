using System;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Io.Nyris.Sdk;
using Io.Nyris.Sdk.Utils;

namespace nyris.ui.Droid
{
    [Activity(Label = "nyris.ui", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true,
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            LoadApplication(new App());

            var nyris = Nyris.CreateInstance("", Platform.Android, true);

            nyris.ImageMatchingAPi
                .Similarity(opt => { opt.Enabled = false; })
                .Ocr(opt => { opt.Enabled = false; })
                .Match(new byte[] {0, 0, 0, 0, 0})
                .Subscribe(x =>
                    {
                        Console.WriteLine(x);
                    },
                    throwable =>
                    {
                        Console.WriteLine(throwable.Message);
                    });
        }
    }
}