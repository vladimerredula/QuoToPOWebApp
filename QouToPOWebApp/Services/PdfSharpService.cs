using PdfSharp.Drawing;
using PdfSharp.Fonts;
using PdfSharp.Pdf;
using System.Globalization;

namespace QouToPOWebApp.Services
{
    public class PdfSharpService
    {
        public byte[] CreatePdf(string title, string content)
        {
            // Create a new PDF document
            var document = new PdfDocument();
            document.Info.Title = title;

            // Create an empty page
            PdfPage page = document.AddPage();
            XGraphics gfx = XGraphics.FromPdfPage(page);
            XFont font = new XFont("Verdana", 20, XFontStyleEx.Bold);

            // Draw the content
            gfx.DrawString(content, font, XBrushes.Black, new XRect(0, 0, page.Width, page.Height), XStringFormats.Center);

            // Save the document to a memory stream
            using (var stream = new MemoryStream())
            {
                document.Save(stream, false);
                return stream.ToArray(); // Return the PDF as a byte array
            }
        }

        public byte[] CreatePo()
        {
            var fontPaths = new Dictionary<string, string>
            {
                { "Meiryo", Path.Combine(Directory.GetCurrentDirectory(), "AppData/Fonts", "Meiryo.ttf") },
                { "Meiryo-bold", Path.Combine(Directory.GetCurrentDirectory(), "AppData/Fonts", "Meiryo-bold.ttf") }
            };

            // Set the global font resolver
            GlobalFontSettings.FontResolver = new FontResolver(fontPaths);

            // Create a new PDF document
            var document = new PdfDocument();
            document.Info.Title = "Blank Quotation";

            // Add a page
            PdfPage page = document.AddPage();
            XGraphics gfx = XGraphics.FromPdfPage(page);

            // Define table dimensions
            double x = 50; // Left margin
            double y = 67; // Top margin
            double rowHeight = 15;
            double colWidth = 100;

            XColor ClayCreek = XColor.FromCmyk(0.00, 0.06, 0.36, 0.42);
            XFont headerFont = new XFont("Meiryo", 19);
            XFont bodyFont = new XFont("Meiryo", 10);
            XFont bodyFont2 = new XFont("Meiryo", 11);
            XBrush ClayCreekBrush = new XSolidBrush(ClayCreek);
            XPen ClayCreekPen = new XPen(ClayCreek, 2);

            gfx.DrawString("発注書", headerFont, XBrushes.Black, new XRect(0, y, page.Width, rowHeight), XStringFormats.Center);

            y += 54;

            //gfx.DrawRectangle(XPens.Black, XBrushes.LightGray, x, y, 50, rowHeight);
            gfx.DrawString("伝票番号：", bodyFont, XBrushes.Black, new XRect(x, y, 50, rowHeight), XStringFormats.CenterLeft);

            // Quotation number
            //gfx.DrawRectangle(XPens.Black, XBrushes.LightGray, 100, y, 100, rowHeight);
            gfx.DrawString("12345678/FF", bodyFont, XBrushes.Black, new XRect(100, y, 100, rowHeight), XStringFormats.CenterLeft);

            // Load an image from file
            string imagePath = Path.Combine(Directory.GetCurrentDirectory(), "AppData/Images", "FFJ_LOGO.jpg");
            XImage image = XImage.FromFile(imagePath);
            gfx.DrawImage(image, 384, y+1, 175, 62);

            y += rowHeight;

            //gfx.DrawRectangle(XPens.Black, XBrushes.LightGray, x, y, 50, rowHeight);
            gfx.DrawString("発注日付：", bodyFont, XBrushes.Black, new XRect(x, y, 50, rowHeight), XStringFormats.Center);

            // Quotation date
            //gfx.DrawRectangle(XPens.Black, XBrushes.LightGray, 100, y, 100, rowHeight);
            gfx.DrawString("2025年1月27日", bodyFont, XBrushes.Black, new XRect(100, y, 100, rowHeight), XStringFormats.CenterLeft);

            y += 32;

            // Supplier Company name
            //gfx.DrawRectangle(XPens.Black, XBrushes.LightGray, 36, y+3, 300, 20);
            gfx.DrawString("株式会社サンプル　〇〇支社", new XFont("Meiryo", 14), XBrushes.Black, new XRect(36, y+3, 300, 20), XStringFormats.Center);


            gfx.DrawString("御 中", bodyFont2, XBrushes.Black, new XRect(340, y+8, 30, rowHeight), XStringFormats.Center);

            y += 25;

            gfx.DrawLine(new XPen(ClayCreek, 1.5), 36, y, 338, y);

            y += 3;

            // Supplier Company address
            //gfx.DrawRectangle(XPens.Black, XBrushes.LightGray, 36, 194, 300, 12);
            gfx.DrawString("〒 123-1234　東京都世田谷区〇〇〇 1-2-3", new XFont("Meiryo", 9), XBrushes.Black, new XRect(36, 194, 300, 12), XStringFormats.Center);

            gfx.DrawRectangle(new XPen(ClayCreek, 0.75), XBrushes.Transparent, 370, y, 184, 95);
            //gfx.DrawRectangle(new XPen(ClayCreek, 0.75), XBrushes.Transparent, 385, y+10, 154, rowHeight);
            gfx.DrawString("Faraday Factory Japan合同会社", new XFont("Meiryo-bold", 9), XBrushes.Black, new XRect(385, y + 10, 154, rowHeight), XStringFormats.CenterLeft);

            //gfx.DrawRectangle(new XPen(ClayCreek, 0.75), XBrushes.Transparent, 385, y+rowHeight*2, 154, 10);
            gfx.DrawString("〒252-0003", new XFont("Meiryo", 7), XBrushes.Black, new XRect(385, y + rowHeight * 2, 154, 10), XStringFormats.CenterLeft);

            //gfx.DrawRectangle(new XPen(ClayCreek, 0.75), XBrushes.Transparent, 385, y+ rowHeight * 3, 154, 10);
            gfx.DrawString("神奈川県座間市ひばりが丘5⁻12⁻15", new XFont("Meiryo", 7), XBrushes.Black, new XRect(385, y + rowHeight * 3, 154, 10), XStringFormats.CenterLeft);

            //gfx.DrawRectangle(new XPen(ClayCreek, 0.75), XBrushes.Transparent, 385, y+ rowHeight * 4, 154, 10);
            gfx.DrawString("☎：03-4335-3000", new XFont("Meiryo", 7), XBrushes.Black, new XRect(385, y + rowHeight * 4, 154, 10), XStringFormats.CenterLeft);

            //gfx.DrawRectangle(new XPen(ClayCreek, 0.75), XBrushes.Transparent, 385, y+ rowHeight * 5, 154, 10);
            gfx.DrawString("✉：e.mployee@faradaygroup.co", new XFont("Meiryo", 8), XBrushes.Black, new XRect(385, y + rowHeight * 5, 154, 10), XStringFormats.CenterLeft);


            y += 12;

            // Supplier contact person name
            gfx.DrawString("担当者：太郎 様", bodyFont2, XBrushes.Black, new XRect(36, y, 300, rowHeight), XStringFormats.Center);

            y += rowHeight*2 + 3;

            // Message
            //gfx.DrawRectangle(new XPen(ClayCreek, 1), XBrushes.Transparent, 38, y, 150, rowHeight);
            gfx.DrawString("下記の通り納品申し上げます。", bodyFont2, XBrushes.Black, new XRect(38, y, 300, rowHeight), XStringFormats.CenterLeft);

            y += 23;

            //gfx.DrawRectangle(new XPen(ClayCreek, 1), XBrushes.Transparent, 31, y, 150, rowHeight);
            gfx.DrawString("【 合計金額 】", new XFont("Meiryo-bold", 16), ClayCreekBrush, new XRect(31, y, 150, rowHeight), XStringFormats.CenterLeft);

            y += rowHeight + 6;

            gfx.DrawLine(new XPen(ClayCreek, 0.75), 36, y+0.5, 338, y + 0.5);
            gfx.DrawLine(new XPen(ClayCreek, 0.75), 36, y+2, 338, y+2);

            // Tax included
            //gfx.DrawRectangle(new XPen(ClayCreek, 1), XBrushes.Transparent, 40, y-1, 75, rowHeight);
            gfx.DrawString("(税込)", new XFont("Meiryo", 8), XBrushes.Black, new XRect(40, y-1, 75, rowHeight), XStringFormats.Center);

            y += 22;

            var tableWitdh = 520;
            var column1 = 300;
            var column2 = 45;
            var column3 = 85;
            var column4 = 90;
            var tableRowHeight = 26;

            //gfx.DrawRectangle(new XPen(ClayCreek, 0.75), new XSolidBrush(ClayCreek), 36, y, tableWitdh, 32);
            gfx.DrawRectangle(new XPen(ClayCreek, 0.75), new XSolidBrush(ClayCreek), 36, y, column1, 32);
            gfx.DrawString("商品名 ／ 品目", new XFont("Meiryo-bold", 12), XBrushes.White, new XRect(36, y, column1, 32), XStringFormats.Center);

            gfx.DrawRectangle(new XPen(ClayCreek, 0.75), new XSolidBrush(ClayCreek), 36 + column1, y, column2, 32);
            gfx.DrawString("数　量", new XFont("Meiryo-bold", 12), XBrushes.White, new XRect(36 + column1, y, column2, 32), XStringFormats.Center);

            gfx.DrawRectangle(new XPen(ClayCreek, 0.75), new XSolidBrush(ClayCreek), 36 + column1 + column2, y, column3, 32);
            gfx.DrawString("単　価", new XFont("Meiryo-bold", 12), XBrushes.White, new XRect(36 + column1 + column2, y, column3, 32), XStringFormats.Center);

            gfx.DrawRectangle(new XPen(ClayCreek, 0.75), new XSolidBrush(ClayCreek), 36 + column1 + column2 + column3, y, column4, 32);
            gfx.DrawString("金　額", new XFont("Meiryo-bold", 12), XBrushes.White, new XRect(36 + column1 + column2 + column3, y, column4, 32), XStringFormats.Center);

            y += 32;

            List<(string itemName, int quantity, int price)> items = new List<(string, int, int)>
            {
                ("○○○○○○　サンプル　タイプＡ", 123456, 10),
                ("△△△△　システム機器（ 自動調整タイプ ）", 2, 123456789),
                ("△△△△　システムの取付作業", 3, 30000),
                ("△△△△　システムの操作説明　講習会", 40, 4000),
                ("□□□□○○○○素材　（　✖✖　を含む　）", 50, 5000)
            };

            double totalAmount = 0;

            var y1 = y;

            foreach (var item in items)
            {
                gfx.DrawString(item.itemName, new XFont("Meiryo", 10), XBrushes.Black, new XRect(42, y1, column1, tableRowHeight), XStringFormats.CenterLeft);
                gfx.DrawString(item.quantity.ToString(), new XFont("Meiryo", 10), XBrushes.Black, new XRect(36 + column1, y1, column2, tableRowHeight), XStringFormats.Center);
                gfx.DrawString(item.price.ToString("N0", new CultureInfo("ja-JP")), new XFont("Meiryo", 10), XBrushes.Black, new XRect(32 + column1 + column2, y1, column3, tableRowHeight), XStringFormats.CenterRight);

                var totalprice = item.price * item.quantity;
                gfx.DrawString(totalprice.ToString("N0", new CultureInfo("ja-JP")), new XFont("Meiryo", 10), XBrushes.Black, new XRect(32 + column1 + column2 + column3, y1, column4, tableRowHeight), XStringFormats.CenterRight);

                totalAmount += totalprice;

                y1 += tableRowHeight;
            }

            // Draw table rows
            for (int row = 1; row < 10; row++)
            {
                gfx.DrawRectangle(new XPen(ClayCreek, 0.75), XBrushes.Transparent, 36, y, column1, tableRowHeight);

                gfx.DrawRectangle(new XPen(ClayCreek, 0.75), XBrushes.Transparent, 36 + column1, y, column2, tableRowHeight);

                gfx.DrawRectangle(new XPen(ClayCreek, 0.75), XBrushes.Transparent, 36 + column1 + column2, y, column3, tableRowHeight);

                gfx.DrawRectangle(new XPen(ClayCreek, 0.75), XBrushes.Transparent, 36 + column1 + column2 + column3, y, column4, tableRowHeight);

                y += tableRowHeight;
            }

            y += 3;

            // Total item price
            //gfx.DrawRectangle(new XPen(ClayCreek, 0.75), XBrushes.Transparent, 35 + column1 + column2, y, 85, 26);
            gfx.DrawString("小　計 (税抜)", new XFont("Meiryo-bold", 12), XBrushes.Black, new XRect(33 + column1 + column2, y, 80, 26), XStringFormats.CenterRight);
            //gfx.DrawRectangle(new XPen(ClayCreek, 0.75), XBrushes.Transparent, 35 + column1 + column2 + column3, y, 87, 26);
            gfx.DrawString(totalAmount.ToString("C0", new CultureInfo("ja-JP")), new XFont("Meiryo-bold", 11), XBrushes.Black, new XRect(33 + column1 + column2 + column3, y, 87, 26), XStringFormats.CenterRight);

            y += 26;

            var taxAmount = totalAmount * 0.1;

            // Total tax
            //gfx.DrawRectangle(new XPen(ClayCreek, 0.75), XBrushes.Transparent, 35 + column1 + column2, y, 85, 26);
            gfx.DrawString("消費税 (10%)", new XFont("Meiryo-bold", 12), XBrushes.Black, new XRect(33 + column1 + column2, y, 80, 26), XStringFormats.CenterRight);
            //gfx.DrawRectangle(new XPen(ClayCreek, 0.75), XBrushes.Transparent, 35 + column1 + column2 + column3, y, 87, 26);
            gfx.DrawString(taxAmount.ToString("C0", new CultureInfo("ja-JP")), new XFont("Meiryo-bold", 11), XBrushes.Black, new XRect(33 + column1 + column2 + column3, y, 87, 26), XStringFormats.CenterRight);

            y += 26;

            totalAmount += taxAmount;

            // Total amount
            //gfx.DrawRectangle(new XPen(ClayCreek, 1), XBrushes.Transparent, 181, y-4, 150, 20);
            gfx.DrawString(totalAmount.ToString("C0", new CultureInfo("ja-JP")), new XFont("Meiryo-bold", 16), XBrushes.Black, new XRect(181, 261, 150, 20), XStringFormats.CenterRight);

            // Total amount
            //gfx.DrawRectangle(new XPen(ClayCreek, 0.75), XBrushes.Transparent, 35 + column1 + column2, y, 85, 26);
            gfx.DrawString("合　計 (税込)", new XFont("Meiryo-bold", 12), XBrushes.Black, new XRect(33 + column1 + column2, y, 80, 26), XStringFormats.CenterRight);
            //gfx.DrawRectangle(new XPen(ClayCreek, 0.75), XBrushes.Transparent, 35 + column1 + column2 + column3, y, 87, 26);
            gfx.DrawString(totalAmount.ToString("C0", new CultureInfo("ja-JP")), new XFont("Meiryo-bold", 11), XBrushes.Black, new XRect(33 + column1 + column2 + column3, y, 87, 26), XStringFormats.CenterRight);

            y += 26;

            gfx.DrawLine(new XPen(ClayCreek, 3), 36 + column1, y, page.Width - 39, y);

            y += 20;

            gfx.DrawString("納期：", new XFont("Meiryo-bold", 10), XBrushes.Black, new XRect(36, y, 50, rowHeight), XStringFormats.CenterLeft);
            gfx.DrawRectangle(XPens.Black, XBrushes.LightGray, 86, y, 300, rowHeight);

            y += 18;

            gfx.DrawString("決済条件：", new XFont("Meiryo-bold", 10), XBrushes.Black, new XRect(36, y, 50, rowHeight), XStringFormats.CenterLeft);
            gfx.DrawRectangle(XPens.Black, XBrushes.LightGray, 86, y, 300, rowHeight);

            y += 18;

            gfx.DrawString("受渡場所：", new XFont("Meiryo-bold", 10), XBrushes.Black, new XRect(36, y, 50, rowHeight), XStringFormats.CenterLeft);
            gfx.DrawRectangle(XPens.Black, XBrushes.LightGray, 86, y, 300, rowHeight);

            y += rowHeight * 3;

            XPen customDashedPen = new XPen(ClayCreek, 1.3);
            customDashedPen.DashPattern = new double[] { 8, 3, 0.8, 3, 0.8, 3 };
            gfx.DrawLine(customDashedPen, 36, y, page.Width - 36, y);
            gfx.DrawLine(new XPen(ClayCreek, 2), 36, y + 5, page.Width - 36, y + 5);

            // Save to memory stream
            using (var stream = new MemoryStream())
            {
                document.Save(stream, false);
                return stream.ToArray();
            }
        }

        public byte[] CreatePdfWithTable(string title, string[][] tableData)
        {
            // Create a new PDF document
            var document = new PdfDocument();
            document.Info.Title = title;

            // Add a page
            PdfPage page = document.AddPage();
            XGraphics gfx = XGraphics.FromPdfPage(page);

            // Define fonts
            XFont headerFont = new XFont("Arial", 12, XFontStyleEx.Bold);
            XFont cellFont = new XFont("Arial", 10, XFontStyleEx.Regular);

            // Define table dimensions
            double x = 40; // Left margin
            double y = 60; // Top margin
            double rowHeight = 20;
            double colWidth = 100;

            // Draw table headers
            for (int col = 0; col < tableData[0].Length; col++)
            {
                gfx.DrawRectangle(XPens.Black, XBrushes.LightGray, x + col * colWidth, y, colWidth, rowHeight);
                gfx.DrawString(tableData[0][col], headerFont, XBrushes.Black,
                    new XRect(x + col * colWidth, y, colWidth, rowHeight),
                    XStringFormats.Center);
            }

            y += rowHeight;

            // Draw table rows
            for (int row = 1; row < tableData.Length; row++)
            {
                for (int col = 0; col < tableData[row].Length; col++)
                {
                    gfx.DrawRectangle(XPens.Black, XBrushes.White, x + col * colWidth, y, colWidth, rowHeight);
                    gfx.DrawString(tableData[row][col], cellFont, XBrushes.Black,
                        new XRect(x + col * colWidth, y, colWidth, rowHeight),
                        XStringFormats.CenterLeft);
                }
                y += rowHeight;
            }

            // Save to memory stream
            using (var stream = new MemoryStream())
            {
                document.Save(stream, false);
                return stream.ToArray();
            }
        }
    }
}
