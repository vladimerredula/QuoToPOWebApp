using PdfiumViewer;
using System.Drawing.Imaging;

namespace QouToPOWebApp.Services
{

    public interface IPdfViewerService
    {
        List<byte[]> GenerateThumbnails(string filePath);
    }

    public class PdfiumViewerService : IPdfViewerService
    {
        public List<byte[]> GenerateThumbnails(string filePath)
        {
            var thumbnails = new List<byte[]>();

            using (var pdfDocument = PdfDocument.Load(filePath))
            {
                for (int i = 0; i < pdfDocument.PageCount; i++)
                {
                    using (var image = pdfDocument.Render(i, 200, 200, true))
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
    }
}
