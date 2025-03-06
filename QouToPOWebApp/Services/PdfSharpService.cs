using PdfSharp.Drawing;
using PdfSharp.Drawing.Layout;
using PdfSharp.Fonts;
using PdfSharp.Pdf;
using QouToPOWebApp.ViewModel;
using System.Globalization;

namespace QouToPOWebApp.Services
{
    public class PdfSharpService
    {
        public PdfSharpService()
        {
             var fontPaths = new Dictionary<string, string>
            {
                { "Meiryo", Path.Combine(Directory.GetCurrentDirectory(), "AppData/Fonts", "Meiryo.ttf") },
                { "Meiryo-bold", Path.Combine(Directory.GetCurrentDirectory(), "AppData/Fonts", "Meiryo-bold.ttf") },
                { "Calibri-regular", Path.Combine(Directory.GetCurrentDirectory(), "AppData/Fonts", "Calibri-regular.ttf") },
                { "Calibri-bold", Path.Combine(Directory.GetCurrentDirectory(), "AppData/Fonts", "Calibri-bold.ttf") },
                { "Calibri-bold-italic", Path.Combine(Directory.GetCurrentDirectory(), "AppData/Fonts", "Calibri-bold-italic.ttf") },
                { "Calibri-italic", Path.Combine(Directory.GetCurrentDirectory(), "AppData/Fonts", "Calibri-italic.ttf") },
                { "Arial", Path.Combine(Directory.GetCurrentDirectory(), "AppData/Fonts", "Arial.ttf") },
                { "Arial-bold", Path.Combine(Directory.GetCurrentDirectory(), "AppData/Fonts", "Arial-bold.ttf") },
                { "Arial-italic", Path.Combine(Directory.GetCurrentDirectory(), "AppData/Fonts", "Arial-italic.ttf") },
                { "Times new roman-bold", Path.Combine(Directory.GetCurrentDirectory(), "AppData/Fonts", "Times new roman-bold.ttf") }
            };

            // Set the global font resolver
            GlobalFontSettings.FontResolver = new FontResolver(fontPaths);
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
                Quotation_date = DateTime.Parse("2025-2-19"),
                Include_tax = true,
                Email = "sample@faradaygroup.com",
                Po_title = "Sample title",
                Payment_terms = new Models.Payment_term
                {
                    Payment_term_name = "Closed at the end of the month and paid at the end of the following month",
                    Payment_term_name_jpn = "月末締め翌月末支払い"
                },
                Delivery_terms = new Models.Delivery_term
                {
                    Delivery_term_name = "After consultation",
                    Delivery_term_name_jpn = "別途ご相談の上"
                },
                Companies = new Models.Company
                {
                    Address = "Zama factory, 5-12-15 Hibarigaoka, Zama, Kanagawa, 252-0003",
                    Address_jpn = "座間工場 〒252-0003 神奈川県座間市ひばりが丘5⁻12⁻15"
                },
                Contact_persons = new Models.Contact_person
                {
                    Contact_person_name = "△△  □□",
                    Contact_person_name_jpn = "△△  □□ ",
                    Company = new Models.Company
                    {
                        Company_name = "Company Sample Inc.",
                        Company_name_jpn = "株式会社サンプル　〇〇支社",
                        Address = "1-2-3 〇〇〇, Setagaya Ward, Tokyo",
                        Address_jpn = "東京都世田谷区〇〇〇 1-2-3",
                        Postal_code = "123-1234"
                    }
                },
                Quotation_items = new List<Models.Quotation_item>
                {
                    new Models.Quotation_item 
                    {
                        Item_name = "○○○○○○　サンプル　タイプＡ\nline1",
                        Item_price = 123456,
                        Item_quantity = 10,
                        Unit = "sets",
                        Order = 1
                    },
                    new Models.Quotation_item 
                    {
                        Item_name = "△△△△　システム機器\nline1\nline2\nline3",
                        Item_price = 2,
                        Item_quantity = 12345,
                        Unit = "pcs",
                        Order = 2
                    },
                    new Models.Quotation_item 
                    {
                        Item_name = "△△△△　システムの取付作業\nline1\nline2",
                        Item_price = 3,
                        Item_quantity = 30000,
                        Order = 3
                    },
                    new Models.Quotation_item 
                    {
                        Item_name = "△△△△　システムの操作説明　講習会\nline1\nline2",
                        Item_price = 40,
                        Item_quantity = 400,
                        Unit = "hours",
                        Order = 4
                    },
                    new Models.Quotation_item 
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
                Quotation_date = DateTime.Parse("2025-2-19"),
                Include_tax = true,
                Email = "sample@faradaygroup.com",
                Po_title = "Sample title",
                Payment_terms = new Models.Payment_term
                {
                    Payment_term_name = "Closed at the end of the month and paid at the end of the following month",
                    Payment_term_name_jpn = "月末締め翌月末支払い"
                },
                Delivery_terms = new Models.Delivery_term
                {
                    Delivery_term_name = "After consultation",
                    Delivery_term_name_jpn = "別途ご相談の上"
                },
                Companies = new Models.Company
                {
                    Address = "5-12-15 Hibarigaoka, Zama, Kanagawa 252-0003, Japan",
                    Address_jpn = "神奈川県座間市ひばりが丘5⁻12⁻15",
                    Postal_code = "252-0003"
                },
                Contact_persons = new Models.Contact_person
                {
                    Contact_person_name = "Luffy Taro",
                    Contact_person_name_jpn = "△△  □□ ",
                    Company = new Models.Company
                    {
                        Company_name = "Company Sample Inc.",
                        Company_name_jpn = "株式会社サンプル　〇〇支社",
                        Address = "1-2-3 ****, Setagaya Ward, Tokyo 123-1234, Japan",
                        Address_jpn = "東京都世田谷区〇〇〇 1-2-3",
                        Postal_code = "123-1234"
                    }
                },
                Correspondents = new Models.Correspondent
                {
                    Correspondent_name = "Monkey D. Dragon",
                    Correspondent_position = "CEO"
                },
                Quotation_items = new List<Models.Quotation_item>
                {
                    new Models.Quotation_item 
                    {
                        Item_name = "** Sample type A",
                        Item_price = 123456,
                        Item_quantity = 10,
                        Unit = "sets",
                        Order = 1
                    },
                    new Models.Quotation_item
                    {
                        Item_name = "System equipment",
                        Item_price = 2,
                        Item_quantity = 12345,
                        Unit = "pcs",
                        Order = 2
                    },
                    new Models.Quotation_item
                    {
                        Item_name = "System installation work",
                        Item_price = 3,
                        Item_quantity = 30000,
                        Order = 3
                    },
                    new Models.Quotation_item
                    {
                        Item_name = "System operation training session",
                        Item_price = 40,
                        Item_quantity = 400,
                        Unit = "hours",
                        Order = 4
                    },
                    new Models.Quotation_item
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

        public byte[] CreatePo(PoViewModel po)
        {
            // Create a new PDF document
            var document = NewDocument();
            document.Info.Title = $"PO for {po?.Contact_persons?.Company?.Company_name}"; // update title according to supplier

            // Add a page
            PdfPage page = document.AddPage();
            XGraphics gfx = XGraphics.FromPdfPage(page);
            XTextFormatter tf = new XTextFormatter(gfx);

            // Define table dimensions
            double x = 50; // Left margin
            double y = 65; // Top margin
            double rowHeight = 14;

            XColor ClayCreek = XColor.FromCmyk(0.00, 0.06, 0.36, 0.42);
            XFont headerFont = new XFont("Meiryo", 18.5);
            XFont bodyFont = new XFont("Meiryo", 10);
            XFont bodyFont2 = new XFont("Meiryo", 11);
            XBrush ClayCreekBrush = new XSolidBrush(ClayCreek);
            XPen ClayCreekPen = new XPen(ClayCreek, 2);

            // Header
            gfx.DrawString("発注書", headerFont, XBrushes.Black, new XRect(0, y, page.Width, rowHeight), XStringFormats.Center);

            // Approvers
            //gfx.DrawString("承認①", new XFont("Meiryo", 8), XBrushes.Black, new XRect(380, y-7, 87, 15), XStringFormats.Center);
            //gfx.DrawString("承認②", new XFont("Meiryo", 8), XBrushes.Black, new XRect(467, y-7, 87, 15), XStringFormats.Center);
            //gfx.DrawRectangle(new XPen(ClayCreek, 0.75), XBrushes.Transparent, 380, y+10, 87, 15);
            //gfx.DrawRectangle(new XPen(ClayCreek, 0.75), XBrushes.Transparent, 467, y+10, 87, 15);

            y += 51;

            //gfx.DrawRectangle(XPens.Black, XBrushes.LightGray, x, y, 50, rowHeight);
            gfx.DrawString("伝票番号：", bodyFont, XBrushes.Black, new XRect(x, y, 50, rowHeight), XStringFormats.CenterLeft);

            // PO number
            //gfx.DrawRectangle(XPens.Black, XBrushes.LightGray, 100, y, 100, rowHeight);
            gfx.DrawString(po?.Po_number ?? string.Empty, bodyFont, XBrushes.Black, new XRect(100, y, 100, rowHeight), XStringFormats.CenterLeft);

            // FFJ logo
            string imagePath = Path.Combine(Directory.GetCurrentDirectory(), "AppData/Images", "FFJ_LOGO.jpg");
            XImage image = XImage.FromFile(imagePath);
            gfx.DrawImage(image, 384, y+1, 175, 62);

            y += rowHeight;

            //gfx.DrawRectangle(XPens.Black, XBrushes.LightGray, x, y, 50, rowHeight);
            gfx.DrawString("発注日付：", bodyFont, XBrushes.Black, new XRect(x, y, 50, rowHeight), XStringFormats.Center);

            // Quotation date
            //gfx.DrawRectangle(XPens.Black, XBrushes.LightGray, 100, y, 100, rowHeight);
            gfx.DrawString(po?.Quotation_date?.ToString("yyyy年MM月dd日") ?? string.Empty, bodyFont, XBrushes.Black, new XRect(100, y, 100, rowHeight), XStringFormats.CenterLeft);

            y += 30;

            // Supplier Company name
            //gfx.DrawRectangle(XPens.Black, XBrushes.LightGray, 36, y+3, 300, 20);
            var supplierName = po?.Contact_persons?.Company?.Company_name_jpn ?? po?.Contact_persons?.Company?.Company_name ?? string.Empty;
            gfx.DrawString(supplierName, new XFont("Meiryo", 14), XBrushes.Black, new XRect(36, y, 300, 20), XStringFormats.Center);


            gfx.DrawString("御 中", bodyFont2, XBrushes.Black, new XRect(339, y+6, 30, rowHeight), XStringFormats.Center);

            y += 23;

            gfx.DrawLine(new XPen(ClayCreek, 1.5), 35, y, 339, y);

            y += 1;

            // Supplier Company address
            //gfx.DrawRectangle(XPens.Black, XBrushes.LightGray, 36, 194, 300, 12);
            var supplierAddress = po?.Contact_persons?.Company?.Address_jpn ?? po?.Contact_persons?.Company?.Address ?? string.Empty;
            var postalCode = po?.Contact_persons?.Company?.Postal_code ?? string.Empty;
            gfx.DrawString($"〒 {postalCode}　{supplierAddress}", new XFont("Meiryo", 9), XBrushes.Black, new XRect(36, y, 300, 12), XStringFormats.Center);

            // User details
            gfx.DrawRectangle(new XPen(ClayCreek, 0.75), XBrushes.Transparent, 370, y, 184, 95);
            //gfx.DrawRectangle(new XPen(ClayCreek, 0.75), XBrushes.Transparent, 385, y+10, 154, rowHeight);
            gfx.DrawString("Faraday Factory Japan合同会社", new XFont("Meiryo-bold", 9), XBrushes.Black, new XRect(385, y + 10, 154, rowHeight), XStringFormats.CenterLeft);

            //gfx.DrawRectangle(new XPen(ClayCreek, 0.75), XBrushes.Transparent, 385, y+rowHeight*2, 154, 10);
            gfx.DrawString("〒252-0003", new XFont("Meiryo", 7), XBrushes.Black, new XRect(385, y + rowHeight * 2, 154, 10), XStringFormats.CenterLeft);

            //gfx.DrawRectangle(new XPen(ClayCreek, 0.75), XBrushes.Transparent, 385, y+ rowHeight * 3, 154, 10);
            gfx.DrawString("神奈川県座間市ひばりが丘5⁻12⁻15", new XFont("Meiryo", 7), XBrushes.Black, new XRect(385, y + rowHeight * 3, 154, 10), XStringFormats.CenterLeft);

            //gfx.DrawRectangle(new XPen(ClayCreek, 0.75), XBrushes.Transparent, 385, y+ rowHeight * 4, 154, 10);
            gfx.DrawString("☎：03-4335-3000", new XFont("Meiryo", 7), XBrushes.Black, new XRect(385, y + rowHeight * 4, 154, 10), XStringFormats.CenterLeft);

            // Employee email
            var email = po?.Email ?? string.Empty;
            //gfx.DrawRectangle(new XPen(ClayCreek, 0.75), XBrushes.Transparent, 385, y+ rowHeight * 5, 154, 10);
            gfx.DrawString($"✉：{email}", new XFont("Meiryo", 8), XBrushes.Black, new XRect(385, y + rowHeight * 5, 154, 10), XStringFormats.CenterLeft);


            y += 12;

            // Supplier contact person name
            var contactPerson = po?.Contact_persons?.Contact_person_name_jpn ?? po?.Contact_persons?.Contact_person_name ?? "○○";
            gfx.DrawString($"担当者：{contactPerson} 様", bodyFont2, XBrushes.Black, new XRect(36, y, 300, rowHeight), XStringFormats.Center);

            y += rowHeight*2 + 3;

            // Message
            //gfx.DrawRectangle(new XPen(ClayCreek, 1), XBrushes.Transparent, 38, y, 150, rowHeight);
            gfx.DrawString("下記の通り発注申し上げます。", bodyFont2, XBrushes.Black, new XRect(38, y, 300, rowHeight), XStringFormats.CenterLeft);

            y += 23;

            //gfx.DrawRectangle(new XPen(ClayCreek, 1), XBrushes.Transparent, 31, y, 150, rowHeight);
            gfx.DrawString("【 合計金額 】", new XFont("Meiryo-bold", 16), ClayCreekBrush, new XRect(31, y, 150, rowHeight), XStringFormats.CenterLeft);

            y += rowHeight + 6;

            gfx.DrawLine(new XPen(ClayCreek, 0.75), 36, y+0.5, 338, y + 0.5);
            gfx.DrawLine(new XPen(ClayCreek, 0.75), 36, y+2, 338, y+2);

            // Tax included
            //gfx.DrawRectangle(new XPen(ClayCreek, 1), XBrushes.Transparent, 40, y-1, 85, rowHeight);
            gfx.DrawString("(" + (po.Include_tax ? "税込" : "税抜") + ")", new XFont("Meiryo", 8), XBrushes.Black, new XRect(40, y-1, 85, rowHeight), XStringFormats.Center);

            y += 20;

            var column1 = 290;
            var column2 = 55;
            var column3 = 85;
            var column4 = 90;
            var tableRowHeight = 26;

            //gfx.DrawRectangle(new XPen(ClayCreek, 0.75), new XSolidBrush(ClayCreek), 36, y, tableWitdh, 32);
            gfx.DrawRectangle(new XPen(ClayCreek, 0.75), new XSolidBrush(ClayCreek), 36, y, column1, 30);
            gfx.DrawString("商品名 ／ 品目", new XFont("Meiryo-bold", 12), XBrushes.White, new XRect(36, y, column1, 30), XStringFormats.Center);

            gfx.DrawRectangle(new XPen(ClayCreek, 0.75), new XSolidBrush(ClayCreek), 36 + column1, y, column2, 30);
            gfx.DrawString("数　量", new XFont("Meiryo-bold", 12), XBrushes.White, new XRect(36 + column1, y, column2, 30), XStringFormats.Center);

            gfx.DrawRectangle(new XPen(ClayCreek, 0.75), new XSolidBrush(ClayCreek), 36 + column1 + column2, y, column3, 30);
            gfx.DrawString("単　価", new XFont("Meiryo-bold", 12), XBrushes.White, new XRect(36 + column1 + column2, y, column3, 30), XStringFormats.Center);

            gfx.DrawRectangle(new XPen(ClayCreek, 0.75), new XSolidBrush(ClayCreek), 36 + column1 + column2 + column3, y, column4, 30);
            gfx.DrawString("金　額", new XFont("Meiryo-bold", 12), XBrushes.White, new XRect(36 + column1 + column2 + column3, y, column4, 30), XStringFormats.Center);

            y += 30;

            float totalAmount = 0;

            // PO title
            var title = po?.Po_title ?? string.Empty;

            if (title != string.Empty)
            {
                gfx.DrawString(title, new XFont("Meiryo-bold", 10), XBrushes.Black, new XRect(42, y, column1, tableRowHeight), XStringFormats.CenterLeft);
                gfx.DrawRectangle(new XPen(ClayCreek, 0.75), XBrushes.Transparent, 36, y, column1, tableRowHeight);
                gfx.DrawRectangle(new XPen(ClayCreek, 0.75), XBrushes.Transparent, 36 + column1, y, column2, tableRowHeight);
                gfx.DrawRectangle(new XPen(ClayCreek, 0.75), XBrushes.Transparent, 36 + column1 + column2, y, column3, tableRowHeight);
                gfx.DrawRectangle(new XPen(ClayCreek, 0.75), XBrushes.Transparent, 36 + column1 + column2 + column3, y, column4, tableRowHeight);

                y += tableRowHeight;
            }

            var y1 = y;

            if (po?.Quotation_items?.Count() > 0)
            {
                foreach (var item in po?.Quotation_items?.OrderBy(q => q.Order))
                {
                    var y2 = y1;
                    var currentRowHeight = 12;

                    var itemName = item?.Item_name?.Replace("\r", "").Split("\n");
                    foreach (var lineName in itemName)
                    {
                        gfx.DrawString(lineName, bodyFont, XBrushes.Black, new XRect(42, y2 + 7, column1, 10), XStringFormats.CenterLeft);
                        y2 += rowHeight;
                        currentRowHeight += 14;
                    }

                    //gfx.DrawString(item.Item_name, bodyFont, XBrushes.Black, new XRect(42, y1, column1, tableRowHeight), XStringFormats.CenterLeft);
                    gfx.DrawRectangle(new XPen(ClayCreek, 0.75), XBrushes.Transparent, 36, y1, column1, currentRowHeight);

                    gfx.DrawString(item?.Item_quantity?.ToString() + (item?.Unit != null ? " "+item.Unit : ""), bodyFont, XBrushes.Black, new XRect(36 + column1, y1, column2, currentRowHeight), XStringFormats.Center);
                    gfx.DrawRectangle(new XPen(ClayCreek, 0.75), XBrushes.Transparent, 36 + column1, y1, column2, currentRowHeight);

                    gfx.DrawString(item?.Item_price?.ToString("N0", new CultureInfo("ja-JP")), bodyFont, XBrushes.Black, new XRect(32 + column1 + column2, y1, column3, currentRowHeight), XStringFormats.CenterRight);
                    gfx.DrawRectangle(new XPen(ClayCreek, 0.75), XBrushes.Transparent, 36 + column1 + column2, y1, column3, currentRowHeight);

                    float totalprice = (float)(item.Item_price * item.Item_quantity);
                    gfx.DrawString(totalprice.ToString("N0", new CultureInfo("ja-JP")), bodyFont, XBrushes.Black, new XRect(32 + column1 + column2 + column3, y1, column4, currentRowHeight), XStringFormats.CenterRight);
                    gfx.DrawRectangle(new XPen(ClayCreek, 0.75), XBrushes.Transparent, 36 + column1 + column2 + column3, y1, column4, currentRowHeight);

                    totalAmount += totalprice;

                    y1 += currentRowHeight;
                }
            }

            y = y1;

            var itemCount = po.Quotation_items != null ? po.Quotation_items.Count() : 0;
            // Draw table rows
            for (int row = itemCount; row < 9; row++)
            {
                if (y < 600) // limit the number of blank rows when it reaches certain point
                {
                    gfx.DrawRectangle(new XPen(ClayCreek, 0.75), XBrushes.Transparent, 36, y, column1, tableRowHeight);

                    gfx.DrawRectangle(new XPen(ClayCreek, 0.75), XBrushes.Transparent, 36 + column1, y, column2, tableRowHeight);

                    gfx.DrawRectangle(new XPen(ClayCreek, 0.75), XBrushes.Transparent, 36 + column1 + column2, y, column3, tableRowHeight);

                    gfx.DrawRectangle(new XPen(ClayCreek, 0.75), XBrushes.Transparent, 36 + column1 + column2 + column3, y, column4, tableRowHeight);

                    y += tableRowHeight;
                }
            }

            y += 3;

            // Total item price
            //gfx.DrawRectangle(new XPen(ClayCreek, 0.75), XBrushes.Transparent, 35 + column1 + column2, y, 85, 26);
            gfx.DrawString("小　計 (税抜)", new XFont("Meiryo-bold", 12), XBrushes.Black, new XRect(33 + column1 + column2, y, 80, 26), XStringFormats.CenterRight);
            //gfx.DrawRectangle(new XPen(ClayCreek, 0.75), XBrushes.Transparent, 35 + column1 + column2 + column3, y, 87, 26);
            gfx.DrawString(totalAmount.ToString("C0", new CultureInfo("ja-JP")), new XFont("Meiryo-bold", 11), XBrushes.Black, new XRect(33 + column1 + column2 + column3, y, 87, 26), XStringFormats.CenterRight);

            y += 26;

            if (po.Include_tax)
            {
                float taxAmount = (float)(totalAmount * 0.1);

                // Total tax
                //gfx.DrawRectangle(new XPen(ClayCreek, 0.75), XBrushes.Transparent, 35 + column1 + column2, y, 85, 26);
                gfx.DrawString("消費税", new XFont("Meiryo-bold", 12), XBrushes.Black, new XRect(33 + column1 + column2, y, 40, 26), XStringFormats.CenterRight);
                gfx.DrawString("(10%)", new XFont("Meiryo-bold", 10), XBrushes.Black, new XRect(93 + column1 + column2, y, 20, 26), XStringFormats.CenterRight);
                //gfx.DrawRectangle(new XPen(ClayCreek, 0.75), XBrushes.Transparent, 35 + column1 + column2 + column3, y, 87, 26);
                gfx.DrawString(taxAmount.ToString("C0", new CultureInfo("ja-JP")), new XFont("Meiryo-bold", 11), XBrushes.Black, new XRect(33 + column1 + column2 + column3, y, 87, 26), XStringFormats.CenterRight);

                y += 26;

                totalAmount += taxAmount;
            }

            // Total amount
            //gfx.DrawRectangle(new XPen(ClayCreek, 1), XBrushes.Transparent, 181, y-4, 150, 20);
            gfx.DrawString(totalAmount.ToString("C0", new CultureInfo("ja-JP")) + "-", new XFont("Meiryo-bold", 16), XBrushes.Black, new XRect(181, 246, 150, 20), XStringFormats.CenterRight);

            // Total amount
            //gfx.DrawRectangle(new XPen(ClayCreek, 0.75), XBrushes.Transparent, 35 + column1 + column2, y, 85, 26);
            gfx.DrawString("合　計 (" + (po.Include_tax ? "税込" : "税抜") + ")", new XFont("Meiryo-bold", 12), XBrushes.Black, new XRect(33 + column1 + column2, y, 80, 26), XStringFormats.CenterRight);
            //gfx.DrawRectangle(new XPen(ClayCreek, 0.75), XBrushes.Transparent, 35 + column1 + column2 + column3, y, 87, 26);
            gfx.DrawString(totalAmount.ToString("C0", new CultureInfo("ja-JP")), new XFont("Meiryo-bold", 11), XBrushes.Black, new XRect(33 + column1 + column2 + column3, y, 87, 26), XStringFormats.CenterRight);

            y += 26;

            gfx.DrawLine(new XPen(ClayCreek, 3), 36 + column1, y, page.Width - 39, y);

            y += rowHeight + 5;

            // Delivery term
            gfx.DrawString("納期：", new XFont("Meiryo-bold", 10), XBrushes.Black, new XRect(36, y, 50, rowHeight), XStringFormats.CenterLeft);
            //gfx.DrawRectangle(XPens.Black, XBrushes.LightGray, 86, y, 300, rowHeight);
            var deliveryTerm = po?.Delivery_term ?? string.Empty;
            gfx.DrawString(deliveryTerm, bodyFont, XBrushes.Black, new XRect(66, y, 300, rowHeight), XStringFormats.CenterLeft);

            y += 18;

            // Payment term
            gfx.DrawString("決済条件：", new XFont("Meiryo-bold", 10), XBrushes.Black, new XRect(36, y, 50, rowHeight), XStringFormats.CenterLeft);
            //gfx.DrawRectangle(XPens.Black, XBrushes.LightGray, 86, y, 300, rowHeight);
            var paymentTerm = po?.Payment_term ?? string.Empty;
            gfx.DrawString(paymentTerm, bodyFont, XBrushes.Black, new XRect(86, y, 300, rowHeight), XStringFormats.CenterLeft);

            y += 18;

            // Delivery address
            gfx.DrawString("受渡場所：", new XFont("Meiryo-bold", 10), XBrushes.Black, new XRect(36, y, 50, rowHeight), XStringFormats.CenterLeft);
            //gfx.DrawRectangle(XPens.Black, XBrushes.LightGray, 86, y, 300, rowHeight);
            var deliveryAddress = po?.Companies?.Address_jpn ?? po?.Companies?.Address ?? string.Empty;
            gfx.DrawString(deliveryAddress, bodyFont, XBrushes.Black, new XRect(86, y, 300, rowHeight), XStringFormats.CenterLeft);

            y += rowHeight * 3;

            XPen customDashedPen = new XPen(ClayCreek, 1.3);
            customDashedPen.DashPattern = new double[] { 8, 3, 0.8, 3, 0.8, 3 };
            gfx.DrawLine(customDashedPen, 36, y, page.Width - 36, y);
            gfx.DrawLine(new XPen(ClayCreek, 2), 36, y + 5, page.Width - 36, y + 5);

            // Save to memory stream
            using (var stream = new MemoryStream())
            {
                document.Save(stream, false);
                document.Dispose();

                return stream.ToArray();
            }
        }

        public byte[] CreatePoEng(PoViewModel po)
        {
            // Create a new PDF document
            var document = NewDocument();
            var poTitle = po?.Po_title ?? string.Empty;
            document.Info.Title = poTitle;

            // Add a page
            PdfPage page = document.AddPage();
            XGraphics gfx = XGraphics.FromPdfPage(page);

            // Define table dimensions
            double x = 43; // X margin
            double y = 43; // Y margin
            double rowHeight = 13;

            XFont calibri = new XFont("Calibri-regular", 11);
            XFont calibriBold = new XFont("Calibri-bold", 11);
            XFont calibriBoldItalic = new XFont("Calibri-bold-italic", 11);
            XFont arial = new XFont("Arial", 10);
            XFont arialBold = new XFont("Arial-bold", 10);
            XFont arialItalic = new XFont("Arial-italic", 10);
            XFont tnrBold = new XFont("Times new roman-bold", 10.5);

            // FFJ logo
            string imagePath = Path.Combine(Directory.GetCurrentDirectory(), "AppData/Images", "FFJ_LOGO.jpg");
            XImage image = XImage.FromFile(imagePath);
            gfx.DrawImage(image, 58, y, 189, 71);

            //gfx.DrawRectangle(XPens.Black, XBrushes.Gray, 282, y, 150, 10);
            gfx.DrawString("Faraday Factory Japan LLC", tnrBold, XBrushes.Black, new XRect(282, y, 150, 10), XStringFormats.CenterLeft);

            y += rowHeight;

            //gfx.DrawRectangle(XPens.Black, XBrushes.Gray, 282, y, 150, 10);
            gfx.DrawString("FINE Bldg.,", calibri, XBrushes.Black, new XRect(282, y, 150, 10), XStringFormats.CenterLeft);

            y += rowHeight;

            //gfx.DrawRectangle(XPens.Black, XBrushes.Gray, 282, y, 150, 10);
            gfx.DrawString("2956-6 Ishikawa-machi, Hachioji,", calibri, XBrushes.Black, new XRect(282, y, 150, 10), XStringFormats.CenterLeft);

            y += rowHeight;

            //gfx.DrawRectangle(XPens.Black, XBrushes.Gray, 282, y, 150, 10);
            gfx.DrawString("Tokyo 192-0032, Japan", calibri, XBrushes.Black, new XRect(282, y, 150, 10), XStringFormats.CenterLeft);

            y += rowHeight;

            //gfx.DrawRectangle(XPens.Black, XBrushes.Gray, 282, y, 150, 10);
            gfx.DrawString("TEL: +81-42-707-7077", calibri, XBrushes.Black, new XRect(282, y, 150, 10), XStringFormats.CenterLeft);

            y += rowHeight;

            //gfx.DrawRectangle(XPens.Black, XBrushes.Gray, 282, y, 150, 10);
            gfx.DrawString("FAX: +81-42-707-9044", calibri, XBrushes.Black, new XRect(282, y, 150, 10), XStringFormats.CenterLeft);

            y += rowHeight;
            
            gfx.DrawLine(new XPen(XColors.Black, 3), 37, y, page.Width - 42, y);
            gfx.DrawLine(new XPen(XColors.Black, 1), 238, y + 3, page.Width - 42, y + 3);

            var date = (bool)(po?.Quotation_date.HasValue) ? po?.Quotation_date.Value.ToString("MMMM d, yyyy") : DateTime.Now.ToString("MMMM d, yyyy");
            //gfx.DrawRectangle(XPens.Black, XBrushes.Gray, 37, y+5, page.Width - 80, 10);
            gfx.DrawString(date, calibri, XBrushes.Black, new XRect(37, y + 5, page.Width - 80, 10), XStringFormats.CenterRight);

            y += 29;

            // Contact person
            var contactPerson = po?.Contact_persons?.Contact_person_name ?? po?.Contact_persons?.Contact_person_name_jpn ?? string.Empty;
            //gfx.DrawRectangle(XPens.Black, XBrushes.Gray, x, y, 200, 10);
            gfx.DrawString(contactPerson, arial, XBrushes.Black, new XRect(x, y, 200, 10), XStringFormats.CenterLeft);

            // Po number
            //gfx.DrawRectangle(XPens.Black, XBrushes.Gray, 300, y, 75, 10);
            gfx.DrawString("Purchase Order:", arialBold, XBrushes.Black, new XRect(300, y, 75, 10), XStringFormats.CenterLeft);
            gfx.DrawString(po?.Po_number ?? string.Empty, arial, XBrushes.Black, new XRect(383, y, 100, 10), XStringFormats.CenterLeft);

            y += rowHeight;

            // Supplier company name
            //gfx.DrawRectangle(XPens.Black, XBrushes.Gray, x, y, 200, 10);
            var supplierName = po?.Contact_persons?.Company?.Company_name ?? po?.Contact_persons?.Company?.Company_name_jpn ?? string.Empty;
            gfx.DrawString(supplierName, arial, XBrushes.Black, new XRect(x, y, 200, 10), XStringFormats.CenterLeft);

            y += rowHeight;

            // Reference
            //gfx.DrawRectangle(XPens.Black, XBrushes.Gray, 300, y, 25, 10);
            gfx.DrawString("Ref.:", arialBold, XBrushes.Black, new XRect(300, y, 25, 10), XStringFormats.CenterLeft);

            var quotationnumber = po?.Quotation_number ?? string.Empty;
            gfx.DrawString(quotationnumber, calibri, XBrushes.Black, new XRect(328, y, 100, 10), XStringFormats.CenterLeft);
            gfx.DrawString("Quotation for Faraday Factory Japan", arialItalic, XBrushes.Black, new XRect(328, y+rowHeight, 200, 10), XStringFormats.CenterLeft);

            XTextFormatter tf = new XTextFormatter(gfx);
            // Supplier Address
            //gfx.DrawRectangle(XPens.Black, XBrushes.Gray, x, y-3, 130, 66);
            var supplierAddress = po?.Contact_persons?.Company?.Address ?? po?.Contact_persons?.Company?.Address_jpn ?? string.Empty;
            var telephone = "\nTel.: " + (po?.Contact_persons?.Company?.Telephone ?? string.Empty);
            var fax = "\nFax: " + (po?.Contact_persons?.Company?.Fax ?? string.Empty);
            tf.DrawString($"{supplierAddress}{telephone}{fax}", arial, XBrushes.Black, new XRect(x, y-3, 130, 66), XStringFormats.TopLeft);

            y += rowHeight * 8;

            // Contact Person
            gfx.DrawString($"Dear {contactPerson},", arial, XBrushes.Black, new XRect(x, y - 6, 200, 10), XStringFormats.CenterLeft);

            y += rowHeight;

            // Message
            tf.DrawString($"Faraday Factory Japan LLC would like to place the Official Purchase Order for the {poTitle}, based on your Price Quotation {quotationnumber}.", calibri, XBrushes.Black, new XRect(x, y, page.Width - 85, 30), XStringFormats.TopLeft);

            y += rowHeight + 26;

            var column1 = 30;
            var column2 = 270;
            var column3 = 50;
            var column4 = 80;
            var column5 = 65;
            var tableWitdh = column1 + column2 + column3 + column4 + column5;

            //gfx.DrawRectangle(XPens.Black, XBrushes.Gray, x, y, column1, 10);
            gfx.DrawString(" No", calibriBold, XBrushes.Black, new XRect(x, y, column1, 10), XStringFormats.CenterLeft);

            //gfx.DrawRectangle(XPens.Black, XBrushes.Gray, x + column1, y, column2, 10);
            gfx.DrawString("Description", calibriBold, XBrushes.Black, new XRect(x + column1, y, column2, 10), XStringFormats.Center);

            //gfx.DrawRectangle(XPens.Black, XBrushes.Gray, x + column1 + column2, y, column3, 10);
            gfx.DrawString("QTY", calibriBold, XBrushes.Black, new XRect(x + column1 + column2, y, column3, 10), XStringFormats.Center);

            //gfx.DrawRectangle(XPens.Black, XBrushes.Gray, x + column1 + column2 + column3, y, column4, 10);
            gfx.DrawString("Unit Price", calibriBold, XBrushes.Black, new XRect(x + column1 + column2 + column3, y, column4, 10), XStringFormats.Center);

            //gfx.DrawRectangle(XPens.Black, XBrushes.Gray, x + column1 + column2 + column3 + column4, y, column5, 10);
            gfx.DrawString("Total", calibriBold, XBrushes.Black, new XRect(x + column1 + column2 + column3 + column4, y, column5, 10), XStringFormats.Center);

            y += rowHeight;

            gfx.DrawString("USD", calibriBold, XBrushes.Black, new XRect(x + column1 + column2 + column3, y, column4, 10), XStringFormats.Center);
            gfx.DrawString("USD", calibriBold, XBrushes.Black, new XRect(x + column1 + column2 + column3 + column4, y, column5, 10), XStringFormats.Center);

            y += rowHeight;

            gfx.DrawLine(new XPen(XColors.Black, 0.5), 37, y, tableWitdh + x, y);

            float totalAmount = 0;

            var y1 = y + 3;

            if (po?.Quotation_items?.Count() > 0)
            {
                foreach (var item in po?.Quotation_items?.OrderBy(q => q.Order))
                {
                    gfx.DrawString(item?.Order?.ToString(), calibri, XBrushes.Black, new XRect(x, y1, column1, rowHeight), XStringFormats.CenterLeft);

                    var y2 = y1;
                    double currentRowHeight = 0;

                    var itemName = item?.Item_name?.Replace("\r", "").Split("\n");
                    foreach (var lineName in itemName)
                    {
                        gfx.DrawString(lineName, calibri, XBrushes.Black, new XRect(x + column1, y2, column2, 10), XStringFormats.CenterLeft);
                        y2 += rowHeight;
                        currentRowHeight += rowHeight;
                    }

                    //gfx.DrawString(item.Item_name, calibri, XBrushes.Black, new XRect(x + column1, y1, column2, rowHeight), XStringFormats.CenterLeft);
                    gfx.DrawString(item?.Item_quantity?.ToString() + (item?.Unit != null ? " " + item?.Unit : ""), calibri, XBrushes.Black, new XRect(x + column1 + column2, y1, column3, rowHeight), XStringFormats.Center);
                    gfx.DrawString(item?.Item_price?.ToString("N2", new CultureInfo("en-US")), calibri, XBrushes.Black, new XRect(x + column1 + column2 + column3, y1, column4, rowHeight), XStringFormats.Center);

                    float totalprice = (float)(item?.Item_price * item?.Item_quantity);
                    gfx.DrawString(totalprice.ToString("N2", new CultureInfo("en-US")), calibri, XBrushes.Black, new XRect(x + column1 + column2 + column3 + column4, y1, column5, rowHeight), XStringFormats.Center);

                    totalAmount += totalprice;

                    y1 += currentRowHeight + 3;
                }
            }

            if (po.Include_tax)
            {
                y1 += rowHeight;

                float taxAmount = (float)(totalAmount * 0.1);

                // Total tax
                //gfx.DrawRectangle(new XPen(ClayCreek, 0.75), XBrushes.Transparent, x + column1, y1, column2, rowHeight);
                gfx.DrawString("Consumption tax 10%", calibri, XBrushes.Black, new XRect(x + column1, y1, column2, rowHeight), XStringFormats.CenterLeft);
                //gfx.DrawRectangle(new XPen(ClayCreek, 0.75), XBrushes.Transparent, x + column1, y1, column2, rowHeight);
                gfx.DrawString(taxAmount.ToString("N2", new CultureInfo("en-US")), calibri, XBrushes.Black, new XRect(x + column1 + column2 + column3 + column4, y1, column5, rowHeight), XStringFormats.Center);

                y1 += rowHeight;

                totalAmount += taxAmount;
            }

            y = y1 < 400 ? 400 : y1 + 5; // set the default height of table

            gfx.DrawLine(new XPen(XColors.Black, 0.5), 37, y, tableWitdh + x, y);

            y += rowHeight;

            gfx.DrawString("TOTAL:", calibriBold, XBrushes.Black, new XRect(x + column1 + column2, y, column3, 10), XStringFormats.Center);
            gfx.DrawString("USD", calibriBold, XBrushes.Black, new XRect(x + column1 + column2 + column3, y, column4, 10), XStringFormats.Center);
            gfx.DrawString(totalAmount.ToString("N2", new CultureInfo("en-US")), calibriBold, XBrushes.Black, new XRect(x + column1 + column2 + column3 + column4, y, column5, 10), XStringFormats.Center);

            y += rowHeight * 3;

            //gfx.DrawRectangle(XPens.Black, XBrushes.Gray, x, y, 54, 10);
            gfx.DrawString("Lead Time:", calibriBold, XBrushes.Black, new XRect(x, y, 54, 10), XStringFormats.CenterLeft);

            var deliveryTerm = po?.Delivery_term ?? string.Empty;
            gfx.DrawString(deliveryTerm, calibri, XBrushes.Black, new XRect(x + 54, y, page.Width - (x * 2 + 54), 10), XStringFormats.CenterLeft);

            y += rowHeight;

            //gfx.DrawRectangle(XPens.Black, XBrushes.Gray, x, y, 85, 10);
            gfx.DrawString("Term of Payment:", calibriBold, XBrushes.Black, new XRect(x, y, 85, 10), XStringFormats.CenterLeft);

            var paymnetTerm = po?.Payment_term ?? string.Empty;
            gfx.DrawString(paymnetTerm, calibri, XBrushes.Black, new XRect(x + 85, y, page.Width - (x * 2 + 85), 10), XStringFormats.CenterLeft);

            y += rowHeight * 3;

            //gfx.DrawRectangle(XPens.Black, XBrushes.Gray, x, y, 200, 10);
            gfx.DrawString("Billing Address:", calibriBoldItalic, XBrushes.Black, new XRect(x, y, 200, 10), XStringFormats.CenterLeft);

            //gfx.DrawRectangle(XPens.Black, XBrushes.Gray, 300, y, 200, 10);
            gfx.DrawString("Shipping Address:", calibriBoldItalic, XBrushes.Black, new XRect(300, y, 200, 10), XStringFormats.CenterLeft);

            y += rowHeight * 2;

            //gfx.DrawRectangle(XPens.Black, XBrushes.Gray, x, y, 150, 10);
            gfx.DrawString("Faraday Factory Japan LLC", tnrBold, XBrushes.Black, new XRect(x, y, 150, 10), XStringFormats.CenterLeft);

            //gfx.DrawRectangle(XPens.Black, XBrushes.Gray, x, y, 150, 10);
            gfx.DrawString("Faraday Factory Japan LLC", tnrBold, XBrushes.Black, new XRect(300, y, 150, 10), XStringFormats.CenterLeft);

            y += rowHeight;

            var deliveryAddress = po?.Companies?.Address ?? po?.Companies?.Address_jpn ?? string.Empty;
            var deliveryTelephone = "\nTel.: " + (po?.Companies?.Telephone ?? string.Empty);
            var deliveryFax = "\nFax: " + (po?.Companies?.Fax ?? string.Empty);
            //gfx.DrawRectangle(XPens.Black, XBrushes.Gray, 300, y, 150, 76);
            tf.DrawString($"{deliveryAddress}{deliveryTelephone}{deliveryFax}", calibri, XBrushes.Black, new XRect(300, y, 150, 76), XStringFormats.TopLeft);

            //gfx.DrawRectangle(XPens.Black, XBrushes.Gray, x, y, 150, 10);
            gfx.DrawString("FINE Bldg.,", calibri, XBrushes.Black, new XRect(x, y, 150, 10), XStringFormats.CenterLeft);

            y += rowHeight;

            //gfx.DrawRectangle(XPens.Black, XBrushes.Gray, x, y, 150, 10);
            gfx.DrawString("2956-6 Ishikawa-machi, Hachioji,", calibri, XBrushes.Black, new XRect(x, y, 150, 10), XStringFormats.CenterLeft);

            y += rowHeight;

            //gfx.DrawRectangle(XPens.Black, XBrushes.Gray, x, y, 150, 10);
            gfx.DrawString("Tokyo 192-0032, Japan", calibri, XBrushes.Black, new XRect(x, y, 150, 10), XStringFormats.CenterLeft);

            y += rowHeight;

            //gfx.DrawRectangle(XPens.Black, XBrushes.Gray, x, y, 150, 10);
            gfx.DrawString("TEL: +81-42-707-7077", calibri, XBrushes.Black, new XRect(x, y, 150, 10), XStringFormats.CenterLeft);

            y += rowHeight;

            //gfx.DrawRectangle(XPens.Black, XBrushes.Gray, x, y, 150, 10);
            gfx.DrawString("FAX: +81-42-707-9044", calibri, XBrushes.Black, new XRect(x, y, 150, 10), XStringFormats.CenterLeft);

            y += rowHeight * 4;

            gfx.DrawString("Best regards,", calibri, XBrushes.Black, new XRect(x, y, 150, 10), XStringFormats.CenterLeft);

            y += rowHeight * 2;

            var correspondent = po?.Correspondents?.Correspondent_name ?? string.Empty;
            var possition = po?.Correspondents?.Correspondent_position ?? string.Empty;
            gfx.DrawString(correspondent, calibri, XBrushes.Black, new XRect(x, y, 150, 10), XStringFormats.CenterLeft);

            y += rowHeight;

            gfx.DrawString($"{possition},", calibri, XBrushes.Black, new XRect(x, y, 150, 10), XStringFormats.CenterLeft);

            y += rowHeight;

            gfx.DrawString("Faraday Factory Japan LLC", calibri, XBrushes.Black, new XRect(x, y, 150, 10), XStringFormats.CenterLeft);


            // Save to memory stream
            using (var stream = new MemoryStream())
            {
                document.Save(stream, false);
                document.Dispose();

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
