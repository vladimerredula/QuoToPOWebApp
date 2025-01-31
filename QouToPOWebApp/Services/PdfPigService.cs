using UglyToad.PdfPig;
using UglyToad.PdfPig.DocumentLayoutAnalysis.PageSegmenter;
using UglyToad.PdfPig.DocumentLayoutAnalysis.ReadingOrderDetector;
using UglyToad.PdfPig.DocumentLayoutAnalysis.WordExtractor;
using UglyToad.PdfPig.Fonts.Standard14Fonts;
using UglyToad.PdfPig.Writer;

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

        public byte[] SampleExtraction(string filePath, int pageNumber)
        {
            using (var document = PdfDocument.Open(filePath))
            {
                var builder = new PdfDocumentBuilder { };
                PdfDocumentBuilder.AddedFont font = builder.AddStandard14Font(Standard14Font.Helvetica);
                var pageBuilder = builder.AddPage(document, pageNumber);
                pageBuilder.SetStrokeColor(0, 255, 0);
                var page = document.GetPage(pageNumber);

                var letters = page.Letters; // no preprocessing

                // 1. Extract words
                var wordExtractor = NearestNeighbourWordExtractor.Instance;

                var words = wordExtractor.GetWords(letters);
                foreach (var item in words)
                {
                    Console.Write(item);
                }

                // 2. Segment page
                var pageSegmenter = DocstrumBoundingBoxes.Instance;

                var textBlocks = pageSegmenter.GetBlocks(words);

                // 3. Postprocessing
                var readingOrder = UnsupervisedReadingOrderDetector.Instance;
                var orderedTextBlocks = readingOrder.Get(textBlocks);

                // 4. Add debug info - Bounding boxes and reading order
                foreach (var block in orderedTextBlocks)
                {
                    var bbox = block.BoundingBox;
                    pageBuilder.DrawRectangle(bbox.BottomLeft, bbox.Width, bbox.Height);
                    pageBuilder.AddText(block.ReadingOrder.ToString(), 8, bbox.TopLeft, font);
                }

                // 5. Write result to a file
                return builder.Build();
            }
        }
    }
}
