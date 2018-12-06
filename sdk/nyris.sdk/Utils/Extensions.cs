namespace Nyris.Sdk.Utils
{
    public static class Extensions
    {
        public static bool IsEmpty(this string myString)
        {
            return string.IsNullOrEmpty(myString);
        }
    }
}