using System;
using Nyris.Api.Api;
using Nyris.Api.Api.RequestOptions;
using Nyris.Api.Model;
using Nyris.Api.Utils;

namespace Nyris.UI.Common
{
    public class NyrisSearcherConfig
    {
        bool loadLastState { get; set; }
        public bool IsDebug { get; set; }

        public string ApiKey { get; set; }

        public string Language { get; set; } = Constants.DefaultLanguage;

        public int Limit { get; set; } = OptionDefaults.DefaultLimit;

        public bool LoadLastState { get; set; } = false;

        public string LastTakenPicturePath { get; set; }

        public Region LastCroppingRegion { get; set; }

        public string DialogErrorTitle { get; set; } = "Error";

        public string AgreeButtonTitle { get; set; } = "OK";

        public string CancelButtonTitle { get; set; } = "Cancel";

        public string CaptureLabelText { get; set; } = "Capture your product";

        public string BackLabelText { get; set; } = "Capture your product";

        public string ConfigurationFailedErrorMessage { get; set; } = "Camera setup failed!";

        public string CameraPermissionDeniedErrorMessage { get; set; } = "Camera permission denied!";

        public string ExternalStoragePermissionDeniedErrorMessage { get; set; } = "Access External Storage permission denied!";

        public string CameraPermissionRequestIfDeniedMessage { get; set; } = "Please authorize camera access";
        
        public NyrisFilterOption? NyrisFilterOption { get; set; }
    }
}