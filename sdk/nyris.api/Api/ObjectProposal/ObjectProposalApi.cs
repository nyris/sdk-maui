using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Nyris.Api.Model;
using Nyris.Api.Service;

namespace Nyris.Api.Api.ObjectProposal
{
    internal class ObjectProposalApi : ApiBase, IObjectProposalApi
    {
        private readonly IObjectProposalService _service;

        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectProposalApi"/> class.
        /// </summary>
        /// <param name="service">The object proposal service to use.</param>
        /// <param name="apiHeader">The HTTP headers.</param>
        internal ObjectProposalApi([NotNull] IObjectProposalService service, [NotNull] ApiHeader apiHeader)
            : base(apiHeader)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        /// <inheritdoc cref="IObjectProposalApi.ExtractObjects"/>
        public IObservable<List<DetectedObjectDto>> ExtractObjects(byte[] image)
            => ExtractObjects<List<DetectedObjectDto>>(image);

        /// <inheritdoc cref="IObjectProposalApi.ExtractObjectsAsync"/>
        public Task<List<DetectedObjectDto>> ExtractObjectsAsync(byte[] image)
            => ExtractObjectsAsync<List<DetectedObjectDto>>(image);

        /// <inheritdoc cref="IObjectProposalApi.ExtractObjects{T}"/>
        public IObservable<T> ExtractObjects<T>(byte[] image)
        {
            var byteContent = new ByteArrayContent(image);
            return _service.ExtractObjects<T>(accept: ApiHeader.ResultFormat,
                userAgent: ApiHeader.UserAgent,
                apiKey: ApiHeader.ApiKey,
                contentType: "image/jpg",
                contentLength: image.Length.ToString(),
                image: byteContent);
        }

        /// <inheritdoc cref="IObjectProposalApi.ExtractObjectsAsync{T}"/>
        public Task<T> ExtractObjectsAsync<T>(byte[] image)
        {
            var byteContent = new ByteArrayContent(image);
            return _service.ExtractObjectsAsync<T>(accept: ApiHeader.ResultFormat,
                userAgent: ApiHeader.UserAgent,
                apiKey: ApiHeader.ApiKey,
                contentType: "image/jpg",
                contentLength: image.Length.ToString(),
                image: byteContent);
        }
    }
}
