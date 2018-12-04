using System.Collections.Generic;

namespace Nyris.Sdk.Network.API
{
    public class Api
    {
        private readonly ApiHeader _apiHeader;
        private Dictionary<string, string> _defaultHeader;
        
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