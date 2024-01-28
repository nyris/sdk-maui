using Nyris.Api.Api.RequestOptions;
using Nyris.UI.Common;
using Nyris.UI.iOS.EventArgs;

namespace Nyris.UI.iOS;

public interface INyrisSearcher : INyrisSearcher<INyrisSearcher>
{       
    INyrisSearcher Theme(AppearanceConfiguration? theme);
}

public class NyrisSearcher : INyrisSearcher
{
    private class CaptureSessionParametres
    {
        public UIImage Screenshot;
        public CGRect CroppingFrame;
    }

    public event EventHandler<Exception> RequestFailed;
    public Action<NyrisSearcherResult?> ResultCallback;

    private UIViewController _presenterController;
    private CropController _cropController;
    private CaptureSessionParametres _previousSessionParametres;
    private NyrisSearcherConfig _config;
    private AppearanceConfiguration _controllerAppearance;
        
    private NyrisSearcher(string apiKey, UIViewController presenterController,bool debug)
    {
        if (string.IsNullOrEmpty(apiKey))
        {
            throw new ArgumentNullException("apiKey", "apiKey cannot be empty or null");
        }

        _config = new NyrisSearcherConfig
        {
            ApiKey = apiKey,
            IsDebug = debug
        };
        this._presenterController = presenterController;
        _previousSessionParametres = new CaptureSessionParametres();
    }

    public static INyrisSearcher Builder(string apiKey, UIViewController presenterController, bool debug = false)
    {
        return new NyrisSearcher(apiKey, presenterController, debug);
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

    public INyrisSearcher CameraPermissionDeniedErrorMessage(string message = "")
    {
        _config.CameraPermissionDeniedErrorMessage = message;
        return this;
    }

    public INyrisSearcher ExternalStoragePermissionDeniedErrorMessage(string message)
    {
        //NO-OP forced implementation by Android platform
        return this;
    }

    public INyrisSearcher CameraPermissionRequestIfDeniedMessage(string message)
    {            
        _config.CameraPermissionRequestIfDeniedMessage = message;
        return this;
    }

    public INyrisSearcher ConfigurationFailedErrorMessage(string message)
    {
        _config.ConfigurationFailedErrorMessage = message;
        return this;
    }

    public INyrisSearcher CaptureLabelText(string label)
    {
        _config.CaptureLabelText = label;
        return this;
    }

    public INyrisSearcher DialogErrorTitle(string title)
    {
        _config.DialogErrorTitle = title;
        return this;
    }

    public INyrisSearcher BackLabelText(string label)
    {
        _config.BackLabelText = label;
        return this;
    }

    public INyrisSearcher ApiKey(string apiKey)
    {
        _config.ApiKey = apiKey;
        return this;
    }

    public INyrisSearcher Language(string language)
    {
        _config.Language = language ?? throw new ArgumentException("language is null");
        return this;
    }

    public INyrisSearcher Limit(int limit)
    {
        _config.Limit = limit;
        return this;
    }

    public INyrisSearcher CategoryPrediction(Action<CategoryPredictionOptions>? options = null)
    {
        options ??= opt => { opt.Enabled = true; };
        _config.CategoryPredictionOptions = new CategoryPredictionOptions();
        options(_config.CategoryPredictionOptions);
        return this;
    }

    public INyrisSearcher Filters(Action<NyrisFilterOption>? options = null)
    {            
        options ??= opt => { opt.Enabled = true; };
        _config.NyrisFilterOption = new NyrisFilterOption();
        options(_config.NyrisFilterOption);
        return this;
    }

    public INyrisSearcher Theme(AppearanceConfiguration theme)
    {
        _controllerAppearance = theme;
        return this;
    }

    public INyrisSearcher LoadLastState(bool LoadLastState)
    {
        _config.LoadLastState = LoadLastState;
        return this;
    }

    public void Start(Action<NyrisSearcherResult?> callback)
    {
        ResultCallback = callback;
        _cropController?.Dispose();
        var bundle = NSBundle.FromClass(new ObjCRuntime.Class(typeof(CameraController)));
        var storyboard = UIStoryboard.FromName("CameraController", bundle);

        if (_presenterController == null)
        {
            throw new ArgumentNullException(nameof(_presenterController), "Presenter view controller is null");
        }


        var controller = storyboard.InstantiateInitialViewController();
        if (controller is not CropController cropController)
        {
            throw new ArgumentNullException(nameof(controller), "Controller is not a type of CropController");
        }

        _cropController = cropController;
        _cropController.Configure(_config, _controllerAppearance);
        _cropController.OfferAvailable += OnOfferAvailable;
        _cropController.RequestFailed += (sender, exception) => RequestFailed?.Invoke(this, exception);

        if (_config.LoadLastState)
        {
            _cropController.ScreenshotImage = _previousSessionParametres.Screenshot;
            _cropController.CroppingFrame = _previousSessionParametres.CroppingFrame;
        }
        else
        {
            _cropController.ScreenshotImage?.Dispose();
            _cropController.ScreenshotImage = null;
            GC.Collect();
        }
        _cropController.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;
        _presenterController.PresentViewController(_cropController, true, null);
    }

    void OnOfferAvailable(object sender, OfferResponseEventArgs e)
    {
        try
        {
            ResultCallback(e.Results);
            _previousSessionParametres.Screenshot = e.Screenshot;
            _previousSessionParametres.CroppingFrame = e.CroppingFrame;
            e.Screenshot = null;
        }
        catch (Exception ex)
        {
            RequestFailed?.Invoke(this, ex);
        }

    }
}