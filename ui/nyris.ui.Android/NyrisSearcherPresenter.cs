using Android.Support.Annotation;
using IO.Nyris.Camera;
using System.Reactive.Disposables;
using Android.Graphics;
using System.IO;
using Android.Content;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System;
using Nyris.UI.Android.Resources.layout;
using Android.OS;
using Nyris.Api;
using Nyris.Api.Model;
using System.Collections.Generic;
using Android;
using Nyris.UI.Android.Custom;
using Nyris.UI.Android.Models;
using Nyris.UI.Android.Mvp;

namespace Nyris.UI.Android
{
    internal class NyrisSearcherPresenter : Java.Lang.Object, SearcherContract.IPresenter, ICallback
    {
        private enum PresenterStatus { CamerListening, Cropping, Searching }
        private SearcherContract.IView _view;
        private INyrisApi _nyrisApi;
        private NyrisSearcherConfig _config;
        private CompositeDisposable _compositeDisposable;
        private Bitmap _bitmapForCropping;
        private Size _imageSize;
        private PresenterStatus _presenterStatus;

        public void OnAtach(SearcherContract.IView view)
        {
            _compositeDisposable = new CompositeDisposable();
            _view = view;
            _view?.SetCaptureLabel(_config.CaptureLabelText);
            _view?.StartCircleViewAnimation();
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
            _nyrisApi = NyrisApi.CreateInstance(_config.ApiKey, Platform.Android, _config.IsDebug);
            MapConfig();
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
                _nyrisApi.ImageMatching
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
                _nyrisApi.ImageMatching
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

            _presenterStatus = PresenterStatus.Cropping;
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

        private void MapConfig()
        {
            _nyrisApi.ImageMatching.OutputFormat(_config.OutputFormat);
            _nyrisApi.ImageMatching.Language(_config.Language);
            _nyrisApi.ImageMatching.Limit(_config.Limit);

            if (_config.ExactOptions != null)
            {
                _nyrisApi.ImageMatching.Exact(obj =>
                {
                    obj.Enabled = _config.ExactOptions.Enabled;
                });
            }

            if (_config.SimilarityOptions != null)
            {
                _nyrisApi.ImageMatching.Similarity(obj =>
                {
                    obj.Enabled = _config.SimilarityOptions.Enabled;
                    obj.Limit = _config.SimilarityOptions.Limit;
                    obj.Threshold = _config.SimilarityOptions.Threshold;
                });
            }

            if (_config.OcrOptions != null)
            {
                _nyrisApi.ImageMatching.Ocr(obj =>
                {
                    obj.Enabled = _config.OcrOptions.Enabled;
                });
            }

            if (_config.RegroupOptions != null)
            {
                _nyrisApi.ImageMatching.Regroup(obj =>
                {
                    obj.Enabled = _config.RegroupOptions.Enabled;
                    obj.Threshold = _config.RegroupOptions.Threshold;
                });
            }

            if (_config.RecommendationModeOptions != null)
            {
                _nyrisApi.ImageMatching.RecommendationMode(obj =>
                {
                    obj.Enabled = _config.RecommendationModeOptions.Enabled;
                });
            }

            if (_config.CategoryPredictionOptions != null)
            {
                _nyrisApi.ImageMatching.CategoryPrediction(obj =>
                {
                    obj.Enabled = _config.CategoryPredictionOptions.Enabled;
                    obj.Limit = _config.CategoryPredictionOptions.Limit;
                    obj.Threshold = _config.CategoryPredictionOptions.Threshold;
                });
            }
        }

        public void OnPermissionsDenied(IList<string> permissions)
        {
            foreach (var permission in permissions)
            {
                if (permission != Manifest.Permission.Camera)
                {
                    continue;
                }
            }
            _view.ShowError(_config.CameraPermissionDeniedErrorMessage);
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
