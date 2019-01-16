# nyris SDK for .Net and Xamarin
![](nyris_logo.png)

Introduction
------
nyris is a high performance visual product search, object detection and visual recommendations engine 
for retail and industry. 

For more information please see [nyris.io](https://nyris.io/)

We provide a SDK with better error handling and reactive/asynchronous programming support.
The SDK is written in [C#](https://docs.microsoft.com/en-us/previous-versions/visualstudio/visual-studio-2012/kx37x362(v%3dvs.110)). 

The SDK offers :
-----
* Support of Reactive/Asynchronous programming paradigm
* Better error handling
* Type-safe HTTP client
* Unified response
* All the different nyris services 

nyris services 
-----
We offer : 
* Visual search
* Similarity search by image
* Object detection
* Manual matching
* Text Search

Requirements
----- 
* Images in **JPEG** format
* The minimum dimensions of the image are `512x512 px`
* The maximum size of the image is less than or equal to `500 KB` 

Solution
-----
The current solution is composed of multiple projects:
* **nyris API**: Which represents sdk of nyris APIs.

* **nyris Console Demo**: Which represents demo console app that shows different usages of the API.

* **nyris ui Android**: Which represents nyris Image matching or Seracher.

* **nyris ui Camera Android**: Which represents a Camera binding jar project for this [library](https://github.com/nyris/Camera.Android).

* **nyris ui Cropping Android**: Which represents a image cropping ui binding jar project.

* **nyris ui Demo Android**: Which represents Android demo app that shows how to use nyris Search component.

Get Started with nyris SDK
-----
### Jump To

* [Get instance](#get-instance)
* [Match your first image](#match-your-first-image)
* [Extract objects from your image](#extract-objects-from-your-image)
* [Mark sent image as not found](#mark-sent-image-as-not-found)
* [Text Match Search](#text-match-search)

### Get instance 
First, initialize an instance of `NyrisApi` with your API Key :
 
```csharp
static class Program
{
    public static async Task Main(string[] args)
    {
        var nyris = NyrisApi.CreateInstance("Your Api Key",/*Select your platform*/ Platform.Generic , /*Enable debug mode*/true);
    }
}
```

### Match your first image 
#### Basic way to match an image : 

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


#### Advanced way to match an image : 

```csharp
    //For more details about available feed attributes please check our documentation : http://docs.nyris.io/#available-feed-attributes.
    val imageByteArray : ByteArray = /* Your byte array */
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
The response is an object of type `OfferResponseDto` that contains list of `Offers`, `RequestCode` and  `PredictedCategories`

* If you specified a custom output format, you should use this call to get response as `Json` format :

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
Extract objects from an image

```csharp
    nyris.ObjectProposal
        .ExtractObjects(image)
        .Subscribe(response /* List<DetectedObjectDto>*/=>
        {
            // Handle your response
            var objects = response;
        }, throwable => ...);
```
Returned response is a List of Detected Object. 

The extracted object has:
* `Confidence` is the probability of the top item. Value range between : `0-1`.
* `Region` is a Bounding box. It represents the location and the size of the object in the sent image. 


### Mark sent image as not found
It may happen that our service can't recognize or match an image. This is why we provide you a service to notify us
about the unrecognized image.

Before you mark an image as not found, you will need to have the `RequestCode`. For more details please check this [section](#match-your-first-image). 

After getting the `RequestCode` you can mark the image as not found. 

```csharp
    nyris.Feedback
        .MarkAsNotFound(RequestCode)
        .Subscribe(response /* HttpResponseMessage */=>
        {
            // Handle your response...
        }, throwable => ...);
```

### Text Match Search
nyris offers a service that helps you to search offers by text, SKU, barcode, etc.

you can use the text search service the same way as [image matching service](#match-your-first-image)

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

Get Started with nyris Xamarin Searcher
-----

### Xamarin Image Matching component for Android
To start using Image Matching component you will need to refence the project `nyris.ui.Android` and iclude all the required dependencies with it. 

#### Start the component: 
```csharp
    NyrisSearcher
        .Builder("Your API Key Here", ActivityOrFragment)
        .Start();
```

#### Custom image matching options :
For more details about image matching options please check this [section](#match-your-first-image)
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

#### Handle returned results : 
```csharp 
protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
{
    base.OnActivityResult(requestCode, resultCode, data);

    if (resultCode == Result.Ok)
    {
        if (requestCode == NyrisSearcher.REQUEST_CODE)
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

#### Json Results : 
If you specified a custom output format, you should use this call to get response as `Json` format :
```csharp
    NyrisSearcher
        .Builder("Your API Key Here", ActivityOrFragment)
        .ResultAsJson()
        .Start();
```

#### Customize text of the component 
You can change text of the coponent by using : 
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
#### Customize color of the component:
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