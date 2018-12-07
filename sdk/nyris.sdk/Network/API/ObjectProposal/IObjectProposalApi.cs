using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Nyris.Sdk.Network.Model;

namespace Nyris.Sdk.Network.API.ObjectProposal
{
    public interface IObjectProposalApi
    {
        IObservable<List<DetectedObject>> ExtractObjects([NotNull] byte[] image);
        
        IObservable<T> ExtractObjects<T>([NotNull] byte[] image);
    }
}