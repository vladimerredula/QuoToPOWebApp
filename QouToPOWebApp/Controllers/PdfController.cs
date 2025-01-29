using Microsoft.AspNetCore.Mvc;
using QouToPOWebApp.Services;

namespace QouToPOWebApp.Controllers
{
    public class PdfController : Controller
    {
        private readonly PdfiumViewerService _pdfViewer;
        private readonly PdfSharpService _pdf;

        public PdfController(PdfiumViewerService pdfViewer)
        {
            _pdfViewer = pdfViewer;
            _pdf = new PdfSharpService();
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

            var thumbnails = _pdfViewer.GenerateThumbnails(filePath);

            if (pageIndex < 0 || pageIndex >= thumbnails.Count)
            {
                pageIndex = 0; // Reset to the first page if out of range
            }

            var totalPages = thumbnails.Count;
            var currentImage = $"data:image/png;base64,{Convert.ToBase64String(thumbnails[pageIndex])}";

            return Json(new
            {
                image = currentImage,
                fileName = Path.GetFileName(filePath),
                filePath = filePath,
                pageIndex = pageIndex,
                totalPages = totalPages
            });
        }
        public IActionResult GenerateTablePdf()
        {
            string[][] tableData = new string[][]
            {
            new string[] { "Header 1", "Header 2", "Header 3" },
            new string[] { "Row 1 Col 1", "Row 1 Col 2", "Row 1 Col 3" },
            new string[] { "Row 2 Col 1", "Row 2 Col 2", "Row 2 Col 3" },
            };

            byte[] pdfBytes = _pdf.CreatePdfWithTable("Table Example", tableData);

            return File(pdfBytes, "application/pdf", "TableExample.pdf");
        }
        public IActionResult GeneratePdf()
        {
            string title = "Sample PDF Document";
            string content = "This is a sample PDF generated using PDFsharp 6.1 in ASP.NET Core MVC.";
            //byte[] pdfBytes = _pdf.CreatePdf(title, content);
            byte[] pdfBytes = _pdf.CreatePo();

            // Return the PDF file as a download
            return File(pdfBytes, "application/pdf", "Sample.pdf");
        }
    }
}
