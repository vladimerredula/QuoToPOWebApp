using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using QouToPOWebApp.Models;
using QouToPOWebApp.Services;
using QouToPOWebApp.ViewModel;
using System.Linq;
using System.Text.RegularExpressions;

namespace QouToPOWebApp.Controllers
{
    public class PoController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly TabulaService _tabula;
        private readonly PdfPigService _pdfPig;
        private readonly TabulaJarService _tabulaJar;

        public PoController(ApplicationDbContext dbContext, TabulaService tabula, PdfPigService pdfPig)
        {
            _db = dbContext;
            _tabula = tabula;
            _pdfPig = pdfPig;
            _tabulaJar = new TabulaJarService();
        }

        public IActionResult CreatePoFromQuotation()
        {
            ViewData["pdfTypeList"] = new SelectList(_db.Pdf_types, "Pdf_type_ID", "Pdf_type_name");
            ViewData["paymentTermList"] = new SelectList(_db.Payment_terms, "Payment_term_ID", "Payment_term_name");
            ViewData["deliveryTermList"] = new SelectList(_db.Delivery_terms, "Delivery_term_ID", "Delivery_term_name");
            ViewData["deliveryAddressList"] = GetDeliveryAddressList();
            ViewData["supplierList"] = GetSupplierList();

            return View();
        }

        public IActionResult Extract(QuotationViewModel model)
        {
            switch (model.Quotation.Pdf_type_ID)
            {
                case 2:
                    return ExtractScannedPdf(model);
                default:
                    return ExtractNativePdf(model);
            }
        }

        public IActionResult ExtractNativePdf(QuotationViewModel model)
        {
            var extractedTables = ExtractTables(model.Quotation.File_path);

            var quoNumber = string.Empty;
            var quoDate = DateTime.MinValue;
            int? supplierID = null;
            int? paymentTerm = null;
            int? deliveryTerm = null;

            foreach (var tables in extractedTables)
            {
                var extractedText = ListToString(tables);
                var cleanedText = extractedText.Replace("_", "").Replace(" ", "");

                if (string.IsNullOrEmpty(quoNumber))
                {
                    quoNumber = GetQuotationNumber(extractedText);
                }

                if (quoDate == DateTime.MinValue)
                {
                    quoDate = ParseQuotationDate(cleanedText);
                }

                if (!supplierID.HasValue)
                {
                    supplierID = GetSupplier(cleanedText);
                }

                if (!paymentTerm.HasValue)
                {
                    paymentTerm = GetPaymentTerms(cleanedText);
                }

                if (!deliveryTerm.HasValue)
                {
                    deliveryTerm = GetDeliveryTerms(cleanedText);
                }
            }

            if (string.IsNullOrEmpty(quoNumber))
            {
                var text = _tabulaJar.ExtractTables(model.Quotation.File_path, "stream");
                quoNumber = GetQuotationNumber(text);
            }

            if (string.IsNullOrEmpty(quoNumber))
            {
                var text = _tabulaJar.ExtractTables(model.Quotation.File_path);
                quoNumber = GetQuotationNumber(text);
            }

            model.Quotation.Quotation_number = quoNumber;
            model.Quotation.Quotation_date = quoDate;
            model.Quotation.Supplier_ID = supplierID;
            model.Quotation.Payment_term_ID = paymentTerm;
            model.Quotation.Delivery_term_ID = deliveryTerm;
            model.Quotation.File_name = Path.GetFileName(model.Quotation.File_path);

            model.Items = GetQuotationItems(model.Quotation.File_path);

            ViewData["pdfTypeList"] = new SelectList(_db.Pdf_types, "Pdf_type_ID", "Pdf_type_name", model.Quotation.Pdf_type_ID);
            ViewData["paymentTermList"] = new SelectList(_db.Payment_terms, "Payment_term_ID", "Payment_term_name", model.Quotation.Payment_term_ID);
            ViewData["deliveryTermList"] = new SelectList(_db.Delivery_terms, "Delivery_term_ID", "Delivery_term_name", model.Quotation.Delivery_term_ID);
            ViewData["deliveryAddressList"] = GetDeliveryAddressList(model.Quotation.Delivery_term_ID);
            ViewData["supplierList"] = GetSupplierList(model.Quotation.Supplier_ID);

            return View("ExtractText", model);
        }

        public IActionResult ExtractScannedPdf(QuotationViewModel model)
        {
            var tess = new TesseractService(new PdfiumViewerService());

            var quoNumber = string.Empty;
            var quoDate = DateTime.MinValue;
            int? supplierID = null;
            int? paymentTerm = null;
            int? deliveryTerm = null;

            var extractedText = tess.ExtractTextFromPdf(model.Quotation.File_path);
            //var extractedText = tess.ExtractTextFromImage("C:\\Users\\v.redula\\OneDrive - Faraday Factory Japan\\Pictures\\Screenshots\\Screenshot 2025-01-29 173521.png");
            var cleanedText = extractedText.Replace("_", "").Replace(" ", "");

            if (string.IsNullOrEmpty(quoNumber))
            {
                quoNumber = GetQuotationNumber(extractedText);
            }

            if (quoDate == DateTime.MinValue)
            {
                quoDate = ParseQuotationDate(cleanedText);
            }

            if (!supplierID.HasValue)
            {
                supplierID = GetSupplier(cleanedText);
            }

            if (!paymentTerm.HasValue)
            {
                paymentTerm = GetPaymentTerms(cleanedText);
            }

            if (!deliveryTerm.HasValue)
            {
                deliveryTerm = GetDeliveryTerms(cleanedText);
            }

            model.Quotation.Quotation_number = quoNumber;
            model.Quotation.Quotation_date = quoDate;
            model.Quotation.Supplier_ID = supplierID;
            model.Quotation.Payment_term_ID = paymentTerm;
            model.Quotation.Delivery_term_ID = deliveryTerm;
            model.Quotation.File_name = Path.GetFileName(model.Quotation.File_path);

            model.Items = GetQuotationItems(model.Quotation.File_path);

            ViewData["pdfTypeList"] = new SelectList(_db.Pdf_types, "Pdf_type_ID", "Pdf_type_name", model.Quotation.Pdf_type_ID);
            ViewData["paymentTermList"] = new SelectList(_db.Payment_terms, "Payment_term_ID", "Payment_term_name", model.Quotation.Payment_term_ID);
            ViewData["deliveryTermList"] = new SelectList(_db.Delivery_terms, "Delivery_term_ID", "Delivery_term_name", model.Quotation.Delivery_term_ID);
            ViewData["deliveryAddressList"] = GetDeliveryAddressList(model.Quotation.Delivery_term_ID);
            ViewData["supplierList"] = GetSupplierList(model.Quotation.Supplier_ID);

            return View("ExtractText", model);
        }

        public List<Quotation_item> GetQuotationItems(string filePath)
        {
            var extractionMode = Request.Form["extractionMode"];
            string extractedTables = _tabulaJar.ExtractTables(filePath, extractionMode);

            var quotationItems = new List<Quotation_item>();
            var rows = extractedTables.Split("\n");

            var headerKeyWords = new List<(List<string> headerNames, int itemIndex)> {
                (new List<string> {"品名", "Discription", "Product", "Specification" }, 0),
                (new List<string> {"数量", "Quantity", "QTY", "Qnt" }, 1),
                (new List<string> {"単価", "Price" }, 2)
            };

            var itemMarker = false;
            foreach (var row in rows)
            {
                if (headerKeyWords.Any(q => q.headerNames.Any(keyword => row.Replace(" ", "").Contains(keyword))))
                {
                    itemMarker = true;

                    var headers = row.Split(",");

                    for (int i = 0; i < headers.Count(); i++)
                    {
                        var keyWord = Regex.Replace(headers[i], @"[\r\n\t,""]", "").Replace(" ","");
                        for (int j = 0; j < headerKeyWords.Count(); j++)
                        {
                            foreach (var keyword in headerKeyWords[j].headerNames)
                            {
                                if (keyWord.Contains(keyword))
                                {
                                    headerKeyWords[j] = (headerKeyWords[j].headerNames, i);
                                    break;
                                }
                            }
                        }
                    }

                    continue;
                }

                var samp = Regex.Replace(row, @"[\r\n\t,""]", "");
                if (Regex.Replace(row, @"[\r\n\t,""]", "") == "")
                {
                    itemMarker = false;
                }

                if (itemMarker)
                {
                    var quotationItem = new Quotation_item();

                    var sample = Regex.Replace(row, @"[\r\n\t]", "");

                    // Regular expression to split the string by commas, ignoring commas inside quotes
                    string pattern = ",(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)";
                    string[] result = Regex.Split(sample, pattern);

                    // Trim any surrounding quotes from each part
                    for (int i = 0; i < result.Length; i++)
                    {
                        result[i] = result[i].Trim('"');
                    }

                    var itemname = headerKeyWords.SingleOrDefault(q => q.headerNames.Contains("品名"));
                    var itemquantity = headerKeyWords.SingleOrDefault(q => q.headerNames.Contains("数量"));
                    var itemprice = headerKeyWords.SingleOrDefault(q => q.headerNames.Contains("単価"));

                    if (result.Length > 2)
                    {
                        quotationItem.Item_name = result[itemname.itemIndex];

                        int quantity;
                        if (int.TryParse(Regex.Replace(result[itemquantity.itemIndex], @"[^0-9.]", ""), out quantity))
                        {
                            quotationItem.Item_quantity = quantity;
                        }

                        float price;
                        if (float.TryParse(Regex.Replace(result[itemprice.itemIndex], @"[^0-9.]", ""), out price) && result.Length > 2)
                        {
                            quotationItem.Item_price = price;
                        }
                    }

                    quotationItems.Add(quotationItem);
                }

            }

            return quotationItems;
        }

        public string GetQuotationNumber(string text)
        {
            text = CleanText(text);
            var quoNumber = string.Empty;

            List<string> keyWords = new List<string> {
                "見積書番号:",
                "見積書番号",
                "伝票番号:",
                "伝票番号",
                "PurchaseOrder:",
                "PurchaseOrder",
                "Ref:",
                "Ref"
            };

            foreach (var keyWord in keyWords)
            {
                int startIndex = text.IndexOf(keyWord);

                if (startIndex != -1)
                {

                    // Adjust start index to be after the search string
                    startIndex += keyWord.Length;

                    int endIndex = text.IndexOf("\n", startIndex);
                    if (endIndex != -1)
                    {
                        quoNumber = text.Substring(startIndex, endIndex - startIndex).Trim();
                        quoNumber.Replace("\"", "");
                        quoNumber.Replace(",", "");
                        break;
                    }
                }
            }

            return quoNumber;
        }

        public List<string> StringToTable(string text)
        {
            List<string> table = new List<string>();

            foreach (var row in text.Split("\n"))
            {
                table.Add(row);
            }

            return table;
        }

        public string ListToString(List<string> extractedTables)
        {
            string analyzedText = string.Empty;

            foreach (var text in extractedTables)
            {
                analyzedText += text;
            }

            return analyzedText;
        }

        private List<List<string>> ExtractTables(string filePath)
        {
            return new List<List<string>>
            {
                _tabula.StreamModeExtraction(filePath),
                _tabula.LatticeModeExtraction(filePath),
                _pdfPig.ExtractText(filePath)
            };
        }

        public DateTime ParseQuotationDate(string text)
        {
            var cleanedText = text.Replace("_", "").Replace(" ", "");

            // Regex pattern to find dates in yyyy.MM.dd format
            string pattern = @"(?<jpnFormat1>\b\d{4}年\d{1,2}月\d{1,2}日\b)"
                    + @"|(?<jpnFormat2>(\d{4})年\s*(\d{1,2})月\s*(\d{1,2})日)"
                    + @"|(?<jpnFormat2>(\d{4})\s*年\s*(\d{1,2})\s*月\s*(\d{1,2})\s*日)"
                    + @"|(?<jpnFormat3>令和(?<year>\d{2})年(?<month>\d{2})月(?<day>\d{2})日)"
                    + @"|(?<jpnFormat4>令\s*和\s*(?<year>\d{1,2})\s*年\s*(?<month>\d{1,2})\s*月\s*(?<day>\d{1,2}))"
                    + @"|(?<dotSeparated>\b\d{4}\.\d{1,2}\.\d{1,2}\b)"
                    + @"|(?<slashSeparated>\b\d{1,2}/\d{1,2}/\d{2,4}\b)"
                    + @"|(?<dashSeparated>\b\d{4}-\d{1,2}-\d{1,2}\b)"
                    + @"|(?<engFormat1>\b(January|February|March|April|May|June|July|August|September|October|November|December) \d{1,2}, \d{4}\b)"
                    + @"|(?<engFormat2>\b\d{1,2} (January|February|March|April|May|June|July|August|September|October|November|December) \d{4}\b)"
                    + @"|(?<engFormat3>\b\d{1,2}(Jan|Feb|Mar|Apr|May|Jun|Jul|Aug|Sep|Oct|Nov|Dec) \d{4}\b)";

            // Match the pattern in the extracted text
            MatchCollection matches = Regex.Matches(cleanedText, pattern);

            // Add each matched date to the list
            foreach (Match match in matches)
            {
                if (match.Groups["jpnFormat3"].Success || match.Groups["jpnFormat4"].Success)
                {
                    // Extract Reiwa year, month, and day
                    int reiwaYear = int.Parse(match.Groups["year"].Value);
                    int month = int.Parse(match.Groups["month"].Value);
                    int day = int.Parse(match.Groups["day"].Value);

                    // Convert Reiwa year to Gregorian year
                    int gregorianYear = 2018 + reiwaYear; // Reiwa era started in 2019

                    return DateTime.Parse($"{gregorianYear}-{month:D2}-{day:D2}");
                }

                if (match.Groups["jpnFormat1"].Success || match.Groups["jpnFormat2"].Success)
                {
                    return DateTime.Parse(match.Value);
                }
            }

            if (matches.Count() > 0)
            {
                return DateTime.Parse(matches[0].Value);
            }

            return DateTime.MinValue;
        }

        public int? GetSupplier(string text)
        {
            var suppliers = _db.Suppliers.ToList();

            foreach (var supplier in suppliers)
            {
                var keywords = supplier.Key_words.Split(",");

                foreach (var keyword in keywords)
                {
                    if (text.Contains(keyword.Replace(" ", "")))
                    {
                        return supplier.Supplier_ID;
                    }
                }
            }

            return null;
        }

        public SelectList GetSupplierList(int? selected = null)
        {
            var supplierList = _db.Suppliers
                .Select(s => new
                {
                    s.Supplier_ID,
                    Supplier_name = s.Company.Company_name
                })
                .OrderBy(s => s.Supplier_name)
                .ToList();

            return new SelectList(supplierList, "Supplier_ID", "Supplier_name", selected);
        }

        public int? GetPaymentTerms(string text)
        {
            var paymentTerms = _db.Payment_terms.ToList();

            foreach (var paymentTerm in paymentTerms)
            {
                var keywords = paymentTerm.Key_words.Split(",");

                foreach (var keyword in keywords)
                {
                    if (text.Contains(CleanText(keyword)))
                    {
                        return paymentTerm.Payment_term_ID;
                    }
                }
            }

            return null;
        }

        public int? GetDeliveryTerms(string text)
        {
            var deliveryTerms = _db.Delivery_terms.ToList();

            foreach (var deliveryTerm in deliveryTerms)
            {
                var keywords = deliveryTerm.Key_words.Split(",");

                foreach (var keyword in keywords)
                {
                    if (text.Contains(CleanText(keyword)))
                    {
                        return deliveryTerm.Delivery_term_ID;
                    }
                }
            }

            return null;
        }

        public List<Company> GetDeliveryAddress()
        {
            var list = _db.Companies.Where(c => c.Company_ID < 5).ToList();

            return list;
        }

        public SelectList GetDeliveryAddressList(int? selected = null)
        {
            return new SelectList(GetDeliveryAddress(), "Company_ID", "Company_name", selected);
        }

        public string CleanText(string text)
        {
            return text.Replace("_", "").Replace(" ", "");
        }

        public IActionResult CreatePo()
        {
            ViewBag.deliveryAddressList = GetDeliveryAddressList();
            ViewBag.supplierList = GetSupplierList();
            ViewBag.paymentTerms = _db.Payment_terms.ToList();
            return View();
        }
    }
}
