using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using System.IO;

namespace CityInfo.API.Controllers
{
    [Route("api/files")]
    [Authorize]
    [ApiController]
    public class FilesController : ControllerBase
    {
        private readonly FileExtensionContentTypeProvider _fileExtensionContentTypeProvider;
        public FilesController(FileExtensionContentTypeProvider fileExtensionContentTypeProvider)
        {
               _fileExtensionContentTypeProvider = fileExtensionContentTypeProvider
                ?? throw new ArgumentNullException(nameof(fileExtensionContentTypeProvider));
        }

        [HttpGet("{fileId}")]
        public IActionResult GetFile(string fileId)
        {
            var pathToFile ="creating-the-api-and-returning-resources-slides.pdf";

            if (!System.IO.File.Exists(pathToFile))
            {
                return NotFound();
            }

            if(!_fileExtensionContentTypeProvider.TryGetContentType(
                pathToFile, out var contentType))
            {
                contentType = "application/octet-stream";
            }

            var bytes = System.IO.File.ReadAllBytes(pathToFile);

            return File(bytes, contentType, Path.GetFileName(pathToFile));
        }
    }
}
