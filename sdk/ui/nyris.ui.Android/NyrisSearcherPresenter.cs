using IO.Nyris.Camera;
using System.Reactive.Disposables;
using Android.Graphics;
using Android.Content;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using Android.OS;
using Nyris.Api;
using Android;
using AndroidX.Annotations;
using Nyris.UI.Android.Custom;
using Nyris.UI.Android.Extensions;
using Nyris.UI.Android.Models;
using Nyris.UI.Android.Mvp;
using Nyris.UI.Common;

namespace Nyris.UI.Android
{
    internal class NyrisSearcherPresenter : Java.Lang.Object, SearcherContract.IPresenter, ICallback
    {
        private enum PresenterStatus { CameraListening, Cropping, Searching }
        private SearcherContract.IView _view;
        private INyrisApi _nyrisApi;
        private NyrisSearcherConfig _config;
        private CompositeDisposable _compositeDisposable;
        private Bitmap _bitmapForCropping;
        private Size _imageSize;
        private PresenterStatus _presenterStatus;
        private RectF _newRectF;

        public void OnAtach(SearcherContract.IView view)
        {
            _compositeDisposable = new CompositeDisposable();
            _view = view;
            checkApiKey();
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
            if (_presenterStatus == PresenterStatus.Cropping)
            {
                return;
            }

            if (_config.LoadLastState)
            {
                var bitmap = BitmapFactory.DecodeFile(_config.LastTakenPicturePath);
                var region = _config.LastCroppingRegion;
                _config.LastCroppingRegion = null;
                _config.LoadLastState = false;

                if (bitmap == null)
                {
                    // fall back
                    _view?.StartCamera();
                }
                else
                {
                    StartCroppingMode(bitmap, region);
                    _view?.HideValidateView();
                }
            }
            else
            {
                _view?.StartCamera();
            }
        }

        public void OnSearchConfig([NonNull]NyrisSearcherConfig config)
        {
            _config = config;

            var httpClientHandler = new Xamarin.Android.Net.AndroidClientHandler();
            _nyrisApi = NyrisApi.CreateInstance(_config.ApiKey, Platform.Android, httpClientHandler, _config.IsDebug);
            MapConfig();
        }

        private void checkApiKey()
        {
            var isValid = !string.IsNullOrEmpty(_config.ApiKey) && !_config.ApiKey.Contains(" ");
            if (!isValid)
            {
                OnError("Please set a correct api key");
            }
        }

        public void OnCircleViewClick()
        {
            _view?.TakePicture();
        }

        public void OnCircleViewAnimationEnd()
        {
            if (_presenterStatus == PresenterStatus.Cropping)
            {
                _view?.ShowValidateView();
            }
        }

        public void OnError(string message)
        {
            _view.ShowError(message);
        }

        public void OnPictureTakenOriginal(BaseCameraView cameraView, byte[] image)
        {
            _view?.StopCamera();
            var bitmap = BitmapFactory.DecodeByteArray(image, 0, image.Length);
            StartCroppingMode(bitmap);
        }

        public void OnImageCrop(RectF rectF)
        {
            _presenterStatus = PresenterStatus.Searching;
            _newRectF = NormalizeRectF(rectF);

            var croppedBitmap = Bitmap.CreateBitmap(_bitmapForCropping,
                (int)_newRectF.Left,
                (int)_newRectF.Top,
                (int)_newRectF.Width(),
                (int)_newRectF.Height());

            var stream = new MemoryStream();
            croppedBitmap.Compress(Bitmap.CompressFormat.Jpeg, 100, stream);
            var bitmapData = stream.ToArray();
            var context = _view as Context;
            var image = ImageUtils.Companion.Resize(context, bitmapData, 512, 512);

            _view?.HideCircleView();
            _view?.HideValidateView();
            _view?.ShowLoading();
            _view?.HideViewCropper();
            _view?.SaveLastCroppingRegion(_newRectF.Left, _newRectF.Top, _newRectF.Right, _newRectF.Bottom);
            
            _nyrisApi.ImageMatching
                .Limit(_config.Limit)
                .Match(image)
                .SubscribeOn(NewThreadScheduler.Default)
                .ObserveOn(new LooperScheduler(Looper.MainLooper))
                .Subscribe(response =>
                {
                    _view?.SendResult(new OfferResponse(response)
                    {
                        TakenImagePath = _config.LastTakenPicturePath
                    });
                }, throwable => _view?.ShowError(throwable.Message))
                .AdToCompositeDisposable(_compositeDisposable);
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
                _presenterStatus = PresenterStatus.CameraListening;
            }
            else
            {
                _view?.Close();
            }
        }

        public void OnOkErrorClick()
        {
            if (_presenterStatus == PresenterStatus.CameraListening)
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
            if (_newRectF == null)
            {
                _view?.ResetViewCropper();
            }
            else
            {
                _view?.ResetViewCropper(_newRectF);
            }
        }

        public void OnPermissionsDenied(IList<string> permissions)
        {
            foreach (var permission in permissions)
            {
                if (permission == Manifest.Permission.Camera)
                {
                    _view.ShowError(_config.CameraPermissionDeniedErrorMessage);
                }
                if (permission == Manifest.Permission.WriteExternalStorage)
                {
                    _view.ShowError(_config.ExternalStoragePermissionDeniedErrorMessage);
                }
            }
        }

        private void StartCroppingMode(Bitmap bitmap, Common.Region region = null)
        {
            _presenterStatus = PresenterStatus.Cropping;
            _bitmapForCropping = bitmap;
            _imageSize = new Size(_bitmapForCropping.Width, _bitmapForCropping.Height);
            _view?.SetImPreviewBitmap(_bitmapForCropping);

            _view?.ShowImageCameraPreview();
            _view?.ShowViewCropper();
            _view?.ShowValidateView();
            if (region != null && region.IsValid)
            {
                _view?.ResetViewCropper(new RectF
                {
                    Left = region.Left,
                    Top = region.Top,
                    Right = region.Right,
                    Bottom = region.Bottom
                });
            }
            else
            {
                _view?.ResetViewCropper();
            }
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
            _nyrisApi.ImageMatching.Language(_config.Language);
            _nyrisApi.ImageMatching.Limit(_config.Limit);

            if (_config.NyrisFilterOption != null)
            {
                _nyrisApi.ImageMatching.Filters(obj =>
                {
                    obj.Enabled = _config.NyrisFilterOption.Enabled;
                    _config.NyrisFilterOption.Filters.ForEach(filter =>
                    {
                        obj.AddFilter(filter.Type, filter.Values);
                    });
                });
            }
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
