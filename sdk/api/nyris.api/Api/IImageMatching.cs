using JetBrains.Annotations;
using Nyris.Api.Api.RequestOptions;

namespace Nyris.Api.Api;

/// <summary>
///     Interface to core image matching options.
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IImageMatching<out T>
    where T : class
{
    /// <summary>
    ///     Configures API filters.
    /// </summary>
    /// <param name="options">The settings.</param>
    [NotNull]
    T Filters([CanBeNull] Action<NyrisFilterOption> options = null);
}