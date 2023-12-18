using Nyris.Api.Api.RequestOptions;
using Nyris.UI.Common;
#if __ANDROID__
using AndroidX.AppCompat.App;
using NativeNyrisSearcher = Nyris.UI.Android.NyrisSearcher;
#elif __IOS__
using UIKit;
using NativeNyrisSearcher = Nyris.UI.iOS.NyrisSearcher;
#endif


namespace Nyris.UI.Maui;

public class NyrisSearcher : INyrisSearcher<NyrisSearcher>
{
    private readonly NyrisSearcherConfig _config;
    
    private NyrisSearcher(string apiKey, bool debug)
    {
        _config = new NyrisSearcherConfig
        {
            ApiKey = apiKey,
            IsDebug = debug
        };
    }

    public NyrisSearcher Language(string language)
    {
        _config.Language = language ?? throw new ArgumentException("language is null"); ;
        return this;
    }

    public NyrisSearcher Limit(int limit)
    {
        _config.Limit = limit;
        return this;
    }

    public NyrisSearcher CategoryPrediction(Action<CategoryPredictionOptions> options = null)
    {
        options ??= opt => { opt.Enabled = true; };
        _config.CategoryPredictionOptions = new CategoryPredictionOptions();
        options(_config.CategoryPredictionOptions);
        return this;
    }

    public NyrisSearcher Filters(Action<NyrisFilterOption> options = null)
    {
        options ??= opt => { opt.Enabled = true; };
        _config.NyrisFilterOption = new NyrisFilterOption();
        options(_config.NyrisFilterOption);
        return this;
    }

    public NyrisSearcher ApiKey(string apiKey)
    {
        _config.ApiKey = apiKey;
        return this;
    }

    public NyrisSearcher LoadLastState(bool loadLastState)
    {
        _config.LoadLastState = loadLastState;
        return this;
    }

    public NyrisSearcher CaptureLabelText(string label)
    {
        _config.CaptureLabelText = label;
        return this;
    }

    public NyrisSearcher DialogErrorTitle(string title)
    {
        _config.DialogErrorTitle = title;
        return this;
    }

    public NyrisSearcher AgreeButtonTitle(string title)
    {
        _config.AgreeButtonTitle = title;
        return this;
    }

    public NyrisSearcher CancelButtonTitle(string title)
    {
        _config.CancelButtonTitle = title;
        return this;
    }

    public NyrisSearcher CameraPermissionDeniedErrorMessage(string message)
    {
        _config.CameraPermissionDeniedErrorMessage = message;
        return this;
    }

    public NyrisSearcher ExternalStoragePermissionDeniedErrorMessage(string message)
    {
        _config.ExternalStoragePermissionDeniedErrorMessage = message;
        return this;
    }

    public NyrisSearcher CameraPermissionRequestIfDeniedMessage(string message)
    {
        _config.CameraPermissionRequestIfDeniedMessage = message;
        return this;
    }

    public NyrisSearcher ConfigurationFailedErrorMessage(string message)
    {
        _config.ConfigurationFailedErrorMessage = message;
        return this;
    }

    public NyrisSearcher BackLabelText(string label)
    {
        _config.BackLabelText = label;
        return this;
    }

    public void Start(Action<NyrisSearcherResult> callback)
    {
#if __ANDROID__
        if (Platform.CurrentActivity is not AppCompatActivity appCompatActivity)
        {
            throw new ArgumentException("Activity is null or Activity should extend AndroidX.AppCompat.App.AppCompatActivity");
        }
        var nativeSearcher = NativeNyrisSearcher.Builder(_config.ApiKey, appCompatActivity, _config.IsDebug);
#elif __IOS__ 
        if (Platform.GetCurrentUIViewController() is not UIViewController viewController)
        {
            throw new ArgumentException("UIViewController is null");
        }
        var nativeSearcher = NativeNyrisSearcher.Builder(_config.ApiKey, viewController, _config.IsDebug);
#endif
        nativeSearcher
            .AgreeButtonTitle(_config.AgreeButtonTitle)
            .CancelButtonTitle(_config.CancelButtonTitle)
            .CameraPermissionDeniedErrorMessage(_config.CameraPermissionDeniedErrorMessage)
            .ExternalStoragePermissionDeniedErrorMessage(_config.ExternalStoragePermissionDeniedErrorMessage)
            .CameraPermissionRequestIfDeniedMessage(_config.CameraPermissionRequestIfDeniedMessage)
            .ConfigurationFailedErrorMessage(_config.ConfigurationFailedErrorMessage)
            .CaptureLabelText(_config.CaptureLabelText)
            .DialogErrorTitle(_config.DialogErrorTitle)
            .BackLabelText(_config.BackLabelText)
            .Language(_config.Language)
            .Limit(_config.Limit)
            .LoadLastState(_config.LoadLastState)
            // forward the filters to the native searcher on Android/iOS implementation
            .Filters(action =>
            {
                if (_config.NyrisFilterOption == null)
                {
                    action.Enabled = false;
                    return;
                }

                action.Enabled = _config.NyrisFilterOption.Enabled;
                _config.NyrisFilterOption.Filters.ForEach(filter =>
                {
                    action.AddFilter(filter.Type, filter.Values);
                });
            })
            // forward the filters to the native searcher on Android/iOS implementation
            .CategoryPrediction(action =>
            {
                if (_config.CategoryPredictionOptions == null)
                {
                    action.Enabled = false;
                    return;
                }
                action.Enabled = _config.CategoryPredictionOptions.Enabled;
                action.Limit = _config.CategoryPredictionOptions.Limit;
                action.Threshold = _config.CategoryPredictionOptions.Threshold;
            })
            .Start(callback);
    }
    
    public static INyrisSearcher<NyrisSearcher> Builder(string apiKey, bool debug = false)
    {
        return new NyrisSearcher(apiKey, debug);
    }
}