using System;
using Io.Nyris.Sdk;
using Io.Nyris.Sdk.Utils;
using UIKit;

namespace nyris.ui.iOS
{
    public class Application
    {
        // This is the main entry point of the application.
        static void Main(string[] args)
        {
            // if you want to use a different Application Delegate class from "AppDelegate"
            // you can specify it here.
            
            var nyris = Nyris.CreateInstance("", Platform.iOS, true);

            nyris.ImageMatchingAPi
                .Similarity(opt => { opt.Enabled = false; })
                .Ocr(opt => { opt.Enabled = false; })
                .Match(new byte[]{0,0,0,0,0})
                .Subscribe(x =>
                {
                    Console.WriteLine(x);
                }, throwable =>
                {
                    Console.WriteLine(throwable.Message);
                });
            UIApplication.Main(args, null, "AppDelegate");
        }
    }
}
