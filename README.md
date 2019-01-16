# nyris SDK for .NET and Xamarin

![nyris.io logo](https://storage.googleapis.com/nyris-logos/title.png)

## Introduction

[nyris](https://nyris.io/) is a high performance visual product search, object detection and visual recommendations engine 
for retail and industry applications. For more information please visit us at [nyris.io](https://nyris.io/).

This repo provides a [C#](https://docs.microsoft.com/en-us/previous-versions/visualstudio/visual-studio-2012/kx37x362(v%3dvs.110)) SDK 
to use our system, including error handling, as well as reactive and asynchronous programming support. Specifically, we provide

* Support for all nyris services,
* Support of Reactive/Asynchronous programming paradigms,
* Simplified error handling,
* Type-safe HTTP client, and a
* Unified response format.

To the following nyris services:

* Exact visual search
* Visual similarity search
* Object detection
* Text search

### Requirements

Please see the documentation at [docs.nyris.io](https://docs.nyris.io) for a complete reference.
At the minimum, the following criteria must be met:

* Images are sent in **JPEG** format,
* The minimum dimensions of an image are `512x512 px`, and
* The size of an image is less than or equal to `500 KB`.

## SDK Solution

The current solution is composed of multiple projects:

* **nyris API**: The SDK for accessing nyris APIs.
* **nyris Console Demo**: A demo console application that shows different usages of the API.
* **nyris ui Android**: The nyris image matching components.
* **nyris ui Camera Android**: A jar binding project for our Java [Camera view](https://github.com/nyris/Camera.Android).
* **nyris ui Cropping Android**: A jar binding project for the Java image cropping view.
* **nyris ui Demo Android**: An Android demo app that shows how to use nyris search component(s).

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
        .OutputFormat("PROVIDED_OUTPUT_FORMAT") // Set the desired OUTPUT_FORMAT
        .Language("de") // Return only offers with language "de".
        .Exact(opt =>
        {
            Enabled = false; // disable exact matching
        })
        .Similarity(opt => //Performs similarity matching
        {
            Threshold = 0.5f; // The lower limit of confidences to be considered good from similarity
            Limit = 10; // The upper limit for the number of results to be returned from similarity
        })
        .Ocr(opt => //Performs optical character recognition on the images
        {
            Enabled = false; // disable OCR
        })
        .Regroup(opt =>
        {
            Enabled = false; // This mode enables regrouping of the items
            Threshold = 0.5f; // The lower limit of confidences to be considered good from similarity
        })
        .Recommendations() // Enables recommendation type searches that return all discovered results regardless of their score.
        .categoryPrediction({opt =>
        {
            Enabled = true; // Enables the output of predicted categories.
            Threshold = 0.5F; // Sets the cutoff threshold for category predictions (range 0..1).
            Limit = 10; // Limits the number of categories to return.
        })
        .Limt(10) // Limit returned offers
        .Match(imageTestBytes)
        .Subscribe(response =>
            {
                //Handle your response
                var offers = response.Offers;
            },
            throwable => ...
        );
```

The response will be an object of type `OfferResponseDto` that contains list of `Offers`, `RequestId` and  `PredictedCategories`

* If you specified a custom output format before, you should use this call to get response as `JSON` format :

```csharp
    nyris.ImageMatching
        .Match<JsonResponseDto(image)
        .Subscribe(response =>
            {
                var json = response.Content;
            },
            throwable => ...
        );
```

### Extract objects from your image 

Extract objects from an image:

```csharp
    nyris.ObjectProposal
        .ExtractObjects(image)
        .Subscribe(response /* List<DetectedObjectDto>*/=>
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

Before you mark an image as "not found", you will need to have the `RequestId`. For more details please check this [section](#match-your-first-image). 

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

### Xamarin Image Matching component for Android

To start using image matching component, you will need to refence the project `nyris.ui.Android` and include all the required dependencies with it. 

#### Starting the component

```csharp
    NyrisSearcher
        .Builder("Your API Key Here", ActivityOrFragment)
        .Start();
```

#### Setting image matching options

For more details about image matching options please check [this section](#match-your-first-image).

```csharp
    NyrisSearcher
        .Builder("Your API Key Here", ActivityOrFragment)
        .Exact(opt =>
        {
            Enabled = false; // disable exact matching
        })
        .Similarity(opt => //Performs similarity matching
        {
            Threshold = 0.5f; // The lower limit of confidences to be considered good from similarity
            Limit = 10; // The upper limit for the number of results to be returned from similarity
        })
        .Ocr(opt => //Performs optical character recognition on the images
        {
            Enabled = false; // disable OCR
        })
        .Start();
```

#### Handling returned results

```csharp 
protected override void OnActivityResult(int requestId, Result resultCode, Intent data)
{
    base.OnActivityResult(requestId, resultCode, data);

    if (resultCode == Result.Ok)
    {
        if (requestId == NyrisSearcher.REQUEST_ID)
        {
            try
            {
                var offerResponse = data.GetParcelableExtra(NyrisSearcher.SEARCH_RESULT_KEY) as OfferResponse;
                _tvResult.Text = $"Found ({offerResponse.Offers.Count}) offers, Categories : ({offerResponse.PredictedCategories.Count})";
            }
            catch
            {
                var offerResponse = data.GetParcelableExtra(NyrisSearcher.SEARCH_RESULT_KEY) as JsonResponse;
                _tvResult.Text = $"Response : {offerResponse.Content}";
            }
        }
    }
    else
    {
        //do something else
    }
}
```

#### Handling JSON results

If you specified a custom output format, you should use this call to get response as `JSON` format:

```csharp
    NyrisSearcher
        .Builder("Your API Key Here", ActivityOrFragment)
        .ResultAsJson()
        .Start();
```

#### Customizing the text of the component 

You can change text of the coponent by using

```csharp
    NyrisSearcher
        .Builder("Your API Key Here", this)
        .CaptureLabelText("My Capture label.")
        .CameraPermissionDeniedErrorMessage("You can not use this componenet until you activate the camera permission!")
        .ShouldShowCameraPermissionMessage("Should show message after second permission request")
        .DialogErrorTitle("Error Title")
        .PositiveButtonText("My OK")
        .Start();
```
#### Customizing the color of the component

To customize the color of the coponent you will need to override the defined colors:

```xml
<?xml version="1.0" encoding="utf-8"?>
<resources>
    <!-- You can override nyris searcher color here -->
    <color name="nyris_color_primary">YOUR_COLOR</color>
    <color name="nyris_color_primary_dark">YOUR_COLOR</color>
    <color name="nyris_color_accent">YOUR_COLOR</color>
</resources>
```

License
=======
    Copyright 2018 nyris GmbH
    
    Licensed under the Apache License, Version 2.0 (the "License");
    you may not use this file except in compliance with the License.
    You may obtain a copy of the License at
    
       http://www.apache.org/licenses/LICENSE-2.0
    
    Unless required by applicable law or agreed to in writing, software
    distributed under the License is distributed on an "AS IS" BASIS,
    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
    See the License for the specific language governing permissions and
    limitations under the License.