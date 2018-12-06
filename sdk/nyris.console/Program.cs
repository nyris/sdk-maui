using System;
using System.IO;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using  Nyris.Sdk;

namespace nyris.console
{
    static class MainClass
    {
        public static void Main(string[] args)
        {
            var nyris = Nyris.Sdk.Nyris.CreateInstance("", true);
            
            var image = File.ReadAllBytes (@"");

            nyris.ImageMatchingAPi
                .Similarity(opt => { opt.Enabled = false; })
                .Ocr(opt => { opt.Enabled = false; })
                .Match(image)
                .Subscribe(x =>
                {
                    Console.WriteLine(x);
                }, throwable =>
                {
                    Console.WriteLine(throwable.Message);
                });

            Console.ReadKey();
        }
    }
}
