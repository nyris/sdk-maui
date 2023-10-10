using Android.Content;
using AndroidX.Annotations;
using AndroidX.AppCompat.App;
using Newtonsoft.Json;
using Nyris.Api.Api.RequestOptions;
using Nyris.UI.Android.Custom;
using Nyris.UI.Common;

namespace Nyris.UI.Android;

public interface INyrisSearcher : INyrisSearcher<INyrisSearcher>
{
}

public class NyrisSearcher : INyrisSearcher
{
    internal const string SearchResultKey = "SEARCH_RESULT_KEY";

    internal const string ConfigKey = "CONFIG_KEY";

    private readonly NyrisSearcherConfig _config;
    private readonly Activity _activity;

    private NyrisSearcher([NonNull] string apiKey, [NonNull] AppCompatActivity activity, bool debug)
    {
        _config = new NyrisSearcherConfig
        {
            ApiKey = apiKey,
            IsDebug = debug
        };
        _activity = activity;
    }

    public static INyrisSearcher Builder(string apiKey, AppCompatActivity activity, bool debug = false)
    {
        return new NyrisSearcher(apiKey, activity, debug);
    }

    public INyrisSearcher CameraPermissionDeniedErrorMessage([NonNull] string message)
    {
        _config.CameraPermissionDeniedErrorMessage = message;
        return this;
    }

    public INyrisSearcher ExternalStoragePermissionDeniedErrorMessage([NonNull] string message)
    {
        _config.ExternalStoragePermissionDeniedErrorMessage = message;
        return this;
    }

    public INyrisSearcher CaptureLabelText([NonNull] string label)
    {
        _config.CaptureLabelText = label;
        return this;
    }

    public INyrisSearcher LoadLastState(bool state)
    {
        _config.LoadLastState = state;
        return this;
    }

    public INyrisSearcher CameraPermissionRequestIfDeniedMessage(string message)
    {
        //NO-OP forced implementation by iOS platform
        return this;
    }

    public INyrisSearcher ConfigurationFailedErrorMessage(string message)
    {
        //NO-OP forced implementation by iOS platform
        return this;
    }

    public INyrisSearcher BackLabelText(string label)
    {
        //NO-OP forced implementation by iOS platform
        return this;
    }

    public INyrisSearcher DialogErrorTitle([NonNull] string title)
    {
        _config.DialogErrorTitle = title;
        return this;
    }

    public INyrisSearcher AgreeButtonTitle(string title)
    {
        _config.AgreeButtonTitle = title;
        return this;
    }

    public INyrisSearcher CancelButtonTitle(string title)
    {
        _config.CancelButtonTitle = title;
        return this;
    }

    public INyrisSearcher ApiKey([NonNull] string apiKey)
    {
        _config.ApiKey = apiKey;
        return this;
    }

    public INyrisSearcher Language([NonNull] string language)
    {
        _config.Language = language ?? throw new ArgumentException("language is null");
        return this;
    }

    public INyrisSearcher Limit(int limit)
    {
        _config.Limit = limit;
        return this;
    }

    public INyrisSearcher Filters(Action<NyrisFilterOption>? options = null)
    {
        options ??= opt => { opt.Enabled = true; };
        _config.NyrisFilterOption = new NyrisFilterOption();
        options(_config.NyrisFilterOption);
        return this;
    }

    public void Start(Action<NyrisSearcherResult?> callback)
    {
        var configJson = JsonConvert.SerializeObject(_config);
        var intent = new Intent(_activity, typeof(NyrisSearcherActivity));
        intent.PutExtra(ConfigKey, configJson);
        ActivityForResultObserver.NyrisSearcherResultCallback = callback;
        ActivityForResultObserver.ActivityResultLauncher?.Launch(intent);
    }
}