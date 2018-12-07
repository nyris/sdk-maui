using System;
using System.Collections.Generic;
using System.Net.Http;
using Io.Nyris.Sdk.Network.Model;
using Io.Nyris.Sdk.Network.Service;

namespace Io.Nyris.Sdk.Network.API.ObjectProposal
{
    public class ObjectProposalApi : Api, IObjectProposalApi
    {
        private readonly IObjectProposalService _objectProposalService;
        
        public ObjectProposalApi(IObjectProposalService objectProposalService, 
            ApiHeader apiHeader) : base(apiHeader)
        {
            _objectProposalService = objectProposalService;
        }

        public IObservable<List<DetectedObject>> ExtractObjects(byte[] image) => ExtractObjects<List<DetectedObject>>(image);

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
    }
}