namespace PhotoBooth.Domain;

public class PhotoValidationResult
{
    public bool IsValid { get; set; }
    public string Message { get; set; } = string.Empty;
}
