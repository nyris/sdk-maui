namespace Nyris.UI.Android.Mvp
{
    internal interface IMvpView<Presenter>
    {
        void ShowError(string message);
    }
}
