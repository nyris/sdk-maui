using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Nyris.Sdk.Network.Model;
using Nyris.Sdk.Network.Service;

namespace Nyris.Sdk.Network.API.ObjectProposal
{
    public class ObjectProposalApi : Api, IObjectProposalApi
    {
        private readonly IObjectProposalService _objectProposalService;
        
        public ObjectProposalApi(IObjectProposalService objectProposalService, 
            ApiHeader apiHeader) : base(apiHeader) => _objectProposalService = objectProposalService;

        public IObservable<List<DetectedObject>> ExtractObjects(byte[] image) => ExtractObjects<List<DetectedObject>>(image);
        
        public Task<List<DetectedObject>> ExtractObjectsAsync(byte[] image) => ExtractObjectsAsync<List<DetectedObject>>(image);

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