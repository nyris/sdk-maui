using System;
using System.IO;
using System.Threading.Tasks;
using  Nyris.Sdk;

namespace nyris.console
{
    static class MainClass
    {
        public static async Task Main(string[] args)
        {
            var nyris = Nyris.Sdk.Nyris.CreateInstance("", true);
            
            var image = File.ReadAllBytes (@"");
            
            await nyris.ImageMatchingAPi
                .Similarity(opt => { opt.Enabled = false; })
                .Ocr(opt => { opt.Enabled = false; })
                .Match(image);
        }
    }
}
