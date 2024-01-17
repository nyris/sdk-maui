using System.Collections.Generic;
using Android.Graphics;
using AndroidX.Annotations;
using IO.Nyris.Camera;
using JetBrains.Annotations;
using Nyris.UI.Android.Models;
using Nyris.UI.Common;

namespace Nyris.UI.Android.Mvp
{
    internal class SearcherContract
    {
        internal interface IPresenter : IMvpPresenter<IView>
        {
            void OnSearchConfig([NonNull] NyrisSearcherConfig config, AndroidThemeConfig? themeConfig);
            void OnResume();
            void OnPause();
            void OnCircleViewClick();
            void OnCircleViewAnimationEnd();
            void OnImageCrop(RectF rectF);
            void OnBackPressed();
            void OnOkErrorClick();
            void OnPermissionsDenied(IList<string> permissions);
        }

        internal interface IView : IMvpView<IPresenter>
        {
            void TintViews(AndroidThemeConfig theme);
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
            void ResetViewCropper(RectF defaultRect);
            void HideViewCropper();
            void ShowViewCropper();
            void SaveLastCroppingRegion(float left, float top, float right, float bottom);
            void SendResult(OfferResponse offerResponse);
            void Close();
        }
    }
}
