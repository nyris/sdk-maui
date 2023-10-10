using Android.Content;
using Android.Database;
using Uri = Android.Net.Uri;

namespace Nyris.UI.Android.Custom;

[ContentProvider(authorities: new[] { "${applicationId}.ActivityForResultProvider" }, Exported = false, Name = "nyris.ui.android.custom.ActivityForResultProvider")]
internal class ActivityForResultProvider : ContentProvider
{
    public override bool OnCreate()
    {
        if (Context is not Application) return false;
        var application = Context as Application;
        application?.RegisterActivityLifecycleCallbacks(new ActivityForResultObserver());
        return false;
    }
    
    public override int Delete(Uri uri, string? selection, string[]? selectionArgs)
    {
        //NO-OP
        return -1;
    }

    public override string? GetType(Uri uri)
    {
        //NO-OP
        return null;
    }

    public override Uri? Insert(Uri uri, ContentValues? values)
    {
        //NO-OP
        return null;
    }

    public override ICursor? Query(Uri uri, string[]? projection, string? selection, string[]? selectionArgs, string? sortOrder)
    {
        //NO-OP
        return null;
    }

    public override int Update(Uri uri, ContentValues? values, string? selection, string[]? selectionArgs)
{        //NO-OP
        return -1;
    }
}