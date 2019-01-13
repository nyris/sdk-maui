using JetBrains.Annotations;
using Nyris.Api.Utils;

namespace Nyris.Api.Api
{
    public sealed class ApiHeader
    {
        private const string SdkId = Constants.SdkId;
        private const string SdkVersion = Constants.SdkVersion;

        [CanBeNull]
        private readonly string _platform;

        [CanBeNull]
        private string _userAgent;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApiHeader"/> class.
        /// </summary>
        /// <param name="apiKey">The API key.</param>
        /// <param name="platform">The current platform.</param>
        public ApiHeader([NotNull] string apiKey, [CanBeNull] string platform)
        {
            ApiKey = apiKey;
            _platform = platform;
        }

        /// <summary>
        /// Gets or sets the API key.
        /// </summary>
        public string ApiKey { get; set; }

        /// <summary>
        /// Gets or sets the requested result format header value.
        /// </summary>
        public string ResultFormat { get; set; } = Constants.DefaultResultFormat;

        /// <summary>
        /// Gets or sets the accepted language header value.
        /// </summary>
        /// <remarks>
        /// Use <c>*</c> to indicate that any language is sufficient.
        /// </remarks>
        public string Language { get; set; } = Constants.DefaultLanguage;

        /// <summary>
        /// Gets the User-Agent header value.
        /// </summary>
        [NotNull]
        public string UserAgent => _userAgent ?? (_userAgent = $"{SdkId}/{SdkVersion} ({_platform})");
    }
}
