namespace PhotoBooth.Domain;

public class PhotoValidationResult
{
    public bool IsValid { get; }
    public bool FaceDetected { get; }
    public bool EyesVisible { get; }
    public bool NeutralExpression { get; }
    public bool LightingOk { get; }

    public PhotoValidationResult(
        bool isValid,
        bool faceDetected,
        bool eyesVisible,
        bool neutralExpression,
        bool lightingOk)
    {
        IsValid = isValid;
        FaceDetected = faceDetected;
        EyesVisible = eyesVisible;
        NeutralExpression = neutralExpression;
        LightingOk = lightingOk;
    }
}