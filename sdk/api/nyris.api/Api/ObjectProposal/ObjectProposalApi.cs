using JetBrains.Annotations;
using Nyris.Api.Model;
using Nyris.Api.Service;
using Nyris.Api.Utils;

namespace Nyris.Api.Api.ObjectProposal;

internal class ObjectProposalApi : ApiBase, IObjectProposalApi
{
    private readonly IObjectProposalService _service;

    /// <summary>
    ///     Initializes a new instance of the <see cref="ObjectProposalApi" /> class.
    /// </summary>
    /// <param name="service">The object proposal service to use.</param>
    /// <param name="apiHeader">The HTTP headers.</param>
    internal ObjectProposalApi([NotNull] IObjectProposalService service, [NotNull] ApiHeader apiHeader)
        : base(apiHeader)
    {
        _service = service ?? throw new ArgumentNullException(nameof(service));
    }

    /// <inheritdoc cref="IObjectProposalApi.ExtractObjects" />
    public IObservable<RegionsObjectDto> ExtractObjects(byte[] image)
    {
        return ExtractObjects<RegionsObjectDto>(image);
    }

    /// <inheritdoc cref="IObjectProposalApi.ExtractObjectsAsync" />
    public Task<RegionsObjectDto> ExtractObjectsAsync(byte[] image)
    {
        return ExtractObjectsAsync<RegionsObjectDto>(image);
    }

    /// <inheritdoc cref="IObjectProposalApi.ExtractObjects{T}" />
    public IObservable<T> ExtractObjects<T>(byte[] image)
    {
        var byteContent = new ByteArrayContent(image);
        return _service.ExtractObjects<T>(Constants.DefaultResultFormat,
            ApiHeader.UserAgent,
            ApiHeader.ApiKey,
            "image/jpg",
            image.Length.ToString(),
            byteContent);
    }

    /// <inheritdoc cref="IObjectProposalApi.ExtractObjectsAsync{T}" />
    public Task<T> ExtractObjectsAsync<T>(byte[] image)
    {
        var byteContent = new ByteArrayContent(image);
        return _service.ExtractObjectsAsync<T>(Constants.DefaultResultFormat,
            ApiHeader.UserAgent,
            ApiHeader.ApiKey,
            "image/jpg",
            image.Length.ToString(),
            byteContent);
    }
}