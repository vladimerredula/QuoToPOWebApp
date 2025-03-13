using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QouToPOWebApp.Models.MiscModels;
using QouToPOWebApp.Services;
using System.Security.Claims;

namespace QouToPOWebApp.Controllers
{
    [Authorize]
    public class PdfController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly PdfSharpService _pdf;
        
        public PdfController(ApplicationDbContext db)
        {
            _db = db;
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

                //return GenerateThumbnail(filePath);
                return GetPdfData(filePath);
            }
            catch (System.Exception ex)
            {
                Console.WriteLine("Error uploading pdf file: " + ex.Message);
                return Json(new { success = false, message = ex.Message });
            }
        }

        /*[HttpPost]
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
        }*/

        [HttpPost]
        public IActionResult GetPdfData(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                return BadRequest("File path is required.");
            }

            byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);
            var currentImage = $"data:application/pdf;base64,{Convert.ToBase64String(fileBytes)}";

            return Json(new
            {
                image = currentImage,
                fileName = Path.GetFileName(filePath),
                filePath
            });
        }

        public IActionResult GenerateTablePdf()
        {
            string[][] tableData =
            [
                ["Header 1", "Header 2", "Header 3"],
                ["Row 1 Col 1", "Row 1 Col 2", "Row 1 Col 3"],
                ["Row 2 Col 1", "Row 2 Col 2", "Row 2 Col 3"],
            ];

            byte[] pdfBytes = _pdf.CreatePdfWithTable("Table Example", tableData);

            return File(pdfBytes, "application/pdf", "TableExample.pdf");
        }

        public IActionResult GeneratePdf()
        {
            byte[] pdfBytes = _pdf.SamplePo();

            // Return the PDF file as a download
            return File(pdfBytes, "application/pdf", "Sample.pdf");
        }

        public IActionResult GeneratePdfEng()
        {
            byte[] pdfBytes = _pdf.SamplePoEng();

            // Return the PDF file as a download
            return File(pdfBytes, "application/pdf", "Sample.pdf");
        }

        public IActionResult Sample()
        {
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

        [HttpPost]
        public IActionResult UploadQuotation(IFormFile quoFile)
        {
            if (quoFile == null || quoFile.Length == 0)
            {
                return Json(new { success = false, message = "No file selected" });
            }

            try
            {
                var tempDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");

                // Ensure the Temp directory exists
                if (!Directory.Exists(tempDir))
                {
                    Directory.CreateDirectory(tempDir);
                }

                var personnelId = GetPersonnelID();

                string filePath = Path.Combine(tempDir, quoFile.FileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    quoFile.CopyTo(stream);
                }

                // Ensure file exists before saving to Attachment
                if (System.IO.File.Exists(filePath))
                {
                    var file = new Attachment
                    {
                        User_ID = GetPersonnelID(),
                        File_name = quoFile.FileName,
                        File_path = filePath,
                        Attachment_type = "quotation",
                        Date_uploaded = DateTime.Now,
                        Status = "pending"
                    };

                    _db.Attachments.Add(file);
                    _db.SaveChanges();
                }

                return Json(new { 
                    success = true, 
                    message = "File uploaded successfully.",
                    fileName = quoFile.FileName,
                    filePath = $"/uploads/{quoFile.FileName}"
                });
            }
            catch (System.Exception ex)
            {
                Console.WriteLine("Error uploading pdf file: " + ex.Message);
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public IActionResult GetQuoFile(string fileName)
        {
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "AppData/Temp", fileName);

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound("File not found.");
            }


            return File(System.IO.File.OpenRead(filePath), "application/pdf");
        }

        public int GetPersonnelID()
        {
            var personnelId = int.Parse(User.FindFirstValue("Personnelid") ?? "0");

            return personnelId;
        }
    }
}
