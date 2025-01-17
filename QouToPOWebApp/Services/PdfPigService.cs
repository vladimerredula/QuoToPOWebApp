using UglyToad.PdfPig;

namespace QouToPOWebApp.Services
{
    public class PdfPigService
    {
        public List<string> ExtractText(string pdfPath)
        {
            var extractedText = new List<string>();

            using (var document = Open(pdfPath))
            {
                foreach (var page in document.GetPages())
                {
                    extractedText.Add(page.Text);
                }
            }

            return extractedText;
        }

        public PdfDocument Open(byte[] fileBytes, ParsingOptions? options = null)
        {
            return PdfDocument.Open(fileBytes, options);
        }

        public PdfDocument Open(string pdfPath, ParsingOptions? options = null)
        {
            return PdfDocument.Open(pdfPath, options);
        }

        public PdfDocument Open(Stream stream, ParsingOptions? options = null)
        {
            return PdfDocument.Open(stream, options);
        }
    }
}
