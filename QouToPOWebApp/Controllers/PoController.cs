using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using QouToPOWebApp.Models.InfoModels;
using QouToPOWebApp.Models.MiscModels;
using QouToPOWebApp.Models.PoModels;
using QouToPOWebApp.Services;
using QouToPOWebApp.ViewModel;
using System.Security.Claims;
using System.Text.RegularExpressions;

namespace QouToPOWebApp.Controllers
{
    [Authorize]
    public class PoController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly string _poPath = Path.Combine(Directory.GetCurrentDirectory(), "AppData/PO");

        public PoController(ApplicationDbContext dbContext)
        {
            _db = dbContext;
        }

        public IActionResult Index(string path = "")
        {
            string fullPath = Path.Combine(_poPath, path);

            if (!Directory.Exists(fullPath))
                return NotFound("Directory not found!");

            var directories = Directory.GetDirectories(fullPath).Select(d => new DirectoryInfo(d));
            var files = Directory.GetFiles(fullPath).Select(f => new FileInfo(f));

            ViewBag.CurrentPath = path;
            ViewBag.Paths = GenerateBreadcrumbs(path);

            return View(directories.Concat<object>(files));
        }

        private List<(string Name, string Path)> GenerateBreadcrumbs(string path)
        {
            var breadcrumbs = new List<(string Name, string Path)> { ("PO", "") };
            if (string.IsNullOrEmpty(path)) return breadcrumbs;

            string[] parts = path.Split(Path.DirectorySeparatorChar, StringSplitOptions.RemoveEmptyEntries);
            string cumulativePath = "";
            foreach (var part in parts)
            {
                cumulativePath = Path.Combine(cumulativePath, part);
                breadcrumbs.Add((part, cumulativePath));
            }
            return breadcrumbs;
        }

        public IActionResult FromQuotation()
        {
            ViewData["pdfTypeList"] = new SelectList(_db.Pdf_types, "Pdf_type_ID", "Pdf_type_name");

            return View();
        }

        public IActionResult Extract(int pdfType, string filePath)
        {
            if (filePath == null || filePath.Length == 0)
            {
                return RedirectToAction(nameof(New));
            }

            switch (pdfType)
            {
                case 2:
                    return ExtractScannedPdf(filePath);
                default:
                    return ExtractNativePdf(filePath);
            }
        }

        public IActionResult ExtractNativePdf(string filePath)
        {
            var extractedTables = ExtractTables(filePath);

            var quoNumber = string.Empty;
            var quoDate = DateTime.MinValue;
            int? contactPersonID = null;
            var paymentTerm = new Payment_term();
            var deliveryTerm = new Delivery_term();
            bool isTaxable = false;

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

                if (!contactPersonID.HasValue)
                {
                    contactPersonID = GetContactPerson(cleanedText);
                }

                if (!isTaxable)
                {
                    isTaxable = IsTaxable(cleanedText);
                }

                paymentTerm ??= GetPaymentTerms(cleanedText);

                deliveryTerm ??= GetDeliveryTerms(cleanedText);
            }

            if (string.IsNullOrEmpty(quoNumber))
            {
                var tabulaJar = new TabulaJarService();
                var text = tabulaJar.ExtractTables(filePath, "stream");
                quoNumber = GetQuotationNumber(text);
            }

            if (string.IsNullOrEmpty(quoNumber))
            {
                var tabulaJar = new TabulaJarService();
                var text = tabulaJar.ExtractTables(filePath);
                quoNumber = GetQuotationNumber(text);
            }

            var model = new PoViewModel()
            {
                Po_items = new List<Po_item>(),
                Extract_mode = Request.Form["Extract_mode"]
            };

            model.Quotation_number = quoNumber;
            model.Po_date = quoDate;
            model.Po_number = quoDate.ToString("yyyyMMdd") + "/FF-000-" + GetPersonnelID().ToString("D3");
            model.Contact_person_ID = contactPersonID;
            model.Payment_term = model.Po_language == "en" ? paymentTerm?.Payment_term_name : paymentTerm?.Payment_term_name_jpn;
            model.Delivery_term = model.Po_language == "en" ? deliveryTerm?.Delivery_term_name : deliveryTerm?.Delivery_term_name_jpn;
            model.Delivery_address_ID = 1;
            model.File_path = filePath;
            model.File_name = Path.GetFileName(filePath);
            model.Include_tax = isTaxable;
            model.Pdf_type_ID = int.Parse(Request.Form["Pdf_type_ID"]);
            model.Extract_mode = Request.Form["Extract_mode"];

