using Tabula;
using Tabula.Detectors;
using Tabula.Extractors;
using UglyToad.PdfPig;

namespace QouToPOWebApp.Services
{
    public class TabulaService
    {
        public TabulaService()
        {
        }

        public List<string> StreamModeExtraction(string pdfPath)
        {
            List<string> extractedText = new List<string>();

            using (var stream = new FileStream(pdfPath, FileMode.Open, FileAccess.Read))
            {
                using (var pdoc = PdfDocument.Open(stream, new ParsingOptions { SkipMissingFonts = true, UseLenientParsing = true }))
                {
                    SimpleNurminenDetectionAlgorithm da = new SimpleNurminenDetectionAlgorithm();
                    IExtractionAlgorithm ea = new BasicExtractionAlgorithm();

                    for (int i = 1; i <= pdoc.NumberOfPages; i++)
                    {
                        var area = ObjectExtractor.Extract(pdoc, i);
                        var pages = da.Detect(area);

                        //List<Table> tables = (List<Table>)ea.Extract(area.GetArea(page[0].BoundingBox)); // take first candidate area

                        //foreach (var table in tables)
                        //{
                        //    extractedText.Add(TableToString(table));
                        //}

                        if (pages.Count == 0) // No areas detected
                        {
                            Console.WriteLine($"No tables detected on page {i}");
                            continue;
                        }

                        foreach (var detectedPage in pages)
                        {
                            // Extract tables within the bounding box of the detected area
                            var tables = ea.Extract(area.GetArea(detectedPage.BoundingBox)) as List<Table>;

                            if (tables != null && tables.Count > 0)
                            {
                                foreach (var table in tables)
                                {
                                    extractedText.Add(TableToString(table));
                                }
                            }
                            else
                            {
                                Console.WriteLine($"No tables found in detected area on page {i}");
                            }
                        }
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
                using (var pdoc = PdfDocument.Open(pdfStream, new ParsingOptions { SkipMissingFonts = true, UseLenientParsing = true }))
                {
                    // Create an ObjectExtractor
                    var area = ObjectExtractor.ExtractPage(pdoc, 1);

                    // Extract tables from the page
                    var tableExtractor = new SpreadsheetExtractionAlgorithm();

                    var tables = tableExtractor.Extract(area);

                    if (tables.Count != 0)
                    {
                        foreach (var table in tables)
                        {
                            // Convert table to text (you can customize formatting)
                            extractedText.Add(TableToString(table));
                        }
                    }
                }
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
