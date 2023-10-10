using AndroidX.Activity.Result;
using AndroidX.Activity.Result.Contract;
using AndroidX.AppCompat.App;
using Nyris.UI.Android.Extensions;
using Nyris.UI.Common;
using JObject = Java.Lang.Object;

namespace Nyris.UI.Android.Custom;

internal class ActivityForResultObserver: Java.Lang.Object, Application.IActivityLifecycleCallbacks
{
    internal static ActivityResultLauncher? ActivityResultLauncher;
    internal static Action<NyrisSearcherResult?>? NyrisSearcherResultCallback;
    
    public void OnActivityCreated(Activity activity, Bundle? savedInstanceState)
    {
        if (activity is not AppCompatActivity appCompatActivity 
            || activity.Class.SimpleName.Equals("NyrisSearcherActivity"))
        {
            return;
        }
        
        ActivityResultLauncher = appCompatActivity.RegisterForActivityResult(
            new ActivityResultContracts.StartActivityForResult(), new ActivityResultCallback(
                activityResult =>
                {
                    NyrisSearcherResultCallback?.Invoke(activityResult?.ToNyrisSearchResult());
                    NyrisSearcherResultCallback = null;
                }));
    }

    public void OnActivityDestroyed(Activity activity)
    {
        if (activity.Class.SimpleName.Equals("NyrisSearcherActivity")) return;
        ActivityResultLauncher = null;
        NyrisSearcherResultCallback = null;
    }

    public void OnActivityStopped(Activity activity)
    {
        //NO-OP
    }

    public void OnActivityStarted(Activity activity)
    {
        //NO-OP
    }

    public void OnActivityPaused(Activity activity)
    {
        //NO-OP
    }

    public void OnActivityResumed(Activity activity)
    {
        //NO-OP
    }

    public void OnActivitySaveInstanceState(Activity activity, Bundle outState)
    {
        //NO-OP
    }
}

public class ActivityResultCallback : JObject, IActivityResultCallback
{
    readonly Action<ActivityResult?> _callback;
    public ActivityResultCallback(Action<ActivityResult?> callback) => _callback = callback;
    public void OnActivityResult(JObject? result) => _callback(result as ActivityResult);
}