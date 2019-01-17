using System;
using Android.Views;

namespace Nyris.UI.Android.Extensions
{
    internal static class ViewExtensions
    {
        internal static void Hide(this View view)
        {
            view.Visibility = ViewStates.Invisible;
        }

        internal static void Show(this View view)
        {
            view.Visibility = ViewStates.Visible;
        }
    }
}
