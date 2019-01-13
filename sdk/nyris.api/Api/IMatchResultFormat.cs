using JetBrains.Annotations;

namespace Nyris.Api.Api
{
    /// <summary>
    /// Interface to the result format options.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IMatchResultFormat<out T>
        where T: class
    {
        /// <summary>
        /// Sets the required output format.
        /// </summary>
        /// <param name="outputFormat">The output format.</param>
        /// <returns>A reference to <typeparamref name="T"/>.</returns>
        [NotNull]
        T OutputFormat([CanBeNull] string outputFormat);

        /// <summary>
        /// Sets the requested result language.
        /// </summary>
        /// <param name="language">The result language.</param>
        /// <returns>A reference to <typeparamref name="T"/>.</returns>
        [NotNull]
        T Language([CanBeNull] string language);

        /// <summary>
        /// Sets the result limit.
        /// </summary>
        /// <param name="limit">The maximum number of results to return.</param>
        /// <returns>A reference to <typeparamref name="T"/>.</returns>
        [NotNull]
        T Limit(int limit);
    }
}
