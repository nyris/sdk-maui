using Android.Support.Annotation;
using Nyris.Sdk;
using Nyris.Sdk.Network.API.ImageMatching;
using Nyris.Sdk.Utils;
using Nyris.Ui.Android.Mvp;
using IO.Nyris.Camera;
using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Concurrency;
using Rx.Xamarin.Android.Core;
using Android.Graphics;

namespace Nyris.Ui.Android
{
    internal class NyrisSearcherPresenter : Java.Lang.Object, SearcherContract.IPresenter, ICallback
    {
        private SearcherContract.IView _view;
        private IImageMatchingApi _imageMatchingApi;
        private NyrisSearcherConfig _config;
        private CompositeDisposable _compositeDisposable;

        public void OnAtach(SearcherContract.IView view)
        {
            _view = view;
            _view?.StartCircleViewAnimation();
            _compositeDisposable = new CompositeDisposable();
        }

        public void OnDetach()
        {
            ClearDisposables();
            _view = null;
        }

        public void OnPause()
        {
            _view.RemoveCameraCallback(this);
            _view?.StopCamera();
        }

        public void OnResume()
        {
            _view.AddCameraCallback(this);
            _view?.StartCamera();
        }

        public void OnSearchConfig([NonNull]NyrisSearcherConfig config)
        {
            _config = config;

            var nyris = NyrisApi.CreateInstance(_config.ApiKey, Platform.Android, _config.IsDebug);
            _imageMatchingApi = nyris.ImageMatchingAPi;
            //Apply Config
        }

        public void OnCircleVieClickw()
        {
            _view?.HideCircleView();
            _view?.TakePicture();
        }

        public void OnCameraClosed(BaseCameraView cameraView)
        {
        }

        public void OnCameraOpened(BaseCameraView cameraView)
        {
        }

        public void OnError(string message)
        {
            _view.ShowError(message);
        }

        public void OnPictureTaken(BaseCameraView cameraView, byte[] image)
        {
            /*
            _imageMatchingApi
                .Limit(_config.Limit)
                .Match(image)
                .SubscribeOn(NewThreadScheduler.Default)
                .ObserveOn(AndroidSchedulers.MainThread())
                .Subscribe(response =>
                {
                    Console.WriteLine("#### Image Matching");
                    Console.WriteLine(response);
                }, throwable => _view?.ShowError(throwable.Message))
                .AdToCompositeDisposable(_compositeDisposable);
                */
        }

        public void OnPictureTakenOriginal(BaseCameraView cameraView, byte[] image)
        {
            _view?.StopCamera();
            var bitmap = BitmapFactory.DecodeByteArray(image, 0, image.Length);
            _view?.SetImPreviewBitmap(bitmap);

            _view?.ShowImageCameraPreview();
            _view?.ShowViewCropper();
            _view?.HideLabelCapture();
            _view?.ResetViewCropper();

        }

        private void ClearDisposables()
        {
            _compositeDisposable?.Clear();
            _compositeDisposable = null;
        }

        public void OnImageCrop(Bitmap croppedImage)
        {
            throw new NotImplementedException();
        }
    }
}
