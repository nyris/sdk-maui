using System.Collections.Generic;
using System.Linq;
using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Support.V4.Content;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using IO.Nyris.Camera;
using IO.Nyris.Croppingview;
using Java.Interop;
using Newtonsoft.Json;
using Nyris.UI.Android.Extensions;
using Nyris.UI.Android.Custom;
using Nyris.UI.Android.Models;
using Nyris.UI.Android.Mvp;
using Pub.Devrel.Easypermissions;
using AlertDialog = Android.App.AlertDialog;

namespace Nyris.UI.Android
{
    [Activity(Label = "NyrisSearcherActivity", Theme = "@style/NyrisSearcherTheme")]
    internal sealed class NyrisSearcherActivity : AppCompatActivity, SearcherContract.IView,
        EasyPermissions.IPermissionCallbacks
    {
        private const int PermisionRequestCode = 1234;
        private CameraView _cameraView;
        private CircleView _circleViewBtn;
        private View _progress;
        private ImageCameraView _imagePreview;
        private PinViewCropper _viewCropper;
        private TextView _captureLabel;
        private View _validateBtn;

        public NyrisSearcherConfig _config;
        private SearcherContract.IPresenter _presenter;
        private bool isPermissionDeniedOnce;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.SearcherLayout);

            #region Views
            _cameraView = FindViewById<CameraView>(Resource.Id.camera);
            _circleViewBtn = FindViewById<CircleView>(Resource.Id.cvTakePic);
            _progress = FindViewById(Resource.Id.vProgress);
            _imagePreview = FindViewById<ImageCameraView>(Resource.Id.imPreview);
            _viewCropper = FindViewById<PinViewCropper>(Resource.Id.pinViewCropper);
            _captureLabel = FindViewById<TextView>(Resource.Id.tvCaptureLabel);
            _validateBtn = FindViewById(Resource.Id.imValidate);
            #endregion

            var extraJson = Intent.GetStringExtra(NyrisSearcher.CONFIG_KEY);
            _config = JsonConvert.DeserializeObject<NyrisSearcherConfig>(extraJson);
            _presenter = new NyrisSearcherPresenter();
            _presenter?.OnSearchConfig(_config);

            _presenter?.OnAtach(this);
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            var grantResultsInt = grantResults.Select(permission =>
            {
                if (permission == Permission.Granted)
                {
                    return 0;
                }
                else
                {
                    return -1;
                }
            }).ToArray();
            EasyPermissions.OnRequestPermissionsResult(requestCode, permissions, grantResultsInt, this);
        }

        protected override void OnResume()
        {
            base.OnResume();
            if (ContextCompat.CheckSelfPermission(this, Manifest.Permission.Camera) == Permission.Granted)
            {
                _presenter?.OnResume();
            }
            else
            {
                if (isPermissionDeniedOnce) return;
                isPermissionDeniedOnce = true;
                EasyPermissions.RequestPermissions(
                    this,
                    _config?.ShouldShowCameraPermissionMessage,
                    PermisionRequestCode,
                    Manifest.Permission.Camera);
            }
        }

        protected override void OnPause()
        {
            base.OnPause();
            _presenter?.OnPause();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            _presenter?.OnDetach();
        }

        public override void OnBackPressed()
        {
            _presenter?.OnBackPressed();
        }

        #region Listeners
        [Export("onCircleViewClick")]
        public void OnCircleViewClick(View v)
        {
            _presenter?.OnCircleVieClickw();
        }

        [Export("onValidateClick")]
        public void OnValidateClick(View v)
        {
            _presenter?.OnImageCrop(_viewCropper.SelectedObjectProposal);
        }
        #endregion

        public void OnPermissionsDenied(int p0, IList<string> permissions)
        {
            _presenter.OnPermissionsDenied(permissions);
        }

        public void OnPermissionsGranted(int p0, IList<string> permissions)
        {
        }

        public void StartCircleViewAnimation()
        {
            _circleViewBtn?.StartAnimation(FindViewById(Resource.Id.vPosCam));
        }

        public void SetCaptureLabel(string label)
        {
            _captureLabel.Text = label;
        }

        public void AddCameraCallback(ICallback callback)
        {
            _cameraView?.AddCallback(callback);
        }

        public void RemoveCameraCallback(ICallback callback)
        {
            _cameraView?.RemoveCallback(callback);
        }

        public void StartCamera()
        {
            _cameraView?.Start();
        }

        public void StopCamera()
        {
            _cameraView?.Stop();
        }

        public void HideLabelCapture()
        {
            _captureLabel.Hide();
        }

        public void ShowLabelCapture()
        {
            _captureLabel.Show();
        }

        public void ShowImageCameraPreview()
        {
            _imagePreview.Show();
        }

        public void HideImageCameraPreview()
        {
            _imagePreview.Hide();
        }

        public void ShowCircleView()
        {
            _circleViewBtn.Show();
        }

        public void HideCircleView()
        {
            _circleViewBtn.Hide();
        }

        public void ShowValidateView()
        {
            _validateBtn.Show();
        }

        public void HideValidateView()
        {
            _validateBtn.Hide();
        }

        public void ShowLoading()
        {
            _progress.Show();
        }

        public void HideLoading()
        {
            _progress.Hide();
        }

        public void TakePicture()
        {
            _cameraView?.TakePicture();
        }

        public void SetImPreviewBitmap(Bitmap bitmap)
        {
            _imagePreview.SetBitmap(bitmap);
        }

        public void ShowError(string message)
        {
            ShowDialog(message);
        }

        public void ResetViewCropper()
        {
            _viewCropper?.Reset();
        }

        public void HideViewCropper()
        {
            _viewCropper.Hide();
        }

        public void ShowViewCropper()
        {
            _viewCropper.Show();
        }

        public void Close()
        {
            SetResult(Result.Canceled, null);
            base.OnBackPressed();
        }

        public void SenResult(OfferResponse offerResponse)
        {
            SetResult(offerResponse);
            Finish();
        }

        public void SenResult(JsonResponse jsonResponse)
        {
            SetResult(jsonResponse);
            Finish();
        }

        private void SetResult(IParcelable parcelable)
        {
            var result = new Intent();
            result.PutExtra(NyrisSearcher.SEARCH_RESULT_KEY, parcelable);
            SetResult(Result.Ok, result);
        }

        private void ShowDialog(string message)
        {
            var alertDialog = new AlertDialog.Builder(this);
            alertDialog.SetTitle(_config.DialogErrorTitle);
            alertDialog.SetMessage(message);
            alertDialog.SetCancelable(false);
            alertDialog.SetPositiveButton(_config.PositiveButtonText, (s, a) =>
            {
                _presenter?.OnOkErrorClick();
            });
            alertDialog.Show();
        }
    }
}
