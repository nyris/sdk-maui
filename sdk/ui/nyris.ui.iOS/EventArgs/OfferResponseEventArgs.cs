using System;
using CoreGraphics;
using Nyris.UI.Common;
using Nyris.UI.iOS.Models;
using UIKit;

namespace Nyris.UI.iOS.EventArgs
{
    public class OfferResponseEventArgs : System.EventArgs
    {
        public readonly NyrisSearcherResult? Results;

        public UIImage Screenshot;

        public CGRect CroppingFrame;

        public OfferResponseEventArgs(UIImage screenshot, CGRect croppingFrame, NyrisSearcherResult? offerResponse)
        {
            Results = offerResponse;
            Screenshot = screenshot;
            CroppingFrame = croppingFrame;
        }
    }
}