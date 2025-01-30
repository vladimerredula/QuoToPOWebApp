using System.Drawing;
using Tesseract;
using PdfiumViewer;
using Microsoft.AspNetCore.Mvc;
using static System.Net.Mime.MediaTypeNames;
using Image = System.Drawing.Image;
using System.Drawing.Imaging;

namespace QouToPOWebApp.Services
{
    public class TesseractService : IDisposable
    {
        private readonly TesseractEngine _engine;
        private readonly PdfiumViewerService _pdf;

        public TesseractService(PdfiumViewerService pdfium)
        {
            _engine = new TesseractEngine(@"TessData", "eng+jpn", EngineMode.TesseractAndLstm);
            _pdf = pdfium;
        }

        public string ExtractTextFromImage(string imagePath)
        {
            if (!File.Exists(imagePath))
            {
                throw new FileNotFoundException("Image file not found.", imagePath);
            }

            using (var img = Pix.LoadFromFile(imagePath))
            {
                using (var page = _engine.Process(img))
                {
                    return page.GetText().Trim();
                }
            }
        }

        public byte[] sample(string pdfPath)
        {
            List<Image> images = ExtractImagesFromPdf1(pdfPath); // Extract images from PDF

            var image = images[0];

            // rescale image 4x biffer for better recognition
            //image = RescaleImage((Bitmap)image, 2.0f);

            // process to grayscale and binarize image
            image = ConvertToGrayscale((Bitmap)image);
            //image = ConvertToBlackAndWhite((Bitmap)image);

            // Create a MemoryStream to save the bitmap image as a byte array
            using (var stream = new System.IO.MemoryStream())
            {
                // Save the bitmap as PNG format (Tesseract works well with PNG format)
                image.Save(stream, System.Drawing.Imaging.ImageFormat.Png);

                // Use Tesseract's Pix.LoadFromMemory to convert the image
                return stream.ToArray();
            }
        }

        // Extract text from a scanned PDF
        public string ExtractTextFromPdf(string pdfPath)
        {
            List<Bitmap> images = ExtractImagesFromPdf(pdfPath); // Extract images from PDF

            string extractedText = string.Empty;
            foreach (var image in images)
            {
                //var grayscaleImage = ConvertToGrayscale(image);
                extractedText += ExtractTextFromImage(image); // Extract text using Tesseract from each image
            }

            return extractedText;
        }

        // Extract images from the PDF using Pdfium
        private List<Bitmap> ExtractImagesFromPdf(string pdfPath)
        {
            var images = new List<Bitmap>();

            // Ensure the file exists
            if (!File.Exists(pdfPath))
            {
                throw new FileNotFoundException("The specified PDF file does not exist.");
            }

            using (var pdfDocument = _pdf.LoadPdf(pdfPath))
            {
                // Loop through each page in the PDF
                for (int pageIndex = 0; pageIndex < pdfDocument.PageCount; pageIndex++)
                {
                    var pageBitmap = RenderPageAsImage(pdfDocument, pageIndex);
                    images.Add(pageBitmap); // Add the rendered image to the list
                }
            }

            return images;
        }

        // Extract images from the PDF using Pdfium
        private List<Image> ExtractImagesFromPdf1(string pdfPath)
        {
            var images = new List<Image>();

            // Ensure the file exists
            if (!File.Exists(pdfPath))
            {
                throw new FileNotFoundException("The specified PDF file does not exist.");
            }

            using (var pdfDocument = _pdf.LoadPdf(pdfPath))
            {
                // Loop through each page in the PDF
                for (int pageIndex = 0; pageIndex < pdfDocument.PageCount; pageIndex++)
                {
                    var pageBitmap = RenderPageAsImage1(pdfDocument, pageIndex);
                    images.Add(pageBitmap); // Add the rendered image to the list
                }
            }

            return images;
        }

        // Render a page of the PDF as an image
        private Bitmap RenderPageAsImage(PdfDocument pdfDocument, int pageIndex)
        {
            // Render the PDF page as an image (600 DPI)
            const float dpi = 1200;
            var pageSize = pdfDocument.PageSizes[pageIndex];
            var pageBitmap = pdfDocument.Render(pageIndex, dpi, dpi, true);
            //var pageBitmap = pdfDocument.Render(pageIndex, (int)pageSize.Width, (int)pageSize.Height, dpi, dpi, true);

            return (Bitmap)pageBitmap;
        }

        // Render a page of the PDF as an image
        private Image RenderPageAsImage1(PdfDocument pdfDocument, int pageIndex)
        {
            // Render the PDF page as an image (600 DPI)
            const float dpi = 1200;
            var pageSize = pdfDocument.PageSizes[pageIndex];
            var pageBitmap = pdfDocument.Render(pageIndex, dpi, dpi, true);
            //var pageBitmap = pdfDocument.Render(pageIndex, (int)pageSize.Width, (int)pageSize.Height, dpi, dpi, true);

            return pageBitmap;
        }

        public string ExtractTextFromImage(Bitmap image)
        {
            // rescale image 4x biffer for better recognition
            var processedImg = RescaleImage(image, 2.0f);

            // process to grayscale and binarize image
            //processedImg = ConvertToGrayscale(processedImg);

            //_engine.SetVariable("textord_min_linesize", "1");  // Handle small text better
            //_engine.SetVariable("tessedit_char_whitelist", "0123456789abcdefghijklmnopqrstuvwxyz");
            //_engine.SetVariable("textord_force_make_prop_words", "0");
            //_engine.SetVariable("textord_noise_normratio", "0");

            using (var pixImage = BitmapToPix(processedImg))
            {
                using (var page = _engine.Process(pixImage))
                {
                    return page.GetText();
                }
            }
        }

        // Helper method to convert Bitmap to Pix for Tesseract
        private Pix BitmapToPix(Bitmap image)
        {
            // Create a MemoryStream to save the bitmap image as a byte array
            using (var stream = new System.IO.MemoryStream())
            {
                // Save the bitmap as PNG format (Tesseract works well with PNG format)
                image.Save(stream, System.Drawing.Imaging.ImageFormat.Png);

                // Reset the stream position to the beginning
                stream.Position = 0;

                // Use Tesseract's Pix.LoadFromMemory to convert the image
                return Pix.LoadFromMemory(stream.ToArray());
            }
        }

        // Helper method to grayscale and binarize image
        public Bitmap PreprocessImage(Bitmap originalImage, int threshold = 128)
        {
            Bitmap grayscaleImage = new Bitmap(originalImage.Width, originalImage.Height);
            Bitmap binarizedImage = new Bitmap(originalImage.Width, originalImage.Height);

            // Convert the image to Grayscale
            for (int y = 0; y < originalImage.Height; y++)
            {
                for (int x = 0; x < originalImage.Width; x++)
                {
                    // Get the pixel color
                    Color pixelColor = originalImage.GetPixel(x, y);

                    // Convert to Grayscale (average of RGB values)
                    int grayValue = (int)((pixelColor.R * 0.3) + (pixelColor.G * 0.59) + (pixelColor.B * 0.11));
                    Color grayColor = Color.FromArgb(grayValue, grayValue, grayValue);

                    grayscaleImage.SetPixel(x, y, grayColor);

                    // Apply Threshold (Binarization)
                    if (grayValue > threshold)
                    {
                        binarizedImage.SetPixel(x, y, Color.White);
                    }
                    else
                    {
                        binarizedImage.SetPixel(x, y, Color.Black);
                    }
                }
            }

            // Return the binarized image
            return binarizedImage;
        }

        public Bitmap ConvertToGrayscale(Bitmap image)
        {
            Bitmap grayscale = new Bitmap(image.Width, image.Height);
            using (Graphics g = Graphics.FromImage(grayscale))
            {
                // Use a ColorMatrix to remove color
                ColorMatrix colorMatrix = new ColorMatrix(new float[][]
                {
                new float[] {0.3f, 0.3f, 0.3f, 0, 0},
                new float[] {0.59f, 0.59f, 0.59f, 0, 0},
                new float[] {0.11f, 0.11f, 0.11f, 0, 0},
                new float[] {0, 0, 0, 1, 0},
                new float[] {0, 0, 0, 0, 1}
                });

                ImageAttributes attributes = new ImageAttributes();
                attributes.SetColorMatrix(colorMatrix);

                g.DrawImage(image, new Rectangle(0, 0, image.Width, image.Height),
                            0, 0, image.Width, image.Height, GraphicsUnit.Pixel, attributes);
            }
            return grayscale;
        }
        
        public Bitmap ConvertToBlackAndWhite(Bitmap image, int threshold = 128)
        {
            Bitmap binarized = new Bitmap(image.Width, image.Height);

            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    Color pixel = image.GetPixel(x, y);
                    int gray = (pixel.R + pixel.G + pixel.B) / 3; // Convert to grayscale

                    Color newColor = (gray < threshold) ? Color.Black : Color.White;
                    binarized.SetPixel(x, y, newColor);
                }
            }
            return binarized;
        }

        // Helper method to rescale image
        private Bitmap RescaleImage(Bitmap image, float scaleFactor)
        {
            int newWidth = (int)(image.Width * scaleFactor);
            int newHeight = (int)(image.Height * scaleFactor);
            Bitmap resizedImage = new Bitmap(newWidth, newHeight);

            using (Graphics g = Graphics.FromImage(resizedImage))
            {
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.DrawImage(image, 0, 0, newWidth, newHeight);
            }

            return resizedImage;
        }

        public void Dispose()
        {
            _engine.Dispose();
        }
    }
}
