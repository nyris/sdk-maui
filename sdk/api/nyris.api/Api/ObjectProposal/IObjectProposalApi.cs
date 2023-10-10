using JetBrains.Annotations;
using Nyris.Api.Model;

namespace Nyris.Api.Api.ObjectProposal;

/// <summary>
///     Interface to the Object Proposal API.
/// </summary>
public interface IObjectProposalApi
{
    /// <summary>
    ///     Identifies objects in the specified <paramref name="image" />.
    /// </summary>
    /// <param name="image">The image to detect objects in.</param>
    /// <returns>An <see cref="IObservable{T}" /> of lists of detected objects.</returns>
    [NotNull]
    IObservable<RegionsObjectDto> ExtractObjects([NotNull] byte[] image);

    /// <summary>
    ///     Identifies objects in the specified <paramref name="image" />.
    /// </summary>
    /// <param name="image">The image to detect objects in.</param>
    /// <returns>A <see cref="Task{T}" /> returning a list of detected objects.</returns>
    [NotNull]
    Task<RegionsObjectDto> ExtractObjectsAsync([NotNull] byte[] image);

    /// <summary>
    ///     Identifies objects in the specified <paramref name="image" />.
    /// </summary>
    /// <param name="image">The image to detect objects in.</param>
    /// <returns>An <see cref="IObservable{T}" /> of lists of detected objects.</returns>
    /// <typeparam name="T">The type of the result document.</typeparam>
    [NotNull]
    IObservable<T> ExtractObjects<T>([NotNull] byte[] image);

    /// <summary>
    ///     Identifies objects in the specified <paramref name="image" />.
    /// </summary>
    /// <param name="image">The image to detect objects in.</param>
    /// <returns>A <see cref="Task{T}" /> returning a list of detected objects.</returns>
    /// <typeparam name="T">The type of the result document.</typeparam>
    [NotNull]
    Task<T> ExtractObjectsAsync<T>([NotNull] byte[] image);
}