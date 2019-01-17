namespace Nyris.Ui.Android.Mvp
{
    internal interface IMvpView<Presenter>
    {
        void ShowError(string message);
    }
}
