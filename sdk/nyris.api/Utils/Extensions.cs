using JetBrains.Annotations;

namespace Nyris.Api.Utils
{
    internal static class Extensions
    {
        [ContractAnnotation("value:null=>true")]
        public static bool IsEmpty([CanBeNull] this string value) => string.IsNullOrEmpty(value);
    }
}
