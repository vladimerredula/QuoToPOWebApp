using PdfiumViewer;
using System.Drawing.Imaging;

namespace QouToPOWebApp.Services
{
    public class PdfiumViewerService
    {
        public List<byte[]> GenerateThumbnails(string filePath)
        {
            var thumbnails = new List<byte[]>();

            using (var pdfDocument = LoadPdf(filePath))
            {
                for (int i = 0; i < pdfDocument.PageCount; i++)
                {
                    using (var image = pdfDocument.Render(i, 100, 100, true))
                    {
                        using (var ms = new MemoryStream())
                        {
                            image.Save(ms, ImageFormat.Png);
                            thumbnails.Add(ms.ToArray());
                        }
                    }
                }
            }

            return thumbnails;
        }

        public PdfDocument LoadPdf(string filePath)
        {
            return PdfDocument.Load(filePath);
        }
    }
}
