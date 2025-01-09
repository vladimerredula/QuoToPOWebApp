using Microsoft.AspNetCore.Mvc;
using QouToPOWebApp.Services;

namespace QouToPOWebApp.Controllers
{
    public class ProcessController : Controller
    {
        private readonly PdfiumViewerService _pdf;

        public ProcessController()
        {
            _pdf = new PdfiumViewerService();
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> UploadPdf(IFormFile pdfFile)
        {
            if (pdfFile == null || pdfFile.Length == 0)
            {
                return Json(new { success = false, message = "No file selected" });
            }

            try
            {
                var filePath = Path.Combine("wwwroot/uploads", pdfFile.FileName);

                // Save the file to the server
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await pdfFile.CopyToAsync(stream);
                }

                return GenerateThumbnail(filePath);
            }
            catch (System.Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
        
        [HttpPost]
        public IActionResult GenerateThumbnail(string filePath, int pageIndex = 0)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                return BadRequest("File path is required.");
            }

            var thumbnails = _pdf.GenerateThumbnails(filePath);

            if (pageIndex < 0 || pageIndex >= thumbnails.Count)
            {
                pageIndex = 0; // Reset to the first page if out of range
            }

            var totalPages = thumbnails.Count;
            var currentImage = $"data:image/png;base64,{Convert.ToBase64String(thumbnails[pageIndex])}";

            return Json( new {
                image = currentImage,
                filePath = filePath,
                pageIndex = pageIndex,
                totalPages = totalPages
            });
        }
    }
}
