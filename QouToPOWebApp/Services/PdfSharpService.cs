using PdfSharp.Drawing;
using PdfSharp.Drawing.Layout;
using PdfSharp.Fonts;
using PdfSharp.Pdf;
using QouToPOWebApp.ViewModel;
using System.Globalization;
using QouToPOWebApp.Helpers;
using System.Text.RegularExpressions;

namespace QouToPOWebApp.Services
{
    public class PdfSharpService
    {
        public PdfSharpService()
        {
            // Set the global font resolver
            GlobalFontSettings.FontResolver = FontResolver.Instance;
        }

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

        public byte[] SamplePo()
        {
            var po = new PoViewModel
            {
                Po_number = "20250219/FF-001-092",
                Quotation_number = "Q24044",
                Po_date = DateTime.Parse("2025-2-19"),
                Include_tax = true,
                Email = "sample@faradaygroup.com",
                Po_title = "Sample title",
                Companies = new Models.InfoModels.Company
                {
                    Address = "Zama factory, 5-12-15 Hibarigaoka, Zama, Kanagawa, 252-0003",
                    Address_jpn = "座間工場 〒252-0003 神奈川県座間市ひばりが丘5⁻12⁻15"
                },
                Contact_persons = new Models.InfoModels.Contact_person
                {
                    Contact_person_name = "△△  □□",
                    Contact_person_name_jpn = "△△  □□ ",
                    Company = new Models.InfoModels.Company
                    {
                        Company_name = "Company Sample Inc.",
                        Company_name_jpn = "株式会社サンプル　〇〇支社",
                        Address = "1-2-3 〇〇〇, Setagaya Ward, Tokyo",
                        Address_jpn = "東京都世田谷区〇〇〇 1-2-3",
                        Postal_code = "123-1234"
                    }
                },
                Po_items = new List<Models.PoModels.Po_item>
                {
                    new Models.PoModels.Po_item 
                    {
                        Item_name = "○○○○○○　サンプル　タイプＡ\nline1",
                        Item_price = 123456,
                        Item_quantity = 10,
                        Unit = "sets",
                        Order = 1
                    },
                    new Models.PoModels.Po_item
                    {
                        Item_name = "△△△△　システム機器\nline1\nline2\nline3",
                        Item_price = 2,
                        Item_quantity = 12345,
                        Unit = "pcs",
                        Order = 2
                    },
                    new Models.PoModels.Po_item
                    {
                        Item_name = "△△△△　システムの取付作業\nline1\nline2",
                        Item_price = 3,
                        Item_quantity = 30000,
                        Order = 3
                    },
                    new Models.PoModels.Po_item
                    {
                        Item_name = "△△△△　システムの操作説明　講習会\nline1\nline2",
                        Item_price = 40,
                        Item_quantity = 400,
                        Unit = "hours",
                        Order = 4
                    },
                    new Models.PoModels.Po_item
                    {
                        Item_name = "□□□□○○○○素材　（　✖✖　を含む　）",
                        Item_price = 50,
                        Item_quantity = 5000,
                        Unit = "kg",
                        Order = 5
                    }
                }
            };

            return CreatePo(po);
        }

        public byte[] SamplePoEng()
        {
            var po = new PoViewModel
            {
                Po_number = "20250219/FF-001-092",
                Quotation_number = "Q24044",
                Po_date = DateTime.Parse("2025-2-19"),
                Include_tax = true,
                Email = "sample@faradaygroup.com",
                Po_title = "Sample title",
                Companies = new Models.InfoModels.Company
                {
                    Address = "5-12-15 Hibarigaoka, Zama, Kanagawa 252-0003, Japan",
                    Address_jpn = "神奈川県座間市ひばりが丘5⁻12⁻15",
                    Postal_code = "252-0003"
                },
                Contact_persons = new Models.InfoModels.Contact_person
                {
                    Contact_person_name = "Luffy Taro",
                    Contact_person_name_jpn = "△△  □□ ",
                    Company = new Models.InfoModels.Company
                    {
                        Company_name = "Company Sample Inc.",
                        Company_name_jpn = "株式会社サンプル　〇〇支社",
                        Address = "1-2-3 ****, Setagaya Ward, Tokyo 123-1234, Japan",
                        Address_jpn = "東京都世田谷区〇〇〇 1-2-3",
                        Postal_code = "123-1234"
                    }
                },
                Correspondents = new Models.InfoModels.Correspondent
                {
                    Correspondent_name = "Monkey D. Dragon",
                    Correspondent_position = "CEO"
                },
                Po_items = new List<Models.PoModels.Po_item>
                {
                    new Models.PoModels.Po_item
                    {
                        Item_name = "** Sample type A",
                        Item_price = 123456,
                        Item_quantity = 10,
                        Unit = "sets",
                        Order = 1
                    },
                    new Models.PoModels.Po_item
                    {
                        Item_name = "System equipment",
                        Item_price = 2,
                        Item_quantity = 12345,
                        Unit = "pcs",
                        Order = 2
                    },
                    new Models.PoModels.Po_item
                    {
                        Item_name = "System installation work",
                        Item_price = 3,
                        Item_quantity = 30000,
                        Order = 3
                    },
                    new Models.PoModels.Po_item
                    {
                        Item_name = "System operation training session",
                        Item_price = 40,
                        Item_quantity = 400,
                        Unit = "hours",
                        Order = 4
                    },
                    new Models.PoModels.Po_item
                    {
                        Item_name = "**Material (including **)",
                        Item_price = 50,
                        Item_quantity = 5000,
                        Unit = "kg",
                        Order = 5
                    }
                }
            };

            return CreatePoEng(po);
        }

        public PdfDocument NewDocument()
        {
            var document = new PdfDocument();
            document.Info.Creator = "Faraday Factory Japan LLC";
            document.Info.Author = "Faraday Factory Japan LLC";

            return document;
        }

        public byte[]? CreatePo(PoViewModel po, bool saveToFile = false)
        {
            // Create a new PDF document
            var document = NewDocument();
            document.Info.Title = $"PO for {po?.Contact_persons?.Company?.Company_name}"; // update title according to supplier

            // Add a page
            PdfPage page = document.AddPage();
            page.Size = PdfSharp.PageSize.A4;
            XGraphics gfx = XGraphics.FromPdfPage(page);
            DrawFooter(gfx, page);
            XTextFormatter tf = new XTextFormatter(gfx);

            // Table dimensions
            double topMargin = 65;
            double bottomMargin = 65;
            double leftMargin = 36;
            double rightMargin = 36;
            double x = leftMargin;
            double y = topMargin;
            double rowHeight = 14;

            XColor ClayCreek = XColor.FromCmyk(0.00, 0.06, 0.36, 0.42);
            XFont headerFont = new XFont("Meiryo", 18.5);
            XFont bodyFont = new XFont("Meiryo", 10);
            XFont bodyFont2 = new XFont("Meiryo", 11);
            XBrush ClayCreekBrush = new XSolidBrush(ClayCreek);
            XPen ClayCreekPen = new XPen(ClayCreek, 2);
            var currency = po?.Currency ?? "JPY";

            // Header
            gfx.DrawString("発注書", headerFont, XBrushes.Black, new XRect(x, y, page.Width - (leftMargin + rightMargin), rowHeight), XStringFormats.Center);

            y += 51;

            gfx.DrawString("伝票番号：", bodyFont, XBrushes.Black, new XRect(x + 14, y, 50, rowHeight), XStringFormats.CenterLeft);

            // PO number
            var poNumber = po?.Po_number ?? string.Empty;
            gfx.DrawString(poNumber, bodyFont, XBrushes.Black, new XRect(100, y, 100, rowHeight), XStringFormats.CenterLeft);

            // FFJ logo
            string imagePath = Path.Combine(Directory.GetCurrentDirectory(), "AppData/Images", "FFJ_LOGO.jpg");
            XImage image = XImage.FromFile(imagePath);
            gfx.DrawImage(image, 384, y+1, 175, 62);

            y += rowHeight;

            // Po date
            var poDate = po?.Po_date?.ToString("yyyy年MM月dd日") ?? string.Empty;
            gfx.DrawString("発注日付：", bodyFont, XBrushes.Black, new XRect(x + 14, y, 50, rowHeight), XStringFormats.Center);
            gfx.DrawString(poDate, bodyFont, XBrushes.Black, new XRect(100, y, 100, rowHeight), XStringFormats.CenterLeft);

            y += 30;

            // Supplier Company name
            var supplierName = po?.Contact_persons?.Company?.Company_name_jpn ?? po?.Contact_persons?.Company?.Company_name ?? string.Empty;
            gfx.DrawString(supplierName, new XFont("Meiryo", 14), XBrushes.Black, new XRect(x, y, 300, 20), XStringFormats.Center);


            gfx.DrawString("御 中", bodyFont2, XBrushes.Black, new XRect(339, y+6, 30, rowHeight), XStringFormats.Center);

            y += 23;

            gfx.DrawLine(new XPen(ClayCreek, 1.5), x, y, 339, y);

            y += 1;

            // Supplier Company address
            var supplierAddress = po?.Contact_persons?.Company?.Address_jpn ?? po?.Contact_persons?.Company?.Address ?? string.Empty;
            var postalCode = po?.Contact_persons?.Company?.Postal_code ?? string.Empty;
            gfx.DrawString($"〒 {postalCode}　{supplierAddress}", new XFont("Meiryo", 9), XBrushes.Black, new XRect(x, y, 300, 12), XStringFormats.Center);

            // User details
            gfx.DrawRectangle(new XPen(ClayCreek, 0.75), XBrushes.Transparent, 370, y, 184, 95);
            gfx.DrawString("Faraday Factory Japan合同会社", new XFont("Meiryo", 9, XFontStyleEx.Bold), XBrushes.Black, new XRect(385, y + 10, 154, rowHeight), XStringFormats.CenterLeft);
            gfx.DrawString("〒252-0003", new XFont("Meiryo", 7), XBrushes.Black, new XRect(385, y + rowHeight * 2, 154, 10), XStringFormats.CenterLeft);
            gfx.DrawString("神奈川県座間市ひばりが丘5⁻12⁻15", new XFont("Meiryo", 7), XBrushes.Black, new XRect(385, y + rowHeight * 3, 154, 10), XStringFormats.CenterLeft);
            gfx.DrawString("☎：03-4335-3000", new XFont("Meiryo", 7), XBrushes.Black, new XRect(385, y + rowHeight * 4, 154, 10), XStringFormats.CenterLeft);

            // Employee email
            var email = po?.Email ?? string.Empty;
            gfx.DrawString($"✉：{email}", new XFont("Meiryo", 8), XBrushes.Black, new XRect(385, y + rowHeight * 5, 154, 10), XStringFormats.CenterLeft);


            y += 12;

            // Supplier contact person name
            var contactPerson = po?.Contact_persons?.Contact_person_name_jpn ?? po?.Contact_persons?.Contact_person_name ?? "○○";
            gfx.DrawString($"担当者：{contactPerson} 様", bodyFont2, XBrushes.Black, new XRect(x, y, 300, rowHeight), XStringFormats.Center);

            y += rowHeight*2 + 3;

            // Message
            gfx.DrawString("下記の通り発注申し上げます。", bodyFont2, XBrushes.Black, new XRect(x + 2, y, 300, rowHeight), XStringFormats.CenterLeft);

            y += 23;

            gfx.DrawString("【 合計金額 】", new XFont("Meiryo", 16, XFontStyleEx.Bold), ClayCreekBrush, new XRect(31, y, 150, rowHeight), XStringFormats.CenterLeft);

            y += rowHeight + 6;

            gfx.DrawLine(new XPen(ClayCreek, 0.75), x, y+0.5, 338, y + 0.5);
            gfx.DrawLine(new XPen(ClayCreek, 0.75), x, y+2, 338, y+2);

            // Tax included
            gfx.DrawString("(" + (po.Include_tax ? "税込" : "税抜") + ")", new XFont("Meiryo", 8), XBrushes.Black, new XRect(40, y-1, 85, rowHeight), XStringFormats.Center);

            y += 20;

            var column1 = 290;
            var column2 = 55;
            var column3 = 85;
            var column4 = 90;
            var tableRowHeight = 26;

            // PO Item table Column headers
            gfx.DrawRectangle(new XPen(ClayCreek, 0.75), new XSolidBrush(ClayCreek), x, y, column1, 30);
            gfx.DrawString("商品名 ／ 品目", new XFont("Meiryo", 12, XFontStyleEx.Bold), XBrushes.White, new XRect(x, y, column1, 30), XStringFormats.Center);

            gfx.DrawRectangle(new XPen(ClayCreek, 0.75), new XSolidBrush(ClayCreek), x + column1, y, column2, 30);
            gfx.DrawString("数　量", new XFont("Meiryo", 12, XFontStyleEx.Bold), XBrushes.White, new XRect(x + column1, y, column2, 30), XStringFormats.Center);

            gfx.DrawRectangle(new XPen(ClayCreek, 0.75), new XSolidBrush(ClayCreek), x + column1 + column2, y, column3, 30);
            gfx.DrawString("単　価", new XFont("Meiryo", 12, XFontStyleEx.Bold), XBrushes.White, new XRect(x + column1 + column2, y, column3, 30), XStringFormats.Center);

            gfx.DrawRectangle(new XPen(ClayCreek, 0.75), new XSolidBrush(ClayCreek), x + column1 + column2 + column3, y, column4, 30);
            gfx.DrawString("金　額", new XFont("Meiryo", 12, XFontStyleEx.Bold), XBrushes.White, new XRect(x + column1 + column2 + column3, y, column4, 30), XStringFormats.Center);

            y += 30;

            float totalAmount = 0;

            // PO title
            var title = po?.Po_title ?? string.Empty;

            if (title != string.Empty)
            {
                var y1 = y;
                var currentRowHeight = 12;

                // Measure and wrap text
                var formattedText = BreakTextIntoLines(gfx, title, new XFont("Meiryo", 10, XFontStyleEx.Bold), column1 - 4);

                // Draw each line and adjust Y position
                foreach (var line in formattedText)
                {
                    gfx.DrawString(line, new XFont("Meiryo", 10, XFontStyleEx.Bold), XBrushes.Black, new XRect(42, y1 + 7, column1, tableRowHeight), XStringFormats.TopLeft);
                    y1 += rowHeight;
                    currentRowHeight += 14;
                }

                gfx.DrawRectangle(new XPen(ClayCreek, 0.75), XBrushes.Transparent, x, y, column1, currentRowHeight);
                gfx.DrawRectangle(new XPen(ClayCreek, 0.75), XBrushes.Transparent, x + column1, y, column2, currentRowHeight);
                gfx.DrawRectangle(new XPen(ClayCreek, 0.75), XBrushes.Transparent, x + column1 + column2, y, column3, currentRowHeight);
                gfx.DrawRectangle(new XPen(ClayCreek, 0.75), XBrushes.Transparent, x + column1 + column2 + column3, y, column4, currentRowHeight);

                y += currentRowHeight;
            }

            CheckPageBreak(ref y);

            if (po?.Po_items?.Count() > 0)
            {
                foreach (var item in po?.Po_items?.OrderBy(q => q.Order))
                {
                    var y1 = y;
                    var currentRowHeight = 12;

                    // Remove un-needed characters
                    var itemName = item?.Item_name?.Replace("\r", "").Split("\n");
                    foreach (var lineName in itemName)
                    {

                        // Measure and wrap text
                        var formattedText = BreakTextIntoLines(gfx, lineName, bodyFont, column1 - 4);

                        // Draw each line and adjust Y position
                        foreach (var line in formattedText)
                        {
                            gfx.DrawString(line, bodyFont, XBrushes.Black, new XRect(42, y1 + 7, column1, 10), XStringFormats.TopLeft);
                            y1 += rowHeight;
                            currentRowHeight += 14;
                        }
                    }

                    gfx.DrawRectangle(new XPen(ClayCreek, 0.75), XBrushes.Transparent, x, y, column1, currentRowHeight);

                    var itemQuantity = item?.Item_quantity?.ToString() + (item?.Unit != null ? " " + item.Unit : "");
                    gfx.DrawString(itemQuantity, bodyFont, XBrushes.Black, new XRect(x + column1, y, column2, currentRowHeight), XStringFormats.Center);
                    gfx.DrawRectangle(new XPen(ClayCreek, 0.75), XBrushes.Transparent, x + column1, y, column2, currentRowHeight);

                    CultureInfo culture;
                    if (currency == "USD")
                    {
                        culture = new CultureInfo("en-US");
                    } else
                    {
                        // Replace the problematic line with the following code:
                        culture = new CultureInfo("ja-JP");
                        var hasDecimal = item?.Item_price % 1 != 0;

                        culture.NumberFormat.CurrencyDecimalDigits = hasDecimal ? 2 : 0;
                    }

                    var itemPrice = item?.Item_price?.ToString("C", culture) ?? string.Empty;
                    gfx.DrawString(itemPrice, bodyFont, XBrushes.Black, new XRect(32 + column1 + column2, y, column3, currentRowHeight), XStringFormats.CenterRight);
                    gfx.DrawRectangle(new XPen(ClayCreek, 0.75), XBrushes.Transparent, x + column1 + column2, y, column3, currentRowHeight);

                    float totalprice = (float)(item.Item_price * item.Item_quantity);
                    gfx.DrawString(GetCurrencyAmount(currency, totalprice), bodyFont, XBrushes.Black, new XRect(32 + column1 + column2 + column3, y, column4, currentRowHeight), XStringFormats.CenterRight);
                    gfx.DrawRectangle(new XPen(ClayCreek, 0.75), XBrushes.Transparent, x + column1 + column2 + column3, y, column4, currentRowHeight);

                    totalAmount += totalprice;

                    y += currentRowHeight;
                }
            }

            var itemCount = po.Po_items != null ? po.Po_items.Count() : 0;
            // Draw table rows
            for (int row = itemCount; row < 8; row++)
            {
                if (y < 600) // limit the number of blank rows when it reaches certain point
                {
                    gfx.DrawRectangle(new XPen(ClayCreek, 0.75), XBrushes.Transparent, x, y, column1, tableRowHeight);

                    gfx.DrawRectangle(new XPen(ClayCreek, 0.75), XBrushes.Transparent, x + column1, y, column2, tableRowHeight);

                    gfx.DrawRectangle(new XPen(ClayCreek, 0.75), XBrushes.Transparent, x + column1 + column2, y, column3, tableRowHeight);

                    gfx.DrawRectangle(new XPen(ClayCreek, 0.75), XBrushes.Transparent, x + column1 + column2 + column3, y, column4, tableRowHeight);

                    y += tableRowHeight;
                }
            }

            y += 3;

            CheckPageBreak(ref y);

            // Total item price
            gfx.DrawString("小　計 (税抜)", new XFont("Meiryo", 12, XFontStyleEx.Bold), XBrushes.Black, new XRect(33 + column1 + column2, y, 80, 26), XStringFormats.CenterRight);
            gfx.DrawString(GetCurrencyAmount(currency, totalAmount), new XFont("Meiryo", 11, XFontStyleEx.Bold), XBrushes.Black, new XRect(33 + column1 + column2 + column3, y, 87, 26), XStringFormats.CenterRight);

            y += 26;

            if (po.Include_tax)
            {
                float taxAmount = (float)(totalAmount * 0.1);

                // Total tax
                gfx.DrawString("消費税", new XFont("Meiryo", 12), XBrushes.Black, new XRect(33 + column1 + column2, y, 40, 26), XStringFormats.CenterRight);
                gfx.DrawString("(10%)", new XFont("Meiryo", 10, XFontStyleEx.Bold), XBrushes.Black, new XRect(93 + column1 + column2, y, 20, 26), XStringFormats.CenterRight);
                gfx.DrawString(GetCurrencyAmount(currency, taxAmount), new XFont("Meiryo", 11, XFontStyleEx.Bold), XBrushes.Black, new XRect(33 + column1 + column2 + column3, y, 87, 26), XStringFormats.CenterRight);

                y += 26;

                totalAmount += taxAmount;
            }

            // Total amount
            gfx.DrawString(GetCurrencyAmount(currency, totalAmount) + "-", new XFont("Meiryo", 16, XFontStyleEx.Bold), XBrushes.Black, new XRect(181, 246, 150, 20), XStringFormats.CenterRight);

            // Total amount
            gfx.DrawString("合　計 (" + (po.Include_tax ? "税込" : "税抜") + ")", new XFont("Meiryo", 12, XFontStyleEx.Bold), XBrushes.Black, new XRect(33 + column1 + column2, y, 80, 26), XStringFormats.CenterRight);
            gfx.DrawString(GetCurrencyAmount(currency, totalAmount), new XFont("Meiryo", 11, XFontStyleEx.Bold), XBrushes.Black, new XRect(33 + column1 + column2 + column3, y, 87, 26), XStringFormats.CenterRight);

            y += 26;

            gfx.DrawLine(new XPen(ClayCreek, 3), x + column1, y, page.Width - 39, y);

            y += rowHeight + 5;

            CheckPageBreak(ref y);

            // Delivery term
            if (!string.IsNullOrEmpty(po?.Delivery_term))
            {
                var deliveryTerm = po?.Delivery_term ?? string.Empty;
                gfx.DrawString("納期：", new XFont("Meiryo", 10, XFontStyleEx.Bold), XBrushes.Black, new XRect(x, y, 50, rowHeight), XStringFormats.CenterLeft);
                gfx.DrawString(deliveryTerm, bodyFont, XBrushes.Black, new XRect(66, y, 300, rowHeight), XStringFormats.CenterLeft);

                y += 18;
            }

            CheckPageBreak(ref y);

            // Payment term
            if (!string.IsNullOrEmpty(po?.Payment_term))
            {
                var paymentTerm = po?.Payment_term;
                gfx.DrawString("決済条件：", new XFont("Meiryo", 10, XFontStyleEx.Bold), XBrushes.Black, new XRect(x, y, 50, rowHeight), XStringFormats.CenterLeft);
                gfx.DrawString(paymentTerm, bodyFont, XBrushes.Black, new XRect(86, y, 300, rowHeight), XStringFormats.CenterLeft);

                y += 18;
            }

            CheckPageBreak(ref y);

            // Custom term
            var stripedCustomTerm = Regex.Replace(po?.Custom_term ?? string.Empty, "<.*?>", string.Empty);
            if (!string.IsNullOrEmpty(po?.Custom_term) && stripedCustomTerm != "")
            {
                y += 10;
                var customTerm = po?.Custom_term;
                var (updatedPage, updatedGfx, updatedY) = HtmlToPdfHelper.RenderHtmlOnPages(
                    document, page, gfx, customTerm, 36, y, page.Width - 80, new XFont("Meiryo", 10));

                page = updatedPage;
                gfx = updatedGfx;
                y = updatedY;
            }

            CheckPageBreak(ref y);

            // Delivery address
            gfx.DrawString("受渡場所：", new XFont("Meiryo", 10, XFontStyleEx.Bold), XBrushes.Black, new XRect(x, y, 50, rowHeight), XStringFormats.CenterLeft);
            var deliveryAddress = po?.Companies?.Address_jpn ?? po?.Companies?.Address ?? string.Empty;
            gfx.DrawString(deliveryAddress, bodyFont, XBrushes.Black, new XRect(86, y, 300, rowHeight), XStringFormats.CenterLeft);


            if (saveToFile)
            {
                document.Save(po.File_path);
                document.Close();

                return null;
            } else
            {
                // Save to memory stream
                using (var stream = new MemoryStream())
                {
                    document.Save(stream, false);
                    document.Dispose();

                    return stream.ToArray();
                }
            }

            void CheckPageBreak(ref double y)
            {
                if (y + rowHeight >= page.Height.Point - bottomMargin - rowHeight)
                {
                    page = document.AddPage();
                    page.Size = PdfSharp.PageSize.A4;
                    gfx = XGraphics.FromPdfPage(page);
                    DrawFooter(gfx, page);
                    y = topMargin;
                }
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

        public byte[]? CreatePoEng(PoViewModel po, bool saveToFile = false)
        {
            // Create a new PDF document
            var document = NewDocument();
            var poTitle = po?.Po_title ?? string.Empty;
            document.Info.Title = poTitle;

            // Add a page
            PdfPage page = document.AddPage();
            page.Size = PdfSharp.PageSize.A4;
            XGraphics gfx = XGraphics.FromPdfPage(page);

            // Table dimensions
            double topMargin = 43;
            double bottomMargin = 43;
            double leftMargin = 43;
            double rightMargin = 42;
            double x = leftMargin;
            double y = topMargin;
            double rowHeight = 13;

            XFont calibri = new XFont("Calibri", 11);
            XFont calibriBold = new XFont("Calibri", 11, XFontStyleEx.Bold);
            XFont calibriBoldItalic = new XFont("Calibri", 11, XFontStyleEx.BoldItalic);
            XFont arial = new XFont("Arial", 10);
            XFont arialBold = new XFont("Arial", 10, XFontStyleEx.Bold);
            XFont arialItalic = new XFont("Arial", 10, XFontStyleEx.Italic);
            XFont tnrBold = new XFont("Times new roman", 10.5, XFontStyleEx.Bold);

            var currency = po?.Currency ?? "USD";

            // FFJ logo
            string imagePath = Path.Combine(Directory.GetCurrentDirectory(), "AppData/Images", "FFJ_LOGO.jpg");
            XImage image = XImage.FromFile(imagePath);
            gfx.DrawImage(image, 58, y, 189, 71);

            gfx.DrawString("Faraday Factory Japan LLC", tnrBold, XBrushes.Black, new XRect(282, y, 150, 10), XStringFormats.CenterLeft);

            y += rowHeight;

            gfx.DrawString("FINE Bldg.,", calibri, XBrushes.Black, new XRect(282, y, 150, 10), XStringFormats.CenterLeft);

            y += rowHeight;

            gfx.DrawString("2956-6 Ishikawa-machi, Hachioji,", calibri, XBrushes.Black, new XRect(282, y, 150, 10), XStringFormats.CenterLeft);

            y += rowHeight;

            gfx.DrawString("Tokyo 192-0032, Japan", calibri, XBrushes.Black, new XRect(282, y, 150, 10), XStringFormats.CenterLeft);

            y += rowHeight;

            gfx.DrawString("TEL: +81-42-707-7077", calibri, XBrushes.Black, new XRect(282, y, 150, 10), XStringFormats.CenterLeft);

            y += rowHeight;

            gfx.DrawString("FAX: +81-42-707-9044", calibri, XBrushes.Black, new XRect(282, y, 150, 10), XStringFormats.CenterLeft);

            y += rowHeight;
            
            gfx.DrawLine(new XPen(XColors.Black, 3), 37, y, page.Width - 42, y);
            gfx.DrawLine(new XPen(XColors.Black, 1), 238, y + 3, page.Width - 42, y + 3);

            var poDate = (bool)(po?.Po_date.HasValue) ? po?.Po_date.Value.ToString("MMMM d, yyyy") : DateTime.Now.ToString("MMMM d, yyyy");
            gfx.DrawString(poDate, calibri, XBrushes.Black, new XRect(37, y + 5, page.Width - 80, 10), XStringFormats.CenterRight);

            y += 29;

            // Contact person
            var contactPerson = po?.Contact_persons?.Contact_person_name ?? po?.Contact_persons?.Contact_person_name_jpn ?? string.Empty;
            gfx.DrawString(contactPerson, arial, XBrushes.Black, new XRect(x, y, 200, 10), XStringFormats.CenterLeft);

            // Po number
            gfx.DrawString("Purchase Order:", arialBold, XBrushes.Black, new XRect(300, y, 75, 10), XStringFormats.CenterLeft);
            gfx.DrawString(po?.Po_number ?? string.Empty, arial, XBrushes.Black, new XRect(383, y, 100, 10), XStringFormats.CenterLeft);

            y += rowHeight;

            // Supplier company name
            var supplierName = po?.Contact_persons?.Company?.Company_name ?? po?.Contact_persons?.Company?.Company_name_jpn ?? string.Empty;
            gfx.DrawString(supplierName, arial, XBrushes.Black, new XRect(x, y, 200, 10), XStringFormats.CenterLeft);

            y += rowHeight;

            // Reference
            gfx.DrawString("Ref.:", arialBold, XBrushes.Black, new XRect(300, y, 25, 10), XStringFormats.CenterLeft);

            var quotationnumber = po?.Quotation_number ?? string.Empty;
            gfx.DrawString(quotationnumber, calibri, XBrushes.Black, new XRect(328, y, 100, 10), XStringFormats.CenterLeft);
            gfx.DrawString("Quotation for Faraday Factory Japan", arialItalic, XBrushes.Black, new XRect(328, y+rowHeight, 200, 10), XStringFormats.CenterLeft);

            XTextFormatter tf = new XTextFormatter(gfx);
            // Supplier Address
            var supplierAddress = po?.Contact_persons?.Company?.Address ?? po?.Contact_persons?.Company?.Address_jpn ?? string.Empty;
            var telephone = "\nTel.: " + (po?.Contact_persons?.Company?.Telephone ?? string.Empty);
            var fax = "\nFax: " + (po?.Contact_persons?.Company?.Fax ?? string.Empty);

            var fullSupplierAdd = ($"{supplierAddress}{telephone}{fax}").Replace("\r", "").Split("\n");
            foreach (var lineName in fullSupplierAdd)
            {
                // Draw each line and adjust Y position
                foreach (var line in BreakTextIntoLines(gfx, lineName, arial, 130))
                {
                    gfx.DrawString(line, arial, XBrushes.Black, new XRect(x, y, 130, rowHeight), XStringFormats.TopLeft);
                    y += rowHeight;
                }
            }

            y += rowHeight * 3;

            // Contact Person
            gfx.DrawString($"Dear {contactPerson},", arial, XBrushes.Black, new XRect(x, y - 6, 200, 10), XStringFormats.CenterLeft);

            y += rowHeight;


            // Message
            var message = $"Faraday Factory Japan LLC would like to place the Official Purchase Order for the {poTitle}, based on your Price Quotation {quotationnumber}.";

            // Measure and wrap text
            var formattedMessage = BreakTextIntoLines(gfx, message, calibri, page.Width - 89);

            // Draw each line and adjust Y position
            foreach (var line in formattedMessage)
            {
                gfx.DrawString(line, calibri, XBrushes.Black, new XRect(x, y, page.Width - 85, 30), XStringFormats.TopLeft);
                y += rowHeight;
            }

            y += 26;

            var column1 = 30;
            var column2 = 270;
            var column3 = 50;
            var column4 = 80;
            var column5 = 65;
            var tableWitdh = column1 + column2 + column3 + column4 + column5;

            // PO items table header
            gfx.DrawString(" No", calibriBold, XBrushes.Black, new XRect(x, y, column1, 10), XStringFormats.CenterLeft);
            gfx.DrawString("Description", calibriBold, XBrushes.Black, new XRect(x + column1, y, column2, 10), XStringFormats.Center);
            gfx.DrawString("QTY", calibriBold, XBrushes.Black, new XRect(x + column1 + column2, y, column3, 10), XStringFormats.Center);
            gfx.DrawString("Unit Price", calibriBold, XBrushes.Black, new XRect(x + column1 + column2 + column3, y, column4, 10), XStringFormats.Center);
            gfx.DrawString("Total", calibriBold, XBrushes.Black, new XRect(x + column1 + column2 + column3 + column4, y, column5, 10), XStringFormats.Center);

            y += rowHeight;

            gfx.DrawString(currency, calibriBold, XBrushes.Black, new XRect(x + column1 + column2 + column3, y, column4, 10), XStringFormats.Center);
            gfx.DrawString(currency, calibriBold, XBrushes.Black, new XRect(x + column1 + column2 + column3 + column4, y, column5, 10), XStringFormats.Center);

            y += rowHeight;

            gfx.DrawLine(new XPen(XColors.Black, 0.5), 37, y, tableWitdh + x, y);

            float totalAmount = 0;

            y += 3;

            if (po?.Po_items?.Count() > 0)
            {
                foreach (var item in po?.Po_items?.OrderBy(q => q.Order))
                {
                    CheckPageBreak(ref y);

                    gfx.DrawString(item?.Order?.ToString(), calibri, XBrushes.Black, new XRect(x, y, column1, rowHeight), XStringFormats.CenterLeft);

                    var y1 = y;
                    double currentRowHeight = 0;

                    var itemName = item?.Item_name?.Replace("\r", "").Split("\n");
                    foreach (var lineName in itemName)
                    {
                        // Measure and wrap text
                        var formattedText = BreakTextIntoLines(gfx, lineName, calibri, column2);

                        // Draw each line and adjust Y position
                        foreach (var line in formattedText)
                        {
                            gfx.DrawString(line, calibri, XBrushes.Black, new XRect(x + column1, y1, column2, rowHeight), XStringFormats.CenterLeft);
                            y1 += rowHeight;
                            currentRowHeight += rowHeight;
                        }
                    }

                    gfx.DrawString(item?.Item_quantity?.ToString() + (item?.Unit != null ? " " + item?.Unit : ""), calibri, XBrushes.Black, new XRect(x + column1 + column2, y, column3, currentRowHeight), XStringFormats.TopCenter);
                    gfx.DrawString(GetCurrencyAmount(currency, item?.Item_price, false), calibri, XBrushes.Black, new XRect(x + column1 + column2 + column3, y, column4, currentRowHeight), XStringFormats.TopCenter);

                    float totalprice = (float)(item?.Item_price * item?.Item_quantity);
                    gfx.DrawString(GetCurrencyAmount(currency, totalprice, false), calibri, XBrushes.Black, new XRect(x + column1 + column2 + column3 + column4, y, column5, currentRowHeight), XStringFormats.TopCenter);

                    totalAmount += totalprice;

                    y += currentRowHeight + 3;
                }
            }

            if (po.Include_tax)
            {
                y += rowHeight;

                CheckPageBreak(ref y);

                float taxAmount = (float)(totalAmount * 0.1);

                // Total tax
                gfx.DrawString("Consumption tax 10%", calibri, XBrushes.Black, new XRect(x + column1, y, column2, rowHeight), XStringFormats.CenterLeft);
                gfx.DrawString(GetCurrencyAmount(currency, taxAmount, false), calibri, XBrushes.Black, new XRect(x + column1 + column2 + column3 + column4, y, column5, rowHeight), XStringFormats.Center);

                y += rowHeight;

                totalAmount += taxAmount;
            }

            y = y < 400 ? 400 : y + 5; // set the default height of table

            gfx.DrawLine(new XPen(XColors.Black, 0.5), 37, y, tableWitdh + x, y);

            y += rowHeight;

            CheckPageBreak(ref y);

            gfx.DrawString("TOTAL:", calibriBold, XBrushes.Black, new XRect(x + column1 + column2, y, column3, 10), XStringFormats.Center);
            gfx.DrawString(currency, calibriBold, XBrushes.Black, new XRect(x + column1 + column2 + column3, y, column4, 10), XStringFormats.Center);
            gfx.DrawString(GetCurrencyAmount(currency, totalAmount, false), calibriBold, XBrushes.Black, new XRect(x + column1 + column2 + column3 + column4, y, column5, 10), XStringFormats.Center);

            y += rowHeight * 3;

            if (!string.IsNullOrEmpty(po?.Delivery_term))
            {
                CheckPageBreak(ref y);

                gfx.DrawString("Lead Time:", calibriBold, XBrushes.Black, new XRect(x, y, 54, 10), XStringFormats.CenterLeft);

                var deliveryTerm = po?.Delivery_term ?? string.Empty;
                gfx.DrawString(deliveryTerm, calibri, XBrushes.Black, new XRect(x + 54, y, page.Width - (x * 2 + 54), 10), XStringFormats.CenterLeft);

                y += rowHeight;
            }

            if (!string.IsNullOrEmpty(po?.Payment_term))
            {
                CheckPageBreak(ref y);

                gfx.DrawString("Term of Payment:", calibriBold, XBrushes.Black, new XRect(x, y, 85, 10), XStringFormats.CenterLeft);

                var paymnetTerm = po?.Payment_term ?? string.Empty;
                gfx.DrawString(paymnetTerm, calibri, XBrushes.Black, new XRect(x + 85, y, page.Width - (x * 2 + 85), 10), XStringFormats.CenterLeft);

                y += rowHeight;
            }

            // Custom term
            var stripedCustomTerm = Regex.Replace(po?.Custom_term ?? string.Empty, "<.*?>", string.Empty);
            if (!string.IsNullOrEmpty(po?.Custom_term) && stripedCustomTerm != "")
            {
                y += 9;

                CheckPageBreak(ref y);

                var customTerm = po?.Custom_term;
                var (updatedPage, updatedGfx, updatedY) = HtmlToPdfHelper.RenderHtmlOnPages(
                    document, page, gfx, customTerm, x, y, page.Width - 86, calibri);

                page = updatedPage;
                gfx = updatedGfx;
                y = updatedY;
            }

            y += rowHeight * 2;

            CheckPageBreak(ref y);

            gfx.DrawString("Billing Address:", calibriBoldItalic, XBrushes.Black, new XRect(x, y, 200, 10), XStringFormats.CenterLeft);
            gfx.DrawString("Shipping Address:", calibriBoldItalic, XBrushes.Black, new XRect(300, y, 200, 10), XStringFormats.CenterLeft);

            y += rowHeight * 2;

            CheckPageBreak(ref y);

            gfx.DrawString("Faraday Factory Japan LLC", tnrBold, XBrushes.Black, new XRect(x, y, 150, 10), XStringFormats.CenterLeft);
            gfx.DrawString("Faraday Factory Japan LLC", tnrBold, XBrushes.Black, new XRect(300, y, 150, 10), XStringFormats.CenterLeft);

            y += rowHeight;

            var deliveryAddress = po?.Companies?.Address ?? po?.Companies?.Address_jpn ?? string.Empty;
            var deliveryTelephone = "\nTel.: " + (po?.Companies?.Telephone ?? string.Empty);
            var deliveryFax = "\nFax: " + (po?.Companies?.Fax ?? string.Empty);
            tf.DrawString($"{deliveryAddress}{deliveryTelephone}{deliveryFax}", calibri, XBrushes.Black, new XRect(300, y, 150, 76), XStringFormats.TopLeft);

            gfx.DrawString("FINE Bldg.,", calibri, XBrushes.Black, new XRect(x, y, 150, 10), XStringFormats.CenterLeft);

            y += rowHeight;

            gfx.DrawString("2956-6 Ishikawa-machi, Hachioji,", calibri, XBrushes.Black, new XRect(x, y, 150, 10), XStringFormats.CenterLeft);

            y += rowHeight;

            gfx.DrawString("Tokyo 192-0032, Japan", calibri, XBrushes.Black, new XRect(x, y, 150, 10), XStringFormats.CenterLeft);

            y += rowHeight;

            gfx.DrawString("TEL: +81-42-707-7077", calibri, XBrushes.Black, new XRect(x, y, 150, 10), XStringFormats.CenterLeft);

            y += rowHeight;

            gfx.DrawString("FAX: +81-42-707-9044", calibri, XBrushes.Black, new XRect(x, y, 150, 10), XStringFormats.CenterLeft);

            y += rowHeight * 4;

            CheckPageBreak(ref y);

            gfx.DrawString("Best regards,", calibri, XBrushes.Black, new XRect(x, y, 150, 10), XStringFormats.CenterLeft);

            y += rowHeight * 2;

            var correspondent = po?.Correspondents?.Correspondent_name ?? string.Empty;
            var possition = po?.Correspondents?.Correspondent_position ?? string.Empty;
            gfx.DrawString(correspondent, calibri, XBrushes.Black, new XRect(x, y, 150, 10), XStringFormats.CenterLeft);

            y += rowHeight;

            gfx.DrawString($"{possition},", calibri, XBrushes.Black, new XRect(x, y, 150, 10), XStringFormats.CenterLeft);

            y += rowHeight;

            gfx.DrawString("Faraday Factory Japan LLC", calibri, XBrushes.Black, new XRect(x, y, 150, 10), XStringFormats.CenterLeft);

            if (saveToFile)
            {
                // Save the document to file
                document.Save(po.File_path);
                document.Close();

                return null;
            }
            else
            {
                // Save to memory stream
                using (var stream = new MemoryStream())
                {
                    document.Save(stream, false);
                    document.Dispose();

                    return stream.ToArray();
                }
            }

            void CheckPageBreak(ref double y)
            {
                if (y + rowHeight >= page.Height.Point - bottomMargin)
                {
                    page = document.AddPage();
                    page.Size = PdfSharp.PageSize.A4;
                    gfx = XGraphics.FromPdfPage(page);
                    y = topMargin;
                }
            }
        }

        public static string GetCurrencyAmount(string currency, float? amount, bool includeSymbol = true)
        {
            if (amount == null)
                return string.Empty;

            CultureInfo culture;
            string format;

            switch (currency)
            {
                case "USD":
                    culture = new CultureInfo("en-US");
                    format = includeSymbol ? "C2" : "N2"; // $1,234.56 or 1,234.56
                    break;

                case "JPY":
                case "YEN":
                    culture = new CultureInfo("ja-JP");
                    format = includeSymbol ? "C0" : "N0"; // ¥1,235 or 1,235
                    break;

                default:
                    // Fallback: no symbol, standard number format
                    culture = CultureInfo.InvariantCulture;
                    format = "N2";
                    break;
            }

            return amount.Value.ToString(format, culture);
        }

        public byte[]? CreatePdfWithTable(string title, string[][] tableData)
        {
            // Create a new PDF document
            var document = new PdfDocument();
            document.Info.Title = title;

            // Add a page
            PdfPage page = document.AddPage();
            XGraphics gfx = XGraphics.FromPdfPage(page);

            // Define fonts
            XFont headerFont = new XFont("Arial", 12, XFontStyleEx.Bold);
            XFont cellFont = new XFont("Arial", 10);

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

        // Helper method to break text into lines
        private List<string> BreakTextIntoLines(XGraphics gfx, string text, XFont font, double maxWidth)
        {
            var words = text.Split(' ');
            var lines = new List<string>();
            string currentLine = "";

            foreach (var word in words)
            {
                var testLine = string.IsNullOrEmpty(currentLine) ? word : currentLine + " " + word;
                var size = gfx.MeasureString(testLine, font);

                if (size.Width > maxWidth)
                {
                    lines.Add(currentLine);
                    currentLine = word; // Start new line
                }
                else
                {
                    currentLine = testLine;
                }
            }

            if (!string.IsNullOrEmpty(currentLine))
            {
                lines.Add(currentLine);
            }

            return lines;
        }
    }
}
