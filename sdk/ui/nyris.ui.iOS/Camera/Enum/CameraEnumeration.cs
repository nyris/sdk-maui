namespace Nyris.UI.iOS.Camera.Enum
{
    public enum TorchMode {
        On, Off, Auto
    }
    
    public enum SessionSetupResult
    {
        Success,

        NotAuthorized,

        ConfigurationFailed
    }
    
    public enum CameraAuthorizationResult
    {
        Authorized,

        Restricted,
        
        NotAuthorized,

        NotDetermined
    }
}