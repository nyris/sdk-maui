using System;
using Nyris.UI.iOS.Models;

namespace Nyris.UI.iOS.EventArgs
{
    public class OfferResponseEventArgs : System.EventArgs
    {
        public OfferResponse OfferResponse;
        
        public JsonResponse OfferJson;

        public OfferResponseEventArgs(OfferResponse offerResponse)
        {
            OfferResponse = offerResponse;
        }
        
        public OfferResponseEventArgs(JsonResponse offerJson)
        {
            OfferJson = offerJson;
        }
    }
}