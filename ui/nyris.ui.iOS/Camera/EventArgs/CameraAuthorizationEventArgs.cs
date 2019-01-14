using Nyris.UI.iOS.Camera.Enum;

namespace Nyris.UI.iOS.Camera.EventArgs
{
    public class CameraAuthorizationEventArgs
    {
        public CameraAuthorizationResult Authorization;
        public CameraAuthorizationEventArgs(CameraAuthorizationResult authorization)
        {
            Authorization = authorization;
        }
    }
}