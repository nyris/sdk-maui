using System;
using System.Reactive.Disposables;

namespace Nyris.UI.Android.Extensions;

internal static class PresenterExtensions
{
    internal static void AdToCompositeDisposable(this IDisposable disposable, CompositeDisposable compositeDisposable)
    {
        compositeDisposable.Add(disposable);
    }
}