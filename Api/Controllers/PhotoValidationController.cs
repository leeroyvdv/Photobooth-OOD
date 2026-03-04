using Microsoft.AspNetCore.Mvc;
using PhotoBooth.Application.Interfaces;

namespace PhotoBooth.Api.Controllers;

// Dit zegt tegen ASP.NET:
// "Dit bestand hoort bij de API en kan verzoeken van de website ontvangen."
[ApiController]

// Dit bepaalt de URL waar de website deze code kan bereiken.
// De website stuurt de foto naar: /api/validate-photo
[Route("api/validate-photo")]
public class PhotoValidationController : ControllerBase
{
    // Dit is de service die het echte werk doet:
    // foto controleren met AI en opslaan.
    private readonly IPhotoService _photoService;

    // Wanneer deze controller start, krijgt hij automatisch
    // de PhotoService van het systeem.
    public PhotoValidationController(IPhotoService photoService)
    {
        _photoService = photoService;
    }

    // Dit stuk code wordt uitgevoerd wanneer de website
    // een foto naar deze API stuurt.
    [HttpPost]
    public async Task<IActionResult> Validate([FromBody] PhotoValidationRequest request)
    {
        // Controle: is er wel een foto gestuurd?
        if (string.IsNullOrEmpty(request?.Base64Image))
            return BadRequest("No image received.");

        // De foto komt binnen als tekst (Base64).
        // Hier maken we daar weer echte foto-data van.
        var imageBytes = Convert.FromBase64String(request.Base64Image);

        // De foto wordt doorgestuurd naar de service.
        // Daar wordt de foto:
        // 1. gecontroleerd door AI
        // 2. opgeslagen als hij goed is
        var result = await _photoService.ValidateAndSaveAsync(
            imageBytes,
            "image/png"
        );

        // Het resultaat gaat terug naar de website.
        // Bijvoorbeeld: "foto goed" of "foto afgekeurd".
        return Ok(result);
    }
}

// Dit beschrijft welke data de website naar de API stuurt.
public class PhotoValidationRequest
{
    // Dit is de foto als Base64 tekst.
    public string? Base64Image { get; set; }
}