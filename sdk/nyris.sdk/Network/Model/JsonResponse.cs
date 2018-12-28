namespace Nyris.Sdk.Network.Model
{
    public class JsonResponse : INyrisResponse
    {
        internal JsonResponse(string json)
        {
            Content = json;
        }
        
        public string Content { get; }
    }
}