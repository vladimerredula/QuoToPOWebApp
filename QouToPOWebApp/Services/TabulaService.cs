using Tabula;
using Tabula.Detectors;
using Tabula.Extractors;
using UglyToad.PdfPig;

namespace QouToPOWebApp.Services
{
    public class TabulaService
    {
        private readonly PdfPigService _pdfPig;

        public TabulaService(PdfPigService pdf)
        {
            _pdfPig = pdf;
        }

        public List<string> StreamModeExtraction(string pdfPath)
        {
            List<string> extractedText = new List<string>();

            using (var stream = new FileStream(pdfPath, FileMode.Open, FileAccess.Read))
            {
                var pdoc = _pdfPig.Open(stream, new ParsingOptions { SkipMissingFonts = true, UseLenientParsing = true });

                SimpleNurminenDetectionAlgorithm da = new SimpleNurminenDetectionAlgorithm();
                IExtractionAlgorithm ea = new BasicExtractionAlgorithm();

                for (int i = 1; i <= pdoc.NumberOfPages; i++)
                {
                    var area = ObjectExtractor.Extract(pdoc, i);
                    var page = da.Detect(area);

                    List<Table> tables = (List<Table>)ea.Extract(area.GetArea(page[0].BoundingBox)); // take first candidate area

                    foreach (var table in tables)
                    {
                        extractedText.Add(TableToString(table));
                    }
                }

            }

            return extractedText;
        }

        public List<string> LatticeModeExtraction(string pdfPath)
        {
            List<string> extractedText = new List<string>();

            if (!File.Exists(pdfPath))
            {
                throw new FileNotFoundException("The specified PDF file does not exist.");
            }

            // Open the PDF file
            using (var pdfStream = new FileStream(pdfPath, FileMode.Open, FileAccess.Read))
            {
                var pdoc = PdfDocument.Open(pdfStream, new ParsingOptions { SkipMissingFonts = true, UseLenientParsing = true });
                
                // Create an ObjectExtractor
                var area = ObjectExtractor.ExtractPage(pdoc, 1);

                // Extract tables from the page
                var tableExtractor = new SpreadsheetExtractionAlgorithm();

                var tables = tableExtractor.Extract(area);

                // Convert table to text (you can customize formatting)
                extractedText.Add(TableToString(tables[0]));
            }

            return extractedText;
        }

        // Helper function to convert table data to a readable string format
        private string TableToString(Table table)
        {
            var tableText = string.Empty;

            foreach (var row in table.Rows)
            {
                tableText += string.Join("\t", row) + "\n";
            }

            return tableText;
        }
    }
}
