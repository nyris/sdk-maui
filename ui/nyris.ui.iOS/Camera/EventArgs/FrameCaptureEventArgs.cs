using CoreGraphics;
using CoreMedia;
using CoreVideo;
using UIKit;

namespace Nyris.UI.iOS.Camera.EventArgs
{
    public class FrameCaptureEventArgs : System.EventArgs
    {
        public CVImageBuffer FrameBuffer;
        public FrameCaptureEventArgs(CVImageBuffer frameBuffer)
        {
            this.FrameBuffer = frameBuffer;
        }
    }
}