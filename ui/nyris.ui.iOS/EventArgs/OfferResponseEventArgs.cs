using System;
using CoreGraphics;
using Nyris.UI.iOS.Models;
using UIKit;

namespace Nyris.UI.iOS.EventArgs
{
    public class OfferResponseEventArgs : System.EventArgs
    {
        public OfferResponse OfferResponse;
        
        public JsonResponse OfferJson;

        public UIImage Screenshot;

        public CGRect CroppingFrame;

        public OfferResponseEventArgs(UIImage screenshot, CGRect croppingFrame, OfferResponse offerResponse)
        {
            OfferResponse = offerResponse;
            Screenshot = screenshot;
            CroppingFrame = croppingFrame;
        }
        
        public OfferResponseEventArgs(UIImage screenshot, CGRect croppingFrame, JsonResponse offerJson)
        {
            OfferJson = offerJson;
            Screenshot = screenshot;
            CroppingFrame = croppingFrame;
        }
    }
}