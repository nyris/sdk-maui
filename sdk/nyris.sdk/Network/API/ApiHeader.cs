using System;

namespace Nyris.Sdk.Network.API
{
    public class ApiHeader
    {
        public string ApiKey { get; set; }
        public string SdkId { get; }
        public string SdkVersion { get; }
        public string GitCommitHash { get; }
        public string PlatformVersion { get; }

        private string _userAgent;
        public string UserAgent => _userAgent ?? (_userAgent = $"{SdkId}/{SdkVersion} ({GitCommitHash} {PlatformVersion})");

        public ApiHeader(string apiKey, string sdkId, string sdkVersion, string gitCommitHash, string platformVersion)
        {
            ApiKey = apiKey;
            SdkId = sdkId;
            SdkVersion = sdkVersion;
            GitCommitHash = gitCommitHash;
            PlatformVersion = platformVersion;
        }
    }
}