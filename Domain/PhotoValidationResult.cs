namespace PhotoBooth.Domain;

// Dit object bevat het resultaat van de foto controle
public class PhotoValidationResult
{
    // Geeft aan of de foto uiteindelijk geldig is
    public bool IsValid { get; }

    // Of er een gezicht op de foto is gevonden
    public bool FaceDetected { get; }

    // Of de ogen goed zichtbaar zijn
    public bool EyesVisible { get; }

    // Of de gezichtsuitdrukking neutraal is
    public bool NeutralExpression { get; }

    // Of de belichting van de foto goed is
    public bool LightingOk { get; }

    public PhotoValidationResult(
        bool isValid,
        bool faceDetected,
        bool eyesVisible,
        bool neutralExpression,
        bool lightingOk)
    {
        // Resultaten van de controles opslaan
        IsValid = isValid;
        FaceDetected = faceDetected;
        EyesVisible = eyesVisible;
        NeutralExpression = neutralExpression;
        LightingOk = lightingOk;
    }
}