using System;
using System.Diagnostics;
using System.IO;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using  Nyris.Sdk;
using Nyris.Sdk.Utils;

namespace nyris.console
{
    static class MainClass
    {
        public static void Main(string[] args)
        {
            var nyris = Nyris.Sdk.Nyris.CreateInstance("",Platform.Other, true);
            
            var image = File.ReadAllBytes (@"");

            nyris.ImageMatchingAPi
                .CategoryPrediction(opt =>
                {
                    opt.Enabled = true;
                    opt.Limit = 5;
                })
                .Match(image)
                .Subscribe(x =>
                {
                    Debug.WriteLine(x);
                }, throwable =>
                {
                    Debug.WriteLine(throwable.Message);
                });

            Console.ReadKey();
        }
    }
}
