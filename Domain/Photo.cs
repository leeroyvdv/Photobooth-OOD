namespace PhotoBooth.Domain;

public class Photo
{
    public int Id { get; set; }

    public byte[] Data { get; set; } = Array.Empty<byte>();

    public string ContentType { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }
}