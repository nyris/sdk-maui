using JetBrains.Annotations;
using Nyris.Api.Api;

namespace Nyris.UI.Common;

public interface INyrisSearcher<out T> : IMatchResultFormat<T>, IImageMatching<T>
    where T : class
{
    T ApiKey([NotNull] string apiKey);
        
    T CaptureLabelText(string label);
        
    T DialogErrorTitle(string title);
        
    T AgreeButtonTitle(string title);
        
    T CancelButtonTitle(string title);

    T CameraPermissionDeniedErrorMessage(string message);

    T ExternalStoragePermissionDeniedErrorMessage(string message);

    T LoadLastState(bool loadLastState);
        
    T CameraPermissionRequestIfDeniedMessage(string message);

    T ConfigurationFailedErrorMessage(string message);
        
    T BackLabelText(string label);
        
    void Start(Action<NyrisSearcherResult?> callback);
}