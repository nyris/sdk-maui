using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using IO.Nyris.Camera;
using IO.Nyris.Croppingview;
using Java.Interop;
using Newtonsoft.Json;
using Nyris.Ui.Android.Custom;
using Nyris.Ui.Android.Extensions;
using Nyris.Ui.Android.Models;
using Nyris.Ui.Android.Mvp;
using AlertDialog = Android.App.AlertDialog;

namespace Nyris.Ui.Android
{
    [Activity(Label = "NyrisSearcherActivity", Theme = "@style/NyrisSearcherTheme")]
    internal class NyrisSearcherActivity : AppCompatActivity, SearcherContract.IView
    {
        private CameraView _cameraView;
        private CircleView _circleViewBtn;
        private View _progress;
        private ImageCameraView _imagePreview;
        private PinViewCropper _viewCropper;
        private TextView _captureLabel;
        private View _validateBtn;

        private SearcherContract.IPresenter _presenter;

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
            var config = JsonConvert.DeserializeObject<NyrisSearcherConfig>(extraJson);
            _presenter = new NyrisSearcherPresenter();
            _presenter?.OnSearchConfig(config);

            _presenter?.OnAtach(this);
        }

        protected override void OnResume()
        {
            base.OnResume();
            _presenter?.OnResume();
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

        public void StartCircleViewAnimation()
        {
            _circleViewBtn?.StartAnimation(FindViewById(Resource.Id.vPosCam));
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
            alertDialog.SetTitle("Error");
            alertDialog.SetMessage(message);
            alertDialog.SetCancelable(false);
            alertDialog.SetPositiveButton("OK", (s, a) =>
            {
                _presenter?.OnOkErrorClick();
            });
            alertDialog.Show();
        }
    }
}
