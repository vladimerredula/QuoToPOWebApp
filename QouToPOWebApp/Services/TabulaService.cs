using Tabula;
using Tabula.Detectors;
using Tabula.Extractors;
using UglyToad.PdfPig;

namespace QouToPOWebApp.Services
{
    public class TabulaService
    {
        public void ExtractTables(string pdfPath)
        {
            var extractedTables = new List<List<string>>();

            using (var stream = new FileStream(pdfPath, FileMode.Open, FileAccess.Read))
            {
                var pdoc = PdfDocument.Open(stream, new ParsingOptions { SkipMissingFonts = true, UseLenientParsing = true });
                PageArea area = ObjectExtractor.ExtractPage(pdoc, 1);

                SimpleNurminenDetectionAlgorithm da = new SimpleNurminenDetectionAlgorithm();
                var sample = da.Detect(area);

                IExtractionAlgorithm ea = new BasicExtractionAlgorithm();
                List<Table> tables = (List<Table>)ea.Extract(area.GetArea(sample[0].BoundingBox)); // take first candidate area
                var table = tables[0];
                var rows = table.Rows;
            }
        }
    }
}
