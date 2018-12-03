using System.Collections.Generic;

namespace Nyris.Sdk.Network.API
{
    public class Api
    {
        private readonly ApiHeader _apiHeader;
        private Dictionary<string, string> _defaultHeader;
        
        public Dictionary<string, string> DefaultHeadersMap => _defaultHeader ?? (_defaultHeader =new Dictionary<string, string>
        {
            ["X-Api-Key"] = _apiHeader.ApiKey,
            ["User-Agent"] = _apiHeader.ApiKey
        });

        public Api(ApiHeader apiHeader)
        {
            _apiHeader = apiHeader;
        }

        public string BuildXOptions()
        {
            return string.Empty;
        }
    }
}