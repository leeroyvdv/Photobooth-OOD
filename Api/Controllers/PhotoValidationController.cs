using Microsoft.AspNetCore.Mvc;
using PhotoBooth.Application.Interfaces;

namespace PhotoBooth.Api.Controllers;

[ApiController]
[Route("api/validate-photo")]
public class PhotoValidationController : ControllerBase
{
    private readonly IPhotoService _photoService;

    public PhotoValidationController(IPhotoService photoService)
    {
        _photoService = photoService;
    }

    [HttpPost]
    public async Task<IActionResult> Validate([FromBody] PhotoValidationRequest request)
    {
        if (string.IsNullOrEmpty(request?.Base64Image))
            return BadRequest("No image received.");

        var imageBytes = Convert.FromBase64String(request.Base64Image);

        var result = await _photoService.ValidateAndSaveAsync(
            imageBytes,
            "image/png"
        );

        return Ok(result);
    }
}

public class PhotoValidationRequest
{
    public string? Base64Image { get; set; }
}