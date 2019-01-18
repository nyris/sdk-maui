namespace Nyris.UI.Android.Mvp
{
    internal interface IMvpPresenter<View>
    {
        void OnAtach(View view);

        void OnDetach();
    }
}
