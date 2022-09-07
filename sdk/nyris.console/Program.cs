using System;
using System.Diagnostics;
using System.IO;
using System.Reactive.Disposables;
using System.Threading.Tasks;
using Nyris.Api;
using Nyris.Api.Model;

namespace nyris.console
{
    static class Program
    {
        public static async Task Main(string[] args)
        {
            var solutionPath = Directory.GetParent(Directory.GetCurrentDirectory())
                                   .Parent?.Parent?.Parent?.FullName ?? "";
            var imagePath = Path.Combine(solutionPath, "sample.jpg");
            var image = File.ReadAllBytes(imagePath);
            var apiKey = Environment.GetEnvironmentVariable("API_KEY") ?? "";
            var nyris = NyrisApi.CreateInstance(apiKey, Platform.Generic, isDebug: true);

            Console.WriteLine("Select Api mode: Reactive(1) or Async(2)");
            string key;
            do
            {
                key = Console.ReadLine();
            } while (key != "1" && key != "2");

            if (key == "1")
            {
                RunReactiveSamples(nyris, image);
            }
            else
            {
                await RunAsyncSamples(nyris, image);
            }
        }

        private static void RunReactiveSamples(INyrisApi nyris, byte[] image)
        {
            var compositeDisposable = new CompositeDisposable();

            #region Image Matching

            compositeDisposable.Add(
                nyris.ImageMatching
                    .CategoryPrediction(opt =>
                    {
                        opt.Enabled = true;
                        opt.Limit = 5;
                    })
                    .Limit(5)
                    .Match(image)
                    .Subscribe(response =>
                        {
                            Console.WriteLine("#### Image Matching");
                            Console.WriteLine(response);
                        },
                        throwable => Console.WriteLine(throwable.Message)
                    ));

            #endregion

            #region Image Matching Json

            compositeDisposable.Add(
                nyris.ImageMatching
                    .CategoryPrediction(opt =>
                    {
                        opt.Enabled = true;
                        opt.Limit = 5;
                    })
                    .Limit(5)
                    .Match<JsonResponseDto>(image)
                    .Subscribe(response =>
                        {
                            Console.WriteLine("#### Image Matching Json");
                            Console.WriteLine(response);
                        },
                        throwable => Console.WriteLine(throwable.Message)
                    ));

            #endregion

            #region Text Search

            nyris.TextSearch
                .Limit(5)
                .Search("Keyboard")
                .Subscribe(response =>
                    {
                        Console.WriteLine("#### Text Search");
                        Console.WriteLine(response);
                    },
                    throwable => Console.WriteLine(throwable.Message)
                );

            #endregion

            #region Object Detections

            compositeDisposable.Add(
                nyris.ObjectProposal
                    .ExtractObjects(image)
                    .Subscribe(response =>
                        {
                            Console.WriteLine("#### Extract Objects");
                            Console.WriteLine(response);
                        },
                        throwable => Console.WriteLine(throwable.Message)));

            #endregion

            #region Mark request as not found

            compositeDisposable.Add(
                nyris.ImageMatching
                    .Match(image)
                    .Subscribe(response =>
                        {
                            compositeDisposable.Add(nyris.Feedback
                                .MarkAsNotFound(response.RequestCode)
                                .Subscribe(response2 =>
                                    {
                                        Console.WriteLine("#### Mark request as not found");
                                        Console.WriteLine(response2);
                                    },
                                    thrown => { Console.WriteLine(thrown.Message); }));
                        },
                        throwable => Debug.WriteLine(throwable.Message)
                    ));

            #endregion

            #region Recommendation by SKU

            /*compositeDisposable.Add(nyris.RecommendationApi
                .GetOffersBySku("SKU")
                .Subscribe(response =>
                    {
                        Console.WriteLine("#### Recommendation by SKU");
                        Console.WriteLine(response);
                    },
                    throwable => Console.WriteLine(throwable.Message)
                ));*/

            #endregion

            Console.Read();
            compositeDisposable.Dispose();
        }

        private static async Task RunAsyncSamples(INyrisApi nyris, byte[] image)
        {
            #region Image Matching

            var response = await nyris.ImageMatching
                .CategoryPrediction(opt =>
                {
                    opt.Enabled = true;
                    opt.Limit = 5;
                })
                .Limit(5)
                .MatchAsync(image);

            Console.WriteLine("#### Image Matching");
            Console.WriteLine(response);

            #endregion

            #region Image Matching Json

            var response2 = await nyris.ImageMatching
                .CategoryPrediction(opt =>
                {
                    opt.Enabled = true;
                    opt.Limit = 5;
                })
                .Limit(5)
                .MatchAsync<JsonResponseDto>(image);

            Console.WriteLine("#### Image Matching Json");
            Console.WriteLine(response2);

            #endregion

            #region Object Detections

            var response3 = await nyris.ObjectProposal
                .ExtractObjectsAsync(image);

            Console.WriteLine("#### Object Detections");
            Console.WriteLine(response3);

            #endregion

            #region Mark request as not found

            var response4 = await nyris.Feedback
                .MarkAsNotFoundAsync(response.RequestCode);

            Console.WriteLine("#### Mark request as not found");
            Console.WriteLine(response4);

            #endregion

            #region Recommendation by SKU

            /*var response5 = await nyris.RecommendationApi
                .GetOffersBySkuAsync("SKU");

            Console.WriteLine("#### Recommendation by SKU");
            Console.WriteLine(response5);*/

            #endregion

            Console.Read();
        }
    }
}
