using CoreGraphics;

namespace Nyris.Ui.iOS.Camera.EventArgs
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