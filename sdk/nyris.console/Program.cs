using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reactive.Disposables;
using System.Threading.Tasks;
using Nyris.Sdk;
using Nyris.Sdk.Network.Model;
using Nyris.Sdk.Utils;
using Refit;

namespace nyris.console
{
    static class MainClass
    {
        public static async Task Main(string[] args)
        {
            var solutionPath = Directory.GetParent(Directory.GetCurrentDirectory())
                                   .Parent?.Parent?.Parent?.FullName ?? "";
            var imagePath = Path.Combine(solutionPath, "sample.jpg");
            var image = File.ReadAllBytes(imagePath);
            var apiKey = Environment.GetEnvironmentVariable("API_KEY") ?? "";

            var compositeDisposable = new CompositeDisposable();
            var nyris = NyrisApi.CreateInstance(apiKey, Platform.Generic, true);
            /*nyris.ImageMatchingAPi
                .CategoryPrediction(opt =>
                {
                    opt.Enabled = true;
                    opt.Limit = 5;
                })
                .Limit(5)
                .Match(image)
                .Subscribe(x => Debug.WriteLine(x),
                    throwable => Debug.WriteLine(throwable.Message)
                );*/


            /*var extractedObjects = await nyris.ObjectProposalApi.ExtractObjectsAsync<string>(image);
            Debug.WriteLine(extractedObjects);
            
            nyris.OfferTextSearchApi
                .Limit(5)
                .SearchOffers("Keyboard")
                .Subscribe(x => Debug.WriteLine(x),
                    throwable => Debug.WriteLine(throwable.Message)
                );
            compositeDisposable.Add(nyris.RecommendationApi
                .GetOffersBySku<string>("sku-test").Subscribe(x => { Debug.WriteLine(x); },
                    throwable => { Debug.WriteLine(throwable.Message); }));*/

            /*nyris.MarkForManualSearchApi.MarkOfferAsNotFound("d3d8004940564aa78c3f04a7d0536f34")
                .Subscribe(x =>
                    {
                        Debug.WriteLine(x);
                    },
                    throwable => { Debug.WriteLine(throwable.Message); });*/

            /**/

            nyris.ImageMatchingAPi
                .CategoryPrediction(opt =>
                {
                    opt.Enabled = true;
                    opt.Limit = 5;
                })
                .Limit(5)
                .Match(image)
                .Subscribe(x =>
                    {
                        nyris.MarkForManualSearchApi
                            .MarkOfferAsNotFound(x.RequestCode)
                            .Subscribe(response => { Debug.WriteLine(response); },
                                thrown => { Debug.WriteLine(thrown.Message); });
                    },
                    throwable => Debug.WriteLine(throwable.Message)
                );
            
            Console.ReadKey();
            compositeDisposable.Dispose();
        }
    }
}