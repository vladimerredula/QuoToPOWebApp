using UglyToad.PdfPig;

namespace QouToPOWebApp.Services
{
    public class PdfPigService
    {
        public List<string> ExtractText(string pdfPath)
        {
            var extractedText = new List<string>();

            using (var document = PdfDocument.Open(pdfPath))
            {
                foreach (var page in document.GetPages())
                {
                    extractedText.Add(page.Text);
                }
            }

            return extractedText;
        }
    }
}
