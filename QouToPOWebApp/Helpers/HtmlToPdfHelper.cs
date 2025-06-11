using PdfSharp.Drawing;
using PdfSharp.Pdf;

namespace QouToPOWebApp.Helpers
{
    public class HtmlToPdfHelper
    {
        public static (PdfPage updatedPage, XGraphics updatedGfx, double updatedY) RenderHtmlOnPages(
            PdfDocument document,
            PdfPage currentPage,
            XGraphics gfx,
            string html,
            double startX,
            double startY,
            double maxWidth,
            XFont? baseFont = null)
        {
            var renderer = new HtmlToPdfSharpRenderer(document, currentPage, gfx, startX, startY, maxWidth, baseFont);
            double finalY = renderer.RenderHtml(html);

            // Return updated state for further drawing
            return (renderer.GetCurrentPage(), renderer.GetCurrentGraphics(), finalY);
        }
    }
}
