namespace PhotoBooth.Domain;

public class Photo
{
    public int Id { get; private set; }

    public byte[] Data { get; private set; }

    public string ContentType { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public Photo(byte[] data, string contentType)
    {
        if (data == null || data.Length == 0)
            throw new ArgumentException("Photo data cannot be empty.");

        if (string.IsNullOrWhiteSpace(contentType))
            throw new ArgumentException("Content type is required.");

        Data = data;
        ContentType = contentType;
        CreatedAt = DateTime.UtcNow;
    }
}