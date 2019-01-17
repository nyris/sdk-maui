using System.Collections.Generic;
using Android.Graphics;
using Android.Support.Annotation;
using IO.Nyris.Camera;
using Nyris.UI.Android.Models;

namespace Nyris.UI.Android.Mvp
{
    internal class SearcherContract
    {
        internal interface IPresenter : IMvpPresenter<IView>
        {
            void OnSearchConfig([NonNull] NyrisSearcherConfig config);
            void OnResume();
            void OnPause();
            void OnCircleVieClickw();
            void OnImageCrop(RectF rectF);
            void OnBackPressed();
            void OnOkErrorClick();
            void OnPermissionsDenied(IList<string> permissions);
        }

        internal interface IView : IMvpView<IPresenter>
        {
            void StartCircleViewAnimation();
            void SetCaptureLabel(string label);
            void AddCameraCallback([NonNull] ICallback callback);
            void RemoveCameraCallback([NonNull] ICallback callback);
            void StartCamera();
            void StopCamera();
            void HideLabelCapture();
            void ShowLabelCapture();
            void ShowImageCameraPreview();
            void HideImageCameraPreview();
            void ShowCircleView();
            void HideCircleView();
            void ShowValidateView();
            void HideValidateView();
            void ShowLoading();
            void HideLoading();
            void TakePicture();
            void SetImPreviewBitmap([NonNull] Bitmap bitmap);
            void ResetViewCropper();
            void HideViewCropper();
            void ShowViewCropper();
            void SenResult(OfferResponse offerResponse);
            void SenResult(JsonResponse jsonResponse);
            void Close();
        }
    }
}
