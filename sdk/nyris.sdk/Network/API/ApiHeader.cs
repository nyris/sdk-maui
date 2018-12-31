using JetBrains.Annotations;
using Nyris.Sdk.Utils;

namespace Nyris.Sdk.Network.API
{
    public class ApiHeader
    {
        private readonly string _sdkId = Constants.SDK_ID;
        private readonly string _sdkVersion = Constants.SDK_VERSION;
        private readonly string _platform;
        private string _userAgent;

        public ApiHeader([NotNull] string apiKey, string platform)
        {
            ApiKey = apiKey;
            _platform = platform;
        }

        public string ApiKey { get; set; }

        public string OutputFormat { get; set; } = Constants.DEFAULT_OUTPUT_FORMAT;

        public string Language { get; set; } = Constants.DEFAULT_LANGUAGE;

        public string UserAgent => _userAgent ?? (_userAgent = $"{_sdkId}/{_sdkVersion} ({_platform})");
    }
}