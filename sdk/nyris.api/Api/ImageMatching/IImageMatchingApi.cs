using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Nyris.Api.Model;

namespace Nyris.Api.Api.ImageMatching
{
    /// <summary>
    /// Interface to the nyris image matching API.
    /// </summary>
    public interface IImageMatchingApi : IMatchResultFormat<IImageMatchingApi>, IImageMatching<IImageMatchingApi>
    {
        /// <summary>
        /// Obtains item matches for the specified <paramref name="image."/>
        /// </summary>
        /// <param name="image">The image to search with.</param>
        /// <returns>An <see cref="IObservable{T}"/> returning the results.</returns>
        [NotNull]
        IObservable<OfferResponseDto> Match([NotNull] byte[] image);

        /// <summary>
        /// Obtains item matches for the specified <paramref name="image."/>
        /// </summary>
        /// <param name="image">The image to search with.</param>
        /// <returns>An <see cref="IObservable{T}"/> returning the results.</returns>
        [NotNull]
        IObservable<T> Match<T>([NotNull] byte[] image) where T : INyrisResponse;

        /// <summary>
        /// Obtains item matches for the specified <paramref name="image."/>
        /// </summary>
        /// <param name="image">The image to search with.</param>
        /// <returns>A <see cref="Task{T}"/> returning the results.</returns>
        [NotNull, ItemNotNull]
        Task<OfferResponseDto> MatchAsync([NotNull] byte[] image);

        /// <summary>
        /// Obtains item matches for the specified <paramref name="image."/>
        /// </summary>
        /// <param name="image">The image to search with.</param>
        /// <returns>A <see cref="Task{T}"/> returning the results.</returns>
        [NotNull, ItemNotNull]
        Task<T> MatchAsync<T>([NotNull] byte[] image) where T : INyrisResponse;
    }
}
