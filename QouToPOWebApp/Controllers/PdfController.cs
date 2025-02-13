using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QouToPOWebApp.Services;

namespace QouToPOWebApp.Controllers
{
    [Authorize]
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
            byte[] pdfBytes = _pdf.CreatePo(new ViewModel.PoViewModel());

            // Return the PDF file as a download
            return File(pdfBytes, "application/pdf", "Sample.pdf");
        }

        public IActionResult Sample()
        {
            var file = "C:\\Users\\v.redula\\source\\repos\\QuoToPOWebApp\\QouToPOWebApp\\wwwroot\\uploads\\20240426 Q+PO AMS Korea MgO pellet.pdf";
            var file2 = "C:\\Users\\v.redula\\source\\repos\\QuoToPOWebApp\\QouToPOWebApp\\wwwroot\\uploads\\20241010 Toshima Ag recycle Q No.24100926SO05.pdf";
            var file3 = "C:\\Users\\v.redula\\source\\repos\\QuoToPOWebApp\\QouToPOWebApp\\wwwroot\\uploads\\NSK製 U-6904-H-20S8FDZZ GVSL 300個 2024-k056 (1).pdf";
            var file4 = "C:\\Users\\v.redula\\source\\repos\\QuoToPOWebApp\\QouToPOWebApp\\wwwroot\\uploads\\20240422 PO ASE Net.pdf";
            var file5 = "\\\\192.168.161.10\\Personal\\0062.Vladimer Redula\\Weekly reports\\2025\\Week4.pdf";
            var pig = new PdfPigService();
            byte[] pdfBytes = pig.SampleExtraction(file5, 1);

            // Return the PDF file as a download
            return File(pdfBytes, "application/pdf", "Sample.pdf");
        }

        public IActionResult samples()
        {
            var file = "C:\\Users\\v.redula\\source\\repos\\QuoToPOWebApp\\QouToPOWebApp\\wwwroot\\uploads\\1.pdf";

            var pig = new TesseractService(new PdfiumViewerService());
            byte[] pdfBytes = pig.sample(file);

            // Return the PDF file as a download
            return File(pdfBytes, "application/pdf", "Sample.png");
        }
    }
}
