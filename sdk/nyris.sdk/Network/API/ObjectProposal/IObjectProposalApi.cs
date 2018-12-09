using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Nyris.Sdk.Network.Model;

namespace Nyris.Sdk.Network.API.ObjectProposal
{
    public interface IObjectProposalApi
    {
        IObservable<List<DetectedObject>> ExtractObjects([NotNull] byte[] image);
        
        Task<List<DetectedObject>> ExtractObjectsAsync([NotNull] byte[] image);
        
        IObservable<T> ExtractObjects<T>([NotNull] byte[] image);
        
        Task<T> ExtractObjectsAsync<T>([NotNull] byte[] image);
    }
}