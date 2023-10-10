using JetBrains.Annotations;
using Nyris.Api.Model;

namespace Nyris.Api.Api.ImageMatching;

/// <summary>
///     Interface to the nyris image matching API.
/// </summary>
public interface IImageMatchingApi : IMatchResultFormat<IImageMatchingApi>, IImageMatching<IImageMatchingApi>
{
    /// <summary>
    ///     Obtains item matches for the specified <paramref name="image." />
    /// </summary>
    /// <param name="image">The image to search with.</param>
    /// <param name="imageName">The image name to send with the request.</param>
    /// <param name="contentType">The image MIME Types.</param>
    /// <returns>An <see cref="IObservable{T}" /> returning the results.</returns>
    [NotNull]
    IObservable<OfferResponseDto> Match([NotNull] byte[] image, string imageName = "image.jpg", string contentType = "image/jpeg");

    /// <summary>
    ///     Obtains item matches for the specified <paramref name="image." />
    /// </summary>
    /// <param name="image">The image to search with.</param>
    /// <param name="imageName">The image name to send with the request.</param>
    /// <param name="contentType">The image MIME Types.</param>
    /// <returns>A <see cref="Task{T}" /> returning the results.</returns>
    [NotNull]
    [ItemNotNull]
    Task<OfferResponseDto> MatchAsync([NotNull] byte[] image, string imageName = "image.jpg", string contentType = "image/jpeg");
}