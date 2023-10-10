using System.Diagnostics;
using System.Reactive.Disposables;
using Nyris.Api;
using Nyris.Api.Model;

namespace Nyris.Demo.Console;

static class Program
{
    public static async Task Main(string[] args)
    {
        var solutionPath = Directory.GetParent(Directory.GetCurrentDirectory())
            .Parent?.Parent?.Parent?.Parent?.FullName ?? "";
        var imagePath = Path.Combine(solutionPath, "sample.jpg");
        var image = File.ReadAllBytes(imagePath);
        var apiKey = Environment.GetEnvironmentVariable("API_KEY") ?? "";
        var nyris = NyrisApi.CreateInstance(apiKey, Platform.Generic, isDebug: true);

        System.Console.WriteLine("Select Api mode: Reactive(1) or Async(2)");
        string key;
        do
        {
            key = System.Console.ReadLine();
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
                .Filters(opt =>
                {
                    opt.AddFilter("color", new List<string> { "red", "blue" });
                })
                .Match(image)
                .Subscribe(response =>
                    {
                        System.Console.WriteLine("#### Image Matching");
                        System.Console.WriteLine(response);
                    },
                    throwable => System.Console.WriteLine(throwable.Message)
                ));

        #endregion

        #region Text Search

        nyris.TextSearch
            .Limit(5)
            .Search("Keyboard")
            .Subscribe(response =>
                {
                    System.Console.WriteLine("#### Text Search");
                    System.Console.WriteLine(response);
                },
                throwable => System.Console.WriteLine(throwable.Message)
            );

        #endregion

        #region Object Detections

        compositeDisposable.Add(
            nyris.ObjectProposal
                .ExtractObjects(image)
                .Subscribe(response =>
                    {
                        System.Console.WriteLine("#### Extract Objects");
                        System.Console.WriteLine(response);
                    },
                    throwable => System.Console.WriteLine(throwable.Message)));

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
                                    System.Console.WriteLine("#### Mark request as not found");
                                    System.Console.WriteLine(response2);
                                },
                                thrown => { System.Console.WriteLine(thrown.Message); }));
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

        System.Console.Read();
        compositeDisposable.Dispose();
    }

    private static async Task RunAsyncSamples(INyrisApi nyris, byte[] image)
    {
        #region Image Matching

        var response = await nyris.ImageMatching
            .Limit(5)
            .MatchAsync(image);

        System.Console.WriteLine("#### Image Matching");
        System.Console.WriteLine(response);

        #endregion

        #region Object Detections

        var response3 = await nyris.ObjectProposal
            .ExtractObjectsAsync(image);

        System.Console.WriteLine("#### Object Detections");
        System.Console.WriteLine(response3);

        #endregion

        #region Mark request as not found

        var response4 = await nyris.Feedback
            .MarkAsNotFoundAsync(response.RequestCode);

        System.Console.WriteLine("#### Mark request as not found");
        System.Console.WriteLine(response4);

        #endregion

        #region Recommendation by SKU

        /*var response5 = await nyris.RecommendationApi
            .GetOffersBySkuAsync("SKU");

        Console.WriteLine("#### Recommendation by SKU");
        Console.WriteLine(response5);*/

        #endregion

        System.Console.Read();
    }
}