using System;
using JetBrains.Annotations;
using Nyris.Api.Api.RequestOptions;

namespace Nyris.Api.Api
{
    /// <summary>
    /// Interface to core image matching options.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IImageMatching<out T> : IRegrouping<T>
        where T : class
    {
        /// <summary>
        /// Configures exact image matching.
        /// </summary>
        /// <param name="options">The settings.</param>
        [NotNull]
        T Exact([CanBeNull] Action<ExactOptions> options = null);

        /// <summary>
        /// Configures image similarity matching.
        /// </summary>
        /// <param name="options">The settings.</param>
        [NotNull]
        T Similarity([CanBeNull] Action<SimilarityOptions> options = null);

        /// <summary>
        /// Configures text recognition.
        /// </summary>
        /// <param name="options">The settings.</param>
        [NotNull]
        T Ocr([CanBeNull] Action<OcrOptions> options = null);

        /// <summary>
        /// Configures recommendation mode.
        /// </summary>
        /// <param name="options">The settings.</param>
        [NotNull]
        T RecommendationMode([CanBeNull] Action<RecommendationModeOptions> options = null);

        /// <summary>
        /// Configures category prediction.
        /// </summary>
        /// <param name="options">The settings.</param>
        [NotNull]
        T CategoryPrediction([CanBeNull] Action<CategoryPredictionOptions> options = null);
    }
}
