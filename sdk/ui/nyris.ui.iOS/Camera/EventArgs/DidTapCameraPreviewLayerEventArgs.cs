using CoreGraphics;

namespace Nyris.UI.iOS.Camera.EventArgs
{
    public class DidTapCameraPreviewLayerEventArgs : System.EventArgs
    {
        public CGPoint TouchLocation;
        public DidTapCameraPreviewLayerEventArgs(CGPoint touchLocation)
        {
            this.TouchLocation = touchLocation;
        }
        
    }
}