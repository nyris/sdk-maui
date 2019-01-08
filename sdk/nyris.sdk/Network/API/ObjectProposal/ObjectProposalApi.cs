using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Nyris.Sdk.Network.Model;
using Nyris.Sdk.Network.Service;

namespace Nyris.Sdk.Network.API.ObjectProposal
{
    internal class ObjectProposalApi : Api, IObjectProposalApi
    {
        private readonly IObjectProposalService _objectProposalService;

        internal ObjectProposalApi(IObjectProposalService objectProposalService,
            ApiHeader apiHeader) : base(apiHeader) => _objectProposalService = objectProposalService;

        public IObservable<List<DetectedObjectDto>> ExtractObjects(byte[] image) =>
            ExtractObjects<List<DetectedObjectDto>>(image);

        public Task<List<DetectedObjectDto>> ExtractObjectsAsync(byte[] image) =>
            ExtractObjectsAsync<List<DetectedObjectDto>>(image);

        public IObservable<T> ExtractObjects<T>(byte[] image)
        {
            var byteContent = new ByteArrayContent(image);
            return _objectProposalService.ExtractObjects<T>(accept: _apiHeader.OutputFormat,
                userAgent: _apiHeader.UserAgent,
                apiKey: _apiHeader.ApiKey,
                contentType: "image/jpg",
                contentLength: image.Length.ToString(),
                image: byteContent);
        }

        public Task<T> ExtractObjectsAsync<T>(byte[] image)
        {
            var byteContent = new ByteArrayContent(image);
            return _objectProposalService.ExtractObjectsAsync<T>(accept: _apiHeader.OutputFormat,
                userAgent: _apiHeader.UserAgent,
                apiKey: _apiHeader.ApiKey,
                contentType: "image/jpg",
                contentLength: image.Length.ToString(),
                image: byteContent);
        }
    }
}