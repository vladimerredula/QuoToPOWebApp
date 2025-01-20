using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using QouToPOWebApp.Models;
using QouToPOWebApp.Services;
using System.Text.RegularExpressions;

namespace QouToPOWebApp.Controllers
{
    public class ProcessController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly TabulaService _tabula;
        private readonly PdfPigService _pdfPig;

        public ProcessController(ApplicationDbContext dbContext, TabulaService tabula, PdfPigService pdfPig)
        {
            _db = dbContext;
            _tabula = tabula;
            _pdfPig = pdfPig;
        }

        public IActionResult Index()
        {
            ViewBag.pdfTypeList = new SelectList(_db.Pdf_types, "Pdf_type_ID", "Pdf_type_name");
            ViewBag.paymentTermList = new SelectList(_db.Payment_terms, "Payment_term_ID", "Payment_term_name");
            ViewBag.deliveryTermList = new SelectList(_db.Delivery_terms, "Delivery_term_ID", "Delivery_term_name");
            ViewBag.supplierList = GetSupplierList(null);

            return View();
        }

        public IActionResult ExtractText([Bind("File_path")] Quotation model)
        {
            //List<string> extractedTables = _tabula.LatticeModeExtraction(model.File_path);
            //var extractedText = ListToString(extractedTables);
            List<string> extractedTables = _pdfPig.ExtractText(model.File_path);
            var extractedText = ListToString(extractedTables);
            var cleanedText = extractedText.Replace("_", "").Replace(" ", "");

            model.Quotation_date = GetQuotationDate(cleanedText);
            model.Supplier_ID = GetSupplier(cleanedText);
            model.File_name = Path.GetFileName(model.File_path);
            model.Payment_term_ID = GetPaymentTerms(cleanedText);
            model.Delivery_term_ID = GetDeliveryTerms(cleanedText);

            ViewBag.pdfTypeList = new SelectList(_db.Pdf_types, "Pdf_type_ID", "Pdf_type_name");
            ViewBag.paymentTermList = new SelectList(_db.Payment_terms, "Payment_term_ID", "Payment_term_name", model.Payment_term_ID);
            ViewBag.deliveryTermList = new SelectList(_db.Delivery_terms, "Delivery_term_ID", "Delivery_term_name", model.Delivery_term_ID);
            ViewBag.supplierList = GetSupplierList(model.Supplier_ID);

            return View(model);
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

        public DateTime GetQuotationDate(string text)
        {
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
            MatchCollection matches = Regex.Matches(text, pattern);

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

        public SelectList GetSupplierList(int? selected)
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

        public string GetQuotationNumber(string text)
        {
            var quoNumber = string.Empty;

            return quoNumber;
        }

        public int? GetPaymentTerms(string text)
        {
            var paymentTerms = _db.Payment_terms.ToList();

            foreach (var paymentTerm in paymentTerms)
            {
                var keywords = paymentTerm.Key_words.Split(",");

                foreach (var keyword in keywords)
                {
                    if (text.Contains(keyword))
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
                    if (text.Contains(keyword))
                    {
                        return deliveryTerm.Delivery_term_ID;
                    }
                }
            }

            return null;
        }
    }
}
