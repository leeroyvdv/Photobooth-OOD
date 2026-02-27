namespace PhotoBooth.Domain;

public class Photo
{
    public byte[] Data { get; set; } = Array.Empty<byte>();
    public string ContentType { get; set; } = string.Empty;
}
