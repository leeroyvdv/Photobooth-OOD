namespace PhotoBooth.Domain;

// Dit is het Photo object.
// Het stelt een foto voor die in de database wordt opgeslagen.
public class Photo
{
    // Uniek ID van de foto (wordt meestal door de database gemaakt)
    public int Id { get; private set; }

    // De foto zelf als byte data
    public byte[] Data { get; private set; }

    // Type afbeelding, bijvoorbeeld image/png of image/jpeg
    public string ContentType { get; private set; }

    // Tijd waarop de foto is gemaakt/opgeslagen
    public DateTime CreatedAt { get; private set; }

    public Photo(byte[] data, string contentType)
    {
        // Controle: er moet wel foto data zijn
        if (data == null || data.Length == 0)
            throw new ArgumentException("Photo data cannot be empty.");

        // Controle: er moet een content type zijn
        if (string.IsNullOrWhiteSpace(contentType))
            throw new ArgumentException("Content type is required.");

        // Opslaan van de foto data
        Data = data;

        // Opslaan van het type afbeelding
        ContentType = contentType;

        // Tijd instellen wanneer de foto wordt gemaakt
        CreatedAt = DateTime.UtcNow;
    }
}