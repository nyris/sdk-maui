using JetBrains.Annotations;
using Nyris.Api.Api;

namespace Nyris.UI.Common
{
    public interface INyrisSearcher<out T> : IMatchResultFormat<T>, IImageMatching<T>
        where T : class
    {
        T ApiKey([NotNull] string apiKey);

        [NotNull]
        T ResultAsJson();

        [NotNull]
        T ResultAsObject();

        void Start();
    }
}
