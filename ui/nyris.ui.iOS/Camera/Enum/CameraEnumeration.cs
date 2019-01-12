namespace Nyris.Ui.iOS.Camera.Enum
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

        NotAuthorized,

        NotDetermined
    }
}