using System;
using System.Collections.Generic;
using Nyris.Sdk.Network.Model;

namespace Nyris.Sdk.Network.API.ObjectProposal
{
    public interface IObjectProposalApi
    {
        IObservable<List<DetectedObject>> ExtractObjects(byte[] image);
        
        IObservable<T> ExtractObjects<T>(byte[] image);
    }
}