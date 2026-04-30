using Azure.Core;
using fut7Manager.Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace fut7Manager.Api.Controllers {
    [ApiController]
    [Route("api/uploads")]
    public class UploadsController : ControllerBase {
        private readonly IFileStorageService _fileService;
        public UploadsController(IFileStorageService fileService) {
            _fileService = fileService;
        }

        [HttpPost("image")]
        public async Task<IActionResult> UploadImage(IFormFile file, string type) {
            if (file == null || file.Length == 0)
                return BadRequest("Archivo inválido");

            var url = await _fileService.SaveImageAsync(file, type, Request);

            return Ok(new { url });
        }
    }
}
