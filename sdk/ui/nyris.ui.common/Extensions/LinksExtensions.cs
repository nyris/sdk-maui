using Nyris.Api.Model;
using Nyris.UI.Common.Models;

namespace Nyris.UI.Common.Extensions;

public static class LinksExtensions
{
    public static Links ToNyrisLinks(this LinksDto? linksModeDto)
    {
        return new Links(linksModeDto?.Main, linksModeDto?.Mobile);
    }
}