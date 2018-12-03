using System;
using System.Threading.Tasks;
using  Nyris.Sdk;

namespace nyris.console
{
    static class MainClass
    {
        public static async Task Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            var nyris = Nyris.Sdk.Nyris.CreateInstance("", true);
            var image = new byte[]{0,0,0,0,0};
            
            await nyris.ImageMatchingAPi
                .Similarity(opt => { opt.Enabled = false; })
                .Ocr(opt => { opt.Enabled = false; })
                .Match(image);
        }
    }
}
