using JetBrains.Annotations;
using Nyris.Api.Service;

namespace Nyris.Api.Api.Feedback;

public class FeedbackApi : ApiBase, IFeedbackApi
{
    private readonly IManualSearchService _service;

    /// <summary>
    ///     Initializes a new instance of the <see cref="FeedbackApi" /> class.
    /// </summary>
    /// <param name="service">The feedback service to use.</param>
    /// <param name="apiHeader">The HTTP headers.</param>
    internal FeedbackApi([NotNull] IManualSearchService service, [NotNull] ApiHeader apiHeader) :
        base(apiHeader)
    {
        _service = service ?? throw new ArgumentNullException(nameof(service));
    }

    /// <inheritdoc cref="IFeedbackApi.MarkAsNotFound" />
    public IObservable<HttpResponseMessage> MarkAsNotFound(string requestId)
    {
        return _service.MarkAsNotFound(ApiHeader.UserAgent,
            ApiHeader.ApiKey,
            requestId);
    }

    /// <inheritdoc cref="IFeedbackApi.MarkAsNotFoundAsync" />
    public Task<HttpResponseMessage> MarkAsNotFoundAsync(string requestId)
    {
        return _service.MarkAsNotFoundAsync(ApiHeader.UserAgent,
            ApiHeader.ApiKey,
            requestId);
    }
}