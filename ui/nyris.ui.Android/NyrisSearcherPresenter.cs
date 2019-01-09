using Android.Support.Annotation;
using Nyris.Sdk;
using Nyris.Sdk.Network.API.ImageMatching;
using Nyris.Sdk.Utils;
using Nyris.Ui.Android.Mvp;
using IO.Nyris.Camera;
using System.Reactive.Disposables;
using Android.Graphics;
using System.IO;
using Android.Content;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System;
using Nyris.Ui.Android.Resources.layout;
using Android.OS;
using Nyris.Ui.Android.Custom;
using Nyris.Ui.Android.Models;
using Nyris.Sdk.Network.Model;

namespace Nyris.Ui.Android
{
    internal class NyrisSearcherPresenter : Java.Lang.Object, SearcherContract.IPresenter, ICallback
    {
        private enum PresenterStatus { CamerListening, Cropping, Searching }
        private SearcherContract.IView _view;
        private IImageMatchingApi _imageMatchingApi;
        private NyrisSearcherConfig _config;
        private CompositeDisposable _compositeDisposable;
        private Bitmap _bitmapForCropping;
        private Size _imageSize;
        private PresenterStatus _presenterStatus;

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
            _compositeDisposable = null;
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
            _view?.TakePicture();
        }

        public void OnError(string message)
        {
            _view.ShowError(message);
        }

        public void OnPictureTakenOriginal(BaseCameraView cameraView, byte[] image)
        {
            _view?.StopCamera();
            _bitmapForCropping = BitmapFactory.DecodeByteArray(image, 0, image.Length);
            _imageSize = new Size(_bitmapForCropping.Width, _bitmapForCropping.Height);
            _view?.SetImPreviewBitmap(_bitmapForCropping);

            _view?.ShowImageCameraPreview();
            _view?.ShowViewCropper();
            _view?.ShowValidateView();
            _view?.ResetViewCropper();
            _presenterStatus = PresenterStatus.Cropping;
        }

        public void OnImageCrop(RectF rectF)
        {
            var newRectF = NormalizeRectF(rectF);

            var croppedBitmap = Bitmap.CreateBitmap(_bitmapForCropping,
                (int)newRectF.Left,
                (int)newRectF.Top,
                (int)newRectF.Width(),
                (int)newRectF.Height());

            var stream = new MemoryStream();
            croppedBitmap.Compress(Bitmap.CompressFormat.Jpeg, 100, stream);
            var bitmapData = stream.ToArray();
            var context = _view as Context;
            var image = ImageUtils.Companion.Resize(context, bitmapData, 512, 512);

            _view?.HideCircleView();
            _view?.HideValidateView();
            _view?.ShowLoading();
            _view?.HideViewCropper();

            if (_config.ResponseType == typeof(JsonResponse))
            {
                _imageMatchingApi
                    .Limit(_config.Limit)
                    .Match<JsonResponseDto>(image)
                    .SubscribeOn(NewThreadScheduler.Default)
                    .ObserveOn(new LooperScheduler(Looper.MainLooper))
                    .Subscribe(response =>
                    {
                        _view?.SenResult(new JsonResponse
                        {
                            Content = response.Content
                        });
                    }, throwable => _view?.ShowError(throwable.Message))
                    .AdToCompositeDisposable(_compositeDisposable);

            }
            else
            {
                _imageMatchingApi
                    .Limit(_config.Limit)
                    .Match(image)
                    .SubscribeOn(NewThreadScheduler.Default)
                    .ObserveOn(new LooperScheduler(Looper.MainLooper))
                    .Subscribe(response =>
                    {
                        _view?.SenResult(new OfferResponse(response));
                    }, throwable => _view?.ShowError(throwable.Message))
                    .AdToCompositeDisposable(_compositeDisposable);
            }
            _presenterStatus = PresenterStatus.Searching;
        }

        public void OnBackPressed()
        {
            if (_presenterStatus == PresenterStatus.Searching)
            {
                ClearDisposables();
                _view?.HideLoading();
                _view?.ShowCircleView();
                _view?.ShowValidateView();
                _view?.ShowViewCropper();
                _view?.ResetViewCropper();
                _presenterStatus = PresenterStatus.Cropping;
            }
            else if (_presenterStatus == PresenterStatus.Cropping)
            {
                _view?.HideLoading();
                _view?.HideViewCropper();
                _view?.HideValidateView();
                _view?.HideImageCameraPreview();

                _view?.ShowCircleView();
                _view?.ShowLabelCapture();
                _view?.StartCamera();
                _presenterStatus = PresenterStatus.CamerListening;
            }
            else
            {
                _view?.Close();
            }
        }

        public void OnOkErrorClick()
        {
            if (_presenterStatus == PresenterStatus.CamerListening)
            {
                _view?.Close();
                return;
            }

            ClearDisposables();
            _view?.HideLoading();
            _view?.ShowCircleView();
            _view?.ShowValidateView();
            _view?.ShowViewCropper();
            _view?.ResetViewCropper();
        }

        private void ClearDisposables()
        {
            _compositeDisposable?.Clear();
        }

        private RectF NormalizeRectF(RectF rectF)
        {
            var newRectF = new RectF(rectF);

            if (newRectF.Left < 0)
                newRectF.Left = 0f;
            if (newRectF.Top < 0)
                newRectF.Top = 0f;
            if (newRectF.Bottom > _bitmapForCropping.Height)
                newRectF.Bottom = _bitmapForCropping.Height;
            if (newRectF.Right > _bitmapForCropping.Width)
                newRectF.Right = _bitmapForCropping.Width;

            return newRectF;
        }

        #region Ignore
        public void OnCameraClosed(BaseCameraView cameraView)
        {
        }

        public void OnCameraOpened(BaseCameraView cameraView)
        {
        }

        public void OnPictureTaken(BaseCameraView cameraView, byte[] image)
        {
        }
        #endregion
    }
}
