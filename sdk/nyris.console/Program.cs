using System;
using System.Diagnostics;
using System.IO;
using Nyris.Sdk;
using Nyris.Sdk.Utils;

namespace nyris.console
{
    static class MainClass
    {
        public static void Main(string[] args)
        {
            var solutionPath = Directory.GetParent(Directory.GetCurrentDirectory())
                                   .Parent?.Parent?.Parent?.FullName ?? "";
            var imagePath = Path.Combine(solutionPath, "sample.jpg");
            var image = File.ReadAllBytes(imagePath);
            var apiKey = Environment.GetEnvironmentVariable("API_KEY") ?? "";

            var nyris = NyrisApi.CreateInstance(apiKey, Platform.Other, true);
            nyris.ImageMatchingAPi
                .CategoryPrediction(opt =>
                {
                    opt.Enabled = true;
                    opt.Limit = 5;
                })
                .Limit(5)
                .Match(image)
                .Subscribe(x => Debug.WriteLine(x),
                    throwable => Debug.WriteLine(throwable.Message)
                );

            nyris.ObjectProposalApi
                .ExtractObjects(image)
                .Subscribe(x => Debug.WriteLine(x),
                    throwable => Debug.WriteLine(throwable.Message));
            Console.ReadKey();
        }
    }
}