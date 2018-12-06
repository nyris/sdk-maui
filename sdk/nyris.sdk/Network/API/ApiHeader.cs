using System;
using Nyris.Sdk.Utils;

namespace Nyris.Sdk.Network.API
{
    public class ApiHeader
    {
        public string ApiKey { get; set; }
        public string OutputFormat { get; set; } = Constants.DEFAULT_OUTPUT_FORMAT;
        public string Language { get; set; } = Constants.DEFAULT_LANGUAGE;
        
        private string SdkId { get; }
        private string SdkVersion { get; }
        private string PlatformVersion { get; }

        private string _userAgent;
        public string UserAgent => _userAgent ?? (_userAgent = $"{SdkId}/{SdkVersion} ({PlatformVersion})");

        public ApiHeader(string apiKey, string sdkId, string sdkVersion, string platformVersion)
        {
            ApiKey = apiKey;
            SdkId = sdkId;
            SdkVersion = sdkVersion;
            PlatformVersion = platformVersion;
        }
    }
}