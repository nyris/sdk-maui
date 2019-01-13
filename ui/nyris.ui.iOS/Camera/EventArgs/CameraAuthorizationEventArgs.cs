using Nyris.UI.iOS.Camera.Enum;

namespace Nyris.UI.iOS.Camera.EventArgs
{
    public class CameraAuthorizationEventArgs
    {
        private CameraAuthorizationResult _authorization;
        public CameraAuthorizationEventArgs(CameraAuthorizationResult authorization)
        {
            _authorization = authorization;
        }
    }
}