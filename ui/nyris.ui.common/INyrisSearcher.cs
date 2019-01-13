using System;
using JetBrains.Annotations;
using Nyris.Api.Api;

namespace Nyris.Ui.Common
{
    public interface INyrisSearcher : IMatchResultFormat<INyrisSearcher>, IImageMatching<INyrisSearcher>
    {
        [NotNull]
        INyrisSearcher ResultAsJson();

        [NotNull]
        INyrisSearcher ResultAsObject();

        void Show();
    }
}
