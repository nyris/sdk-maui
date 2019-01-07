namespace Nyris.Api.Model
{
    public class JsonResponseDto : INyrisResponse
    {
        internal JsonResponseDto(string json)
        {
            Content = json;
        }

        public string Content { get; }

        public override string ToString()
        {
            return $"Json: {Content}";
        }
    }
}
