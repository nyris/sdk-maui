using JetBrains.Annotations;
using Nyris.Api.Api;

namespace Nyris.Ui.Common
{
    public interface INyrisSearcher : IMatchResultFormat<INyrisSearcher>, IImageMatching<INyrisSearcher>
    {
        INyrisSearcher ApiKey([NotNull] string apiKey);

        [NotNull]
        INyrisSearcher ResultAsJson();

        [NotNull]
        INyrisSearcher ResultAsObject();

        void Start();
    }
}