            model.Po_items = GetPoItems(filePath);

            ViewData["pdfTypeList"] = new SelectList(_db.Pdf_types, "Pdf_type_ID", "Pdf_type_name", model.Pdf_type_ID);
            ViewData["contactPersonList"] = GetContactPersonList(model.Contact_person_ID);
            ViewData["correspondentList"] = new SelectList(_db.Correspondents.ToList(), "Correspondent_ID", "Correspondent_name", model.Correspondent_ID);
            ViewBag.paymentTerms = _db.Payment_terms.ToList();
            ViewBag.deliveryTerms = _db.Delivery_terms.ToList();

            return View("ExtractText", model);
        }

        public IActionResult ExtractScannedPdf(string filePath)
        {
            var tess = new TesseractService(new PdfiumViewerService());

            var quoNumber = string.Empty;
            var quoDate = DateTime.MinValue;
            int? contactPersonID = null;
            var paymentTerm = new Payment_term();
            var deliveryTerm = new Delivery_term();

            var extractedText = tess.ExtractTextFromPdf(filePath);
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

            if (!contactPersonID.HasValue)
            {
                contactPersonID = GetContactPerson(cleanedText);
            }

            if (paymentTerm == null)
            {
                paymentTerm = GetPaymentTerms(cleanedText);
            }

            if (deliveryTerm == null)
            {
                deliveryTerm = GetDeliveryTerms(cleanedText);
            }

            var model = new PoViewModel()
            {
                Po_items = new List<Po_item>(),
                Extract_mode = Request.Form["Extract_mode"]
            };

            model.Quotation_number = quoNumber;
            model.Po_date = quoDate;
            model.Contact_person_ID = contactPersonID;
            model.Payment_term = model.Po_language == "en" ? paymentTerm?.Payment_term_name : paymentTerm?.Payment_term_name_jpn;
            model.Delivery_term = model.Po_language == "en" ? deliveryTerm?.Delivery_term_name : deliveryTerm?.Delivery_term_name_jpn;
            model.File_path = filePath;
            model.File_name = Path.GetFileName(filePath);
            model.Include_tax = IsTaxable(cleanedText);

            model.Po_items = GetPoItems(filePath);

            ViewData["pdfTypeList"] = new SelectList(_db.Pdf_types, "Pdf_type_ID", "Pdf_type_name", model.Pdf_type_ID);
            ViewData["deliveryAddressList"] = GetDeliveryAddressList(1);
            ViewData["contactPersonList"] = GetContactPersonList(model.Contact_person_ID);
            ViewData["correspondentList"] = new SelectList(_db.Correspondents.ToList(), "Correspondent_ID", "Correspondent_name", model.Correspondent_ID);
            ViewBag.paymentTerms = _db.Payment_terms.ToList();
            ViewBag.deliveryTerms = _db.Delivery_terms.ToList();

            return View("ExtractText", model);
        }

        public List<Po_item> GetPoItems(string filePath)
        {
            var extractionMode = Request.Form["Extract_mode"];

            var tabulaJar = new TabulaJarService();
            string extractedTables = tabulaJar.ExtractTables(filePath, extractionMode);

            var poItems = new List<Po_item>();
            var rows = extractedTables.Split("\n");

            var headerKeyWords = new List<(List<string> headerNames, int itemIndex)> {
                (new List<string> {"品名", "Discription", "Product", "Specification" }, 0),
                (new List<string> {"数量", "Quantity", "QTY", "Qnt" }, 1),
                (new List<string> {"単位", "Unit" }, 2),
                (new List<string> {"単価", "Price" }, 3)
            };

            var itemMarker = false;
            foreach (var row in rows)
            {
                if (headerKeyWords.Any(q => q.headerNames.Any(keyword => row.Replace(" ", "").Contains(keyword))))
                {
                    itemMarker = true;

                    var headers = row.Split(",");

                    for (int i = 0; i < headers.Length; i++)
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
                    var poItem = new Po_item();

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
                    var itemunit = headerKeyWords.SingleOrDefault(q => q.headerNames.Contains("単位"));
                    var itemprice = headerKeyWords.SingleOrDefault(q => q.headerNames.Contains("単価"));

                    if (result.Length > 2)
                    {
                        poItem.Item_name = result[itemname.itemIndex];

                        if (itemquantity.itemIndex != itemunit.itemIndex)
                        {
                            poItem.Unit = result[itemunit.itemIndex];
                        }

                        if (int.TryParse(Regex.Replace(result[itemquantity.itemIndex], @"[^0-9.]", ""), out int quantity))
                        {
                            poItem.Item_quantity = quantity;
                        }

                        if (float.TryParse(Regex.Replace(result[itemprice.itemIndex], @"[^0-9.]", ""), out float price) && result.Length > 2)
                        {
                            poItem.Item_price = price;
                        }
                    }

                    poItems.Add(poItem);
                }

            }

            return poItems;
        }

        public bool IsTaxable(string text)
        {
            text = CleanText(text);

            List<string> keyWords = [
                "消費税:",
                "消費税",
                "10%",
                "税込"
            ];

            foreach (var keyword in keyWords)
            {
                if (text.Contains(keyword.Replace(" ", "")))
                {
                    return true;
                }
            }

            return false;
        }

        public string GetQuotationNumber(string text)
        {
            text = CleanText(text);
            var quoNumber = string.Empty;

            List<string> keyWords = [
                "見積書番号:",
                "見積書番号",
                "伝票番号:",
                "伝票番号",
                "見積No",
                "PurchaseOrder:",
                "PurchaseOrder",
                "Ref:",
                "Ref",
                "No:"
            ];

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
                        quoNumber.Replace("\"", "").Replace(",", "");
                        break;
                    }
                }
            }

            return quoNumber;
        }

        public List<string> StringToTable(string text)
        {
            List<string> table = [.. text.Split("\n")];

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

        private static List<List<string>> ExtractTables(string filePath)
        {
            var pdfPig = new PdfPigService();
            var tabula = new TabulaService();

            return
            [
                tabula.StreamModeExtraction(filePath),
                tabula.LatticeModeExtraction(filePath),
                pdfPig.ExtractText(filePath)
            ];
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
                    + @"|(?<engFormat3>\b\d{1,2}(Jan|Feb|Mar|Apr|May|Jun|Jul|Aug|Sep|Oct|Nov|Dec)\d{4}\b)"
                    + @"|(?<engFormat4>\b\d{1,2}(Jan|Feb|Mar|Apr|May|Jun|Jul|Aug|Sep|Oct|Nov|Dec) \d{4}\b)";

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

        public int? GetContactPerson(string text)
        {
            var contactPersons = _db.Contact_persons.Include(s => s.Company).ToList();

            foreach (var contactPerson in contactPersons)
            {
                var keywords = new List<string>();

                if (contactPerson.Key_words != null)
                {
                    keywords = contactPerson.Key_words.Split(",").ToList();
                } else
                {
                    keywords.Add(contactPerson.Company.Company_name ?? contactPerson.Company.Company_name_jpn);
                }

                foreach (var keyword in keywords)
                {
                    if (text.Contains(keyword.Replace(" ", "")))
                    {
                        return contactPerson.Contact_person_ID;
                    }
                }
            }

            return null;
        }

        public SelectList GetContactPersonList(int? selected = null)
        {
            var contactPersonList = _db.Contact_persons
                .Select(s => new
                {
                    s.Contact_person_ID,
                    Supplier_name = s.Company.Company_name + (s.Contact_person_name != null ? ": " + s.Contact_person_name : s.Contact_person_name_jpn != null ? ": " + s.Contact_person_name_jpn : "")
                })
                .OrderBy(s => s.Supplier_name)
                .ToList();

            return new SelectList(contactPersonList, "Contact_person_ID", "Supplier_name", selected);
        }

        public Payment_term? GetPaymentTerms(string text)
        {
            var paymentTerms = _db.Payment_terms.ToList();

            foreach (var paymentTerm in paymentTerms)
            {
                var keywords = new List<string>();

                if (paymentTerm.Key_words != null)
                {
                    keywords = paymentTerm.Key_words.Split(",").ToList();
                }

                foreach (var keyword in keywords)
                {
                    if (text.Contains(CleanText(keyword)))
                    {
                        return paymentTerm;
                    }
                }
            }

            return null;
        }

        public Delivery_term? GetDeliveryTerms(string text)
        {
            var deliveryTerms = _db.Delivery_terms.ToList();

            foreach (var deliveryTerm in deliveryTerms)
            {
                var keywords = new List<string>();

                if (deliveryTerm.Key_words != null)
                {
                    keywords = deliveryTerm.Key_words.Split(",").ToList();
                }

                foreach (var keyword in keywords)
                {
                    if (text.Contains(CleanText(keyword)))
                    {
                        return deliveryTerm;
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

        [HttpGet]
        public IActionResult New()
        {
            ViewBag.deliveryAddressList = GetDeliveryAddressList();
            ViewBag.contactPersonList = GetContactPersonList();
            ViewBag.correspondentList = new SelectList(_db.Correspondents.ToList(), "Correspondent_ID", "Correspondent_name");
            ViewBag.paymentTerms = _db.Payment_terms.ToList();
            ViewBag.deliveryTerms = _db.Delivery_terms.ToList();

            return View();
        }

        [HttpPost]
        public IActionResult New(PoViewModel po)
        {
            ViewBag.deliveryAddressList = GetDeliveryAddressList();
            ViewBag.contactPersonList = GetContactPersonList();
            ViewBag.correspondentList = new SelectList(_db.Correspondents.ToList(), "Correspondent_ID", "Correspondent_name", po?.Correspondent_ID);
            ViewBag.paymentTerms = _db.Payment_terms.ToList();
            ViewBag.deliveryTerms = _db.Delivery_terms.ToList();

            return View(po);
        }

        public byte[]? GeneratePo(PoViewModel po, bool saveToFile = false)
        {
            po.Contact_persons = _db.Contact_persons
                .Include(s => s.Company)
                .FirstOrDefault(s => s.Contact_person_ID == po.Contact_person_ID);

            po.Companies = _db.Companies.FirstOrDefault(c => c.Company_ID == po.Delivery_address_ID);
            po.Correspondents = _db.Correspondents.FirstOrDefault(c => c.Correspondent_ID == po.Correspondent_ID);
            po.Email = User.FindFirst(ClaimTypes.Email)?.Value;

            var pdf = new PdfSharpService();

            return po.Po_language == "en" ? pdf.CreatePoEng(po, saveToFile) : pdf.CreatePo(po, saveToFile);
        }

        public IActionResult DownloadPo(PoViewModel po)
        {
            // Return the PDF file as a download
            return File(GeneratePo(po), "application/pdf", $"{po.Po_number}.pdf");
        }

        [HttpPost]
        public async Task<IActionResult> SavePoDraft(PoViewModel po)
        {
            var userId = GetPersonnelID(); // Fetch logged-in user ID

            if (po == null)
                return BadRequest("No data received.");

            po.File_name = DateTime.Now.ToString("yyyyMMdd");

            if (po.Contact_person_ID != null)
            {
                var cp = await _db.Contact_persons
                    .Include(c => c.Company)
                    .Where(c => c.Contact_person_ID == po.Contact_person_ID)
                    .FirstOrDefaultAsync();

                po.File_name += $" {(cp?.Company?.Company_name ?? po?.Po_number)}.pdf";
            } else
            {
                po.File_name += $" {po.Po_number}.pdf";
            }

            var poJson = JsonConvert.SerializeObject(po);

            var existingDraft = await _db.Po_drafts.FirstOrDefaultAsync(d => d.User_ID == userId && !d.Is_completed);

            if (existingDraft != null)
            {
                existingDraft.Po_data_json = poJson;
                existingDraft.Last_saved = DateTime.UtcNow;
            }
            else
            {
                var newDraft = new Po_draft
                {
                    User_ID = userId,
                    Po_data_json = poJson,
                    Last_saved = DateTime.UtcNow
                };
                await _db.Po_drafts.AddAsync(newDraft);
            }

            await _db.SaveChangesAsync();
            return Ok(new { message = "Draft saved." });
        }

        public int GetPersonnelID()
        {
            var personnelId = int.Parse(User.FindFirstValue("Personnelid") ?? "0");

            return personnelId;
        }

        [HttpGet]
        public async Task<IActionResult> GetPoDraft()
        {
            var userId = GetPersonnelID();
            var draft = await _db.Po_drafts
                .Where(d => d.User_ID == userId && !d.Is_completed)
                .OrderByDescending(d => d.Last_saved)
                .FirstOrDefaultAsync();

            if (draft != null)
                return Ok(new
                {
                    hasDraft = true,
                    draftId = draft.Draft_ID,
                    draftData = draft.Po_data_json
                });

            return Ok(new { hasDraft = false });
        }

        [HttpPost]
        public async Task<IActionResult> RemovePoDraft(int id)
        {
            var draft = await _db.Po_drafts.FindAsync(id);
            if (draft == null)
            {
                return NotFound(new { message = "No draft found." });
            }

            try
            {
                _db.Po_drafts.Remove(draft);
                await _db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unable to delete Po Draft: {id} - {ex}");
            }

            return Ok(new { message = "PO draft has been removed." });
        }

        [HttpPost]
        public IActionResult PreviewPo(PoViewModel po)
        {
            byte[] pdfData = GeneratePo(po);
            string base64Pdf = Convert.ToBase64String(pdfData);
            string dataUrl = "data:application/pdf;base64," + base64Pdf;

            return Json(new { PdfDataUrl = dataUrl });
        }

        public async Task<IActionResult> AddAttachments(PoViewModel model)
        {
            await SavePoDraft(model);

            var userId = GetPersonnelID();

            // Removed unused attachments in temp files
            var attachments = await _db.Attachments
                    .Where(a => a.User_ID == userId && a.Status == "pending")
                    .ToListAsync();

            if (attachments.Count > 0)
            {
                foreach (var attachment in attachments)
                {
                    // Delete files from temp folder
                    if (System.IO.File.Exists(attachment.File_path))
                        System.IO.File.Delete(attachment.File_path);

                    // Remove in DB
                    _db.Attachments.Remove(attachment);
                }

                await _db.SaveChangesAsync();
            }

            return View(model);
        }

        public async Task<IActionResult> Save()
        {
            try
            {
                var userId = GetPersonnelID();

                // Get PO draft
                var draft = await _db.Po_drafts
                    .Where(d => d.User_ID == userId && !d.Is_completed)
                    .OrderByDescending(d => d.Last_saved)
                    .FirstOrDefaultAsync();

                if (draft != null)
                {
                    // Get the number of saved Po within today
                    var savedPoCount = _db.File_groups
                        .Where(f => f.Date_created.Date == DateTime.Now.Date)
                        .Count();

                    var fileGroupDir = Path.Combine(_poPath, DateTime.Now.ToString("yyyy/MMMM/d"), $"{savedPoCount + 1}");

                    // Ensure the File group directory exists
                    if (!Directory.Exists(fileGroupDir))
                    {
                        Directory.CreateDirectory(fileGroupDir);
                    }

                    var fileGroup = new File_group
                    {
                        Date_created = DateTime.Now,
                        User_ID = userId,
                        Directory_path = fileGroupDir
                    };

                    _db.File_groups.Add(fileGroup);
                    await _db.SaveChangesAsync();

                    var poDataJson = JObject.Parse(draft.Po_data_json);

                    // Convert PO data JSON to PO model
                    // and PO view model for PO generation
                    var po = poDataJson.ToObject<Po>();
                    var poView = poDataJson.ToObject<PoViewModel>();
                    poView.File_path = Path.Combine(fileGroup.Directory_path, poView.File_name);
                    GeneratePo(poView, true);

                    po.File_group_ID = fileGroup.File_group_ID;
                    po.File_path = Path.Combine(fileGroup.Directory_path, po.File_name);

                    _db.Pos.Add(po);
                    await _db.SaveChangesAsync();

                    if (poDataJson["Po_items"] != null)
                    {
                        var poItems = poDataJson["Po_items"].ToObject<List<Po_item>>();

                        foreach (var poItem in poItems)
                        {
                            poItem.Po_ID = po.Po_ID;
                            _db.Po_items.Add(poItem);
                        }
                    }

                    // Get Attachments
                    var attachments = await _db.Attachments
                        .Where(a => a.User_ID == userId && a.Status == "pending")
                        .ToListAsync();

                    if (attachments.Count != 0)
                    {
                        foreach (var attachment in attachments)
                        {
                            var newFilePath = Path.Combine(fileGroup.Directory_path, attachment.File_name);

                            // Check if attachment file exists
                            if (System.IO.File.Exists(attachment.File_path))
                            {
                                // Copy the attachment file from temp folder to the file group folder
                                System.IO.File.Copy(attachment.File_path, newFilePath);

                                // Delete files from temp folder
                                System.IO.File.Delete(attachment.File_path);
                            }

                            attachment.File_path = newFilePath;
                            attachment.File_group_ID = fileGroup.File_group_ID;
                            attachment.Status = "saved";

                            _db.Update(attachment);
                        }

                        await _db.SaveChangesAsync();
                    }

                    _db.Po_drafts.Remove(draft);
                    await _db.SaveChangesAsync();
                }

                return RedirectToAction(nameof(Saved));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        public IActionResult Saved()
        {
            return View();
        }
    }
}
