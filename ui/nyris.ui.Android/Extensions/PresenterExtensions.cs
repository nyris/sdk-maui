using System;
using System.Reactive.Disposables;

namespace Nyris.Ui.Android.Resources.layout
{
    internal static class PresenterExtensions
    {
        internal static void AdToCompositeDisposable(this IDisposable disposable, CompositeDisposable compositeDisposable)
        {
            compositeDisposable.Add(disposable);
        }
    }
}
