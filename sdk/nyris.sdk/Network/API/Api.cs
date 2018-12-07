namespace Nyris.Sdk.Network.API
{
    public class Api
    {
        protected readonly ApiHeader _apiHeader;

        protected Api(ApiHeader apiHeader) =>
            _apiHeader = apiHeader;

        protected virtual string BuildXOptions()
        {
            return string.Empty;
        }
    }
}