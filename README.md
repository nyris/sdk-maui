# nyris SDK for .NET, Xamarin and MAUI

![nyris.io logo](https://storage.googleapis.com/nyris-logos/title.png)

## Introduction

[nyris](https://nyris.io/) is a high performance visual product search, object detection and visual recommendations engine
for retail and industry applications. For more information please visit us at [nyris.io](https://nyris.io/).

This repo provides a [C#](https://docs.microsoft.com/en-us/previous-versions/visualstudio/visual-studio-2012/kx37x362(v%3dvs.110)) SDK
to use our system, including error handling, as well as reactive and asynchronous programming support. Specifically, we provide:

* Support for all nyris services,
* Support of Reactive/Asynchronous programming paradigms,
* Simplified error handling,
* Type-safe HTTP client, and a Unified response format.

To the following nyris services:

* Exact visual search,
* Visual similarity search,
* Object detection,
* Text search.

### Requirements

Please see the documentation at [docs.nyris.io](https://docs.nyris.io) for a complete reference.
At the minimum, the following criteria must be met:

* Images are sent in **JPEG** format,
* The minimum dimensions of an image are `512x512 px`, and
* The size of an image is less than or equal to `500 KB`.
* In Xamarin.Android or MAUI you need to make sure to handle the permissions and to include these permissions in `AndroidManifest.xml`
```xml
<?xml version="1.0" encoding="utf-8"?>
<manifest xmlns:android="http://schemas.android.com/apk/res/android">
    ...
  <uses-permission android:name="android.permission.READ_EXTERNAL_STORAGE" />
  <uses-permission android:name="android.permission.WRITE_EXTERNAL_STORAGE" />
  <uses-permission android:name="android.permission.CAMERA" />
</manifest>
```


# Get Started
* [Get Started with nyris SDK](#get-started-with-nyris-sdk)
* [Get Started with nyris Xamarin Searcher](#get-started-with-nyris-xamarin-searcher)
* [Get Started with nyris MAUI Searcher](#get-started-with-nyris-maui-searcher)

## Get Started with nyris SDK

### Jump To
* [Getting an instance](#getting-an-instance)
* [Match your first image](#match-your-first-image)
* [Extract objects from your image](#extract-objects-from-your-image)
* [Mark sent image as not found](#mark-sent-image-as-not-found)
* [Text Match Search](#text-match-search)

### Getting an instance

First, initialize an instance of `NyrisApi` with your API Key :

```csharp
static class Program
{
    public static async Task Main(string[] args)
    {
        var apiKey = "Your Api Key";     // specify your nyris API key
        var platform = Platform.Generic; // select your platform
        var debugMode = true;            // enable or disable debugging

        var nyris = NyrisApi.CreateInstance(apiKey, platform, debugMode);
    }
}
```

### Match your first image
#### Simple matching using RX

```csharp
    byte[] imageByteArray = /*Your byte array*/;

    //Asynchronous
    var response = await nyris.ImageMatching.MatchAsync(image);

    //Reactive
    nyris.ImageMatching
        .Match(image)
        .Subscribe(response =>
            {
                Console.WriteLine(response);
            },
            throwable => Console.WriteLine(throwable.Message)
        );
```


#### Fully-configured image matching

```csharp
    //For more details about available feed attributes please check our documentation : http://docs.nyris.io/#available-feed-attributes.
    var imageByteArray = ...; /* Your byte array */
    nyris.ImageMatching
        .Language("de") // Return only offers with language "de".
        .Limt(10) // Limit returned offers
        .CategoryPrediction(opt =>
        {
            opt.Enabled = true
            opt.Threshold = 0.6f
            opt.Limit = 10 //Size of the returned catgories
        }) // Get predicted categories
        .Filters(opt => // Add Filters 
        {
            opt.AddFilter("color", new List<string> { "red", "blue" });
        })
        .Match(imageTestBytes)
        .Subscribe(response =>
            {
                //Handle your response
                var offers = response.Offers;
            },
            throwable => ...
        );
```

The response will be an object of type `OfferResponseDto` that contains list of `Offers`, `RequestId`

### Extract objects from your image

Extract objects from an image:

```csharp
    nyris.ObjectProposal
        .ExtractObjects(image)
        .Subscribe(response /* RegionsObjectDto */=>
        {
            // Handle your response
            var objects = response;
        }, throwable => ...);
```
The returned response is a List of detected objects.
Each object has:

* `Confidence`; this is the probability of the top item with values ranging between: `0-1`.
* `Region` is a bounding box. It represents the location and the size of the object in the sent image as a fraction of the image size.

### Marking a response as not successful

It may happen that our service can't recognize or match an image. This is why we provide you a service to notify us
about any unsuccessful matches.

Before you mark an image as **"not successful"**, you will need to have the `RequestId`. For more details please check this [section](#match-your-first-image).

After getting the `RequestId` you can mark the image as not found.

```csharp
    nyris.Feedback
        .MarkAsNotFound(RequestId)
        .Subscribe(response /* HttpResponseMessage */=>
        {
            // Handle your response...
        }, throwable => ...);
```

### Text Match Search

nyris offers a service that helps you to search offers by text, SKU, barcode, etc.
You can use the text search service the same way as the [image matching service](#match-your-first-image):

```csharp
    nyris.TextSearch
        .Search("Your text")
        .Subscribe(response =>
            {
                Console.WriteLine(response);
            },
            throwable => Console.WriteLine(throwable.Message)
        );
```

## Get Started with nyris Xamarin Searcher

To start using image matching component, you will need to reference the project `nyris.ui.Android` for Android and the project `nyris.ui.iOS`for iOS and include all the required dependencies with it.

### Jump To
* [Starting the component](#starting-the-component)
* [Setting image matching options](#starting-the-component)
* [Handling returned results](#handling-returned-results)
* [Handling JSON results](#handling-json-results)
* [Customizing the text of the component](#customizing-the-text-of-the-component)
* [Customizing the color of the component](#customizing-the-color-of-the-component)
* [Load last session state](#load-last-session-state)

### Starting the component
To start the component call:

```csharp
    NyrisSearcher
        .Builder("Your API Key Here", ActivityOrPresenterController)
        .Start(...);
```


### Setting image matching options

For more details about image matching options please check [this section](#match-your-first-image).

```csharp
    NyrisSearcher
        .Builder("Your API Key Here", ActivityOrPresenterController)
        .Filters(opt => // Add Filters 
        {
            opt.AddFilter("color", new List<string> { "red", "blue" });
        })
        .Start(...);
```

### Handling returned results

#### For Android
```csharp
    NyrisSearcher
        .Builder("Your API Key Here", ActivityOrPresenterController)
        .CategoryPrediction(opt => 
        {
            opt.Enabled = true
            opt.Threshold = 0.6f
            opt.Limit = 10 //Size of the returned catgories
        }) // Get predicted categories
        .Filters(opt => // Add Filters 
        {
            opt.AddFilter("color", new List<string> { "red", "blue" });
        })
        .Start(result => // Handling your result
        {
            if (result == null)
            {
                _tvResult.Text =
                    "the searcher is canceled or an exception is raised which forces the result to be null";
            }
            else
            {
                _tvResult.Text = $"Image Path = offerResponse.TakenImagePath \n" +
                                 $"Found ({result.Offers.Count}) offers, with request id: {result.RequestCode})";
            }
        });
```

#### For iOS
The iOS version uses events to notify you about search result, first subscribe to `OfferAvailable`

```csharp
	searchService = NyrisSearcher,Builder("Your API Key Here", presenterController);

	searchService.OfferAvailable += SearchServiceOnOfferAvailable;
```
Whenever an search is performed `SearchServiceOnOfferAvailable` will be called:

```csharp
void SearchServiceOnOfferAvailable(object sender, OfferResponseEventArgs e)
{
    var offers = e.OfferResponse;
    var json = e.OfferJson;
}
```
Only one response type is available in `OfferResponseEventArgs` at a time. You can access the  screenshot and the cropping frame by using :
```csharp
void SearchServiceOnOfferAvailable(object sender, OfferResponseEventArgs e)
{
    var screenshot = e.Screenshot;
    var croppingFrame = e.Screenshot;
}
```

### Customizing the text of the component

You can change text of the component by using:

```csharp
    // You need to handle Camera and storage permissions before opening the nyris searcher
    NyrisSearcher
        .Builder("Your API Key Here", ActivityOrPresenterController)
        .AgreeButtonTitle("My OK")
        .CancelButtonTitle("My Cancel")
        .CameraPermissionDeniedErrorMessage("You can not use this componenet until you activate the camera permission!")
        .ExternalStoragePermissionDeniedErrorMessage("You can not use this componenet until you activate the access to external storage permission!")
        .CameraPermissionRequestIfDeniedMessage("Your message when camera permission is denied") // For iOS only
        .ConfigurationFailedErrorMessage("Your message when configuration is failed") // For iOS only
        .CaptureLabelText("My Capture label.")
        .DialogErrorTitle("Error Title")
        .BackLabelText("Your back label") // For iOS only
        .CategoryPrediction(opt => 
        {
            opt.Enabled = true
            opt.Threshold = 0.6f
            opt.Limit = 10 //Size of the returned catgories
        }) // Get predicted categories
        .Start();
```
See `NyrisSearcherConfig.cs` for full list of configuration.

### Customizing the color of the component

#### For Android
To customize the color of the component you will need to override the defined colors:

```xml
<?xml version="1.0" encoding="utf-8"?>
<resources>
    <!-- You can override nyris searcher color here -->
    <color name="nyris_color_primary">YOUR_COLOR</color>
    <color name="nyris_color_primary_dark">YOUR_COLOR</color>
    <color name="nyris_color_accent">YOUR_COLOR</color>
</resources>
```

#### For iOS
To customize the color or image of different views of the Searcher controller, create an instance of `AppearanceConfiguration` and assign it to the builder as follow:
```csharp
    var theme = new AppearanceConfiguration
    {
        CaptureLabelColor = UIColor.Red,
        CropButtonTint = UIColor.Yellow,
        BackButtonTint = UIColor.Red,
        CaptureButtonTint = UIColor.Green,
        CaptureButtonImage = UIImage,
        CropButtonImage = UIImage,
        FlashLightOffButtonImage = UIImage,
        FlashLightOnButtonImage = UIImage,
    };

    searchService = NyrisSearcher
        .Builder("Your API Key Here", this)
        .Theme(theme)
```

### Load last session state

To load last session state of the `NyrisSearcher` you will need :
```csharp
    NyrisSearcher
        .Builder("Your API Key Here", ActivityOrPresenterController)
        .LoadLastState(true)
        .Start(...);
```
In case of an unknown error or un saved state the parameter will fallback to default mode.

## Get Started with nyris MAUI Searcher

Not different from Xamarin.Android SDK or Xamarin.iOS SDK, you can start using the nyris MAUI searcher by following this:

```csharp
    NyrisSearcher
        .Builder("Your API Key Here", ActivityOrPresenterController)
        .AgreeButtonTitle("My OK")
        .CancelButtonTitle("My Cancel")
        .CameraPermissionDeniedErrorMessage("You can not use this componenet until you activate the camera permission!")
        .ExternalStoragePermissionDeniedErrorMessage("You can not use this componenet until you activate the access to external storage permission!")
        .CameraPermissionRequestIfDeniedMessage("Your message when camera permission is denied") // For iOS only
        .ConfigurationFailedErrorMessage("Your message when configuration is failed") // For iOS only
        .CaptureLabelText("My Capture label.")
        .DialogErrorTitle("Error Title")
        .BackLabelText("Your back label") // For iOS only
        .Language("de")
        .Limit(10)
        .CategoryPrediction(opt =>
        {
            opt.Enabled = true
            opt.Threshold = 0.6f
            opt.Limit = 10 //Size of the returned catgories
        }) // Get predicted categories
        .Start(result => // Handling your result
        {
            if (result == null)
            {
                // the searcher is canceled or an exception is raised which forces the result to be null
            }
            else
            {
                // Your result
            }
        };
```

License
=======

See the [LICENSE](LICENSE) file for the software license.
