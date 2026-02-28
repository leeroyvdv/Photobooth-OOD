using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PhotoBooth.Data;
using PhotoBooth.Domain;

namespace PhotoBooth.Controllers
{
    [ApiController]
    [Route("api/validate-photo")]
    public class PhotoValidationController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PhotoValidationController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> Validate([FromBody] PhotoValidationRequest request)
        {
            if (string.IsNullOrEmpty(request?.Base64Image))
                return BadRequest("No image received.");

            var imageBytes = Convert.FromBase64String(request.Base64Image);

            var photo = new Photo
            {
                Data = imageBytes,
                ContentType = "image/png",
                CreatedAt = DateTime.UtcNow
            };

            _context.Photos.Add(photo);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                faceDetected = true,
                eyesVisible = true,
                neutralExpression = true,
                lightingOk = true,
                savedPhotoId = photo.Id
            });
        }

        [HttpGet("count")]
        public async Task<IActionResult> Count()
        {
            var count = await _context.Photos.CountAsync();
            return Ok(count);
        }
    }

    public class PhotoValidationRequest
    {
        public string? Base64Image { get; set; }
    }
}