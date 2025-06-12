using HtmlAgilityPack;
using PdfSharp.Pdf;
using PdfSharp.Drawing;
using System.Text.RegularExpressions;

namespace QouToPOWebApp.Helpers
{
    public class HtmlToPdfSharpRenderer
    {
        private PdfPage currentPage;
        private XGraphics gfx;
        private readonly PdfDocument document;
        private readonly double maxWidth;
        private double initialX;
        private double cursorX;
        private double cursorY;
        private readonly XFont regularFont;
        private readonly XFont boldFont;
        private readonly XFont italicFont;
        private readonly XFont bulletFont;
        private readonly double lineHeight;
        private double pageHeight;
        private double topMargin = 40;
        private double bottomMargin = 40;

        public HtmlToPdfSharpRenderer(PdfDocument document, PdfPage currentPage, XGraphics gfx, double startX, double startY, double maxWidth, XFont? baseFont = null)
        {
            this.document = document;
            this.currentPage = currentPage;
            this.gfx = gfx;
            this.initialX = startX;
            this.cursorX = startX;
            this.cursorY = startY;
            this.maxWidth = maxWidth;

            pageHeight = currentPage.Height.Point;

            var font = baseFont ?? new XFont("Arial", 12);
            regularFont = font;
            boldFont = new XFont(font.FontFamily.Name, font.Size, XFontStyleEx.Bold);
            italicFont = new XFont(font.FontFamily.Name, font.Size, XFontStyleEx.Italic);
            bulletFont = new XFont(font.FontFamily.Name, font.Size, XFontStyleEx.Regular);

            lineHeight = font.GetHeight() * 1.1;
        }

        public double RenderHtml(string html)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(html);
            RenderNodes(doc.DocumentNode.ChildNodes);
            return cursorY; // Return the new Y position after rendering
        }

        private void RenderNodes(HtmlNodeCollection nodes)
        {
            foreach (var node in nodes)
            {
                switch (node.Name)
                {
                    case "#text":
                        RenderText(node.InnerText, regularFont);
                        break;
                    case "b":
                    case "strong":
                        RenderText(node.InnerText, boldFont);
                        break;
                    case "i":
                    case "em":
                        RenderText(node.InnerText, italicFont);
                        break;
                    case "br":
                        cursorY += lineHeight;
                        cursorX = initialX;
                        EnsureSpaceForLine();
                        break;
                    case "p":
                        RenderNodes(node.ChildNodes);
                        cursorY += lineHeight;
                        cursorX = initialX;
                        EnsureSpaceForLine();
                        break;
                        break;
                    case "ul":
                        RenderList(node);
                        break;
                    case "ol":
                        RenderList(node);
                        break;
                    default:
                        RenderNodes(node.ChildNodes); // Recursively handle unknown tags
                        break;
                }
            }
        }

        private void RenderText(string text, XFont font)
        {
            if (string.IsNullOrWhiteSpace(text)) return;

            string[] words = Regex.Split(text, @"(\s+)");
            string line = "";
            double spaceLeft = maxWidth - cursorX;

            foreach (var word in words)
            {
                string testLine = line + word;
                var size = gfx.MeasureString(testLine, font);

                if (size.Width > spaceLeft)
                {
                    // Draw the current line
                    EnsureSpaceForLine();
                    gfx.DrawString(line.TrimEnd(), font, XBrushes.Black, new XPoint(cursorX, cursorY));
                    cursorY += lineHeight;
                    cursorX = initialX;
                    line = word.TrimStart();
                    spaceLeft = maxWidth - cursorX;
                }
                else
                {
                    line = testLine;
                }
            }

            if (!string.IsNullOrWhiteSpace(line))
            {
                EnsureSpaceForLine();
                gfx.DrawString(line.TrimEnd(), font, XBrushes.Black, new XPoint(cursorX, cursorY));
                cursorX += gfx.MeasureString(line, font).Width;
            }
        }

        private void RenderList(HtmlNode listNode)
        {
            int index = 1;

            foreach (var li in listNode.Elements("li"))
            {
                string listType = li.GetAttributeValue("data-list", "bullet").Trim().ToLower();

                string bullet = listType == "ordered" ? $"{index++}." : "•";

                // Draw the bullet
                gfx.DrawString(bullet, bulletFont, XBrushes.Black, new XPoint(cursorX, cursorY));

                // Indent for the list item content
                double indentX = gfx.MeasureString("• ", bulletFont).Width + 5;
                double contentX = cursorX + indentX;
                double contentMaxWidth = maxWidth - indentX;

                // Render the inner content of <li>
                var innerRenderer = new HtmlToPdfSharpRenderer(document, currentPage, gfx, contentX, cursorY, contentMaxWidth, regularFont);
                double renderedY = innerRenderer.RenderHtml(li.InnerHtml);

                // Move to the line after the rendered content
                cursorY = renderedY + lineHeight;
                cursorX = initialX;
            }
        }

        private void EnsureSpaceForLine()
        {
            if (cursorY + lineHeight > pageHeight - bottomMargin - lineHeight)
            {
                currentPage = document.AddPage();
                gfx = XGraphics.FromPdfPage(currentPage);
                DrawFooter(gfx, currentPage);
                cursorY = topMargin;
                cursorX = initialX;
                pageHeight = currentPage.Height.Point;
            }
        }

        private void DrawFooter(XGraphics gfx, PdfPage page)
        {
            XColor ClayCreek = XColor.FromCmyk(0.00, 0.06, 0.36, 0.42);
            double margin = 65;
            double y = page.Height - margin;
            double x = 36;

            XPen customDashedPen = new XPen(ClayCreek, 1.3);
            customDashedPen.DashPattern = new double[] { 8, 3, 0.8, 3, 0.8, 3 };
            gfx.DrawLine(customDashedPen, x, y, page.Width - x, y);
            gfx.DrawLine(new XPen(ClayCreek, 2), x, y + 5, page.Width - x, y + 5);
        }

        public PdfPage GetCurrentPage() => currentPage;
        public XGraphics GetCurrentGraphics() => gfx;
    }
}
