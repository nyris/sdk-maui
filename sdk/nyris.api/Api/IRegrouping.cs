using System;
using JetBrains.Annotations;
using Nyris.Api.Api.RequestOptions;

namespace Nyris.Api.Api
{
    /// <summary>
    /// Interface to result re-grouping options.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IRegrouping<out T>
        where T: class
    {
        /// <summary>
        /// Configures result re-grouping.
        /// </summary>
        /// <param name="options">The settings.</param>
        [NotNull]
        T Regroup([CanBeNull] Action<RegroupOptions> options = null);
    }
}
