using System;
using System.Net.Http;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Nyris.Api.Api.Feedback
{
    /// <summary>
    /// Interface to the Feedback API.
    /// </summary>
    public interface IFeedbackApi
    {
        /// <summary>
        /// Marks a request as unsuccessful.
        /// </summary>
        /// <param name="requestId">The request ID.</param>
        /// <returns>An <see cref="IObservable{T}"/> returning the HTTP status code.</returns>
        [NotNull]
        IObservable<HttpResponseMessage> MarkAsNotFound([NotNull] string requestId);

        /// <summary>
        /// Marks a request as unsuccessful.
        /// </summary>
        /// <param name="requestId">The request ID.</param>
        /// <returns>A <see cref="Task{T}"/> returning the HTTP status code.</returns>
        [NotNull]
        Task<HttpResponseMessage> MarkAsNotFoundAsync([NotNull] string requestId);
    }
}
