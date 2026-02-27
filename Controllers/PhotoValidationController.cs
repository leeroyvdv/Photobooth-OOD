using Microsoft.AspNetCore.Mvc;

namespace PhotoBooth.Controllers
{
    [ApiController]
    [Route("api/validate-photo")]
    public class PhotoValidationController : ControllerBase
    {
        [HttpPost]
        public IActionResult Validate([FromBody] PhotoValidationRequest request)
        {
            if (request?.Base64Image == null)
            {
                return BadRequest();
            }

            // Mock response (wordt momenteel niet gebruikt)
            return Ok(new
            {
                faceDetected = true,
                eyesVisible = true,
                neutralExpression = true,
                lightingOk = true
            });
        }
    }

    public class PhotoValidationRequest
    {
        public string? Base64Image { get; set; }
    }
}
