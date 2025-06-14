﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QouToPOWebApp.Models.InfoModels;

namespace QouToPOWebApp.Controllers
{
    [Authorize]
    public class InfoController : Controller
    {
        private readonly ApplicationDbContext _db;

        public InfoController(ApplicationDbContext context)
        {
            _db = context;
        }

        public IActionResult Index()
        {
            return RedirectToAction(nameof(Company));
        }

        #region Company Functions
        // GET: Company
        public async Task<IActionResult> Company()
        {
            ViewData["companies"] = await _db.Companies.ToListAsync();
            return View();
        }

        // POST: Info/CreateCompany
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCompany([Bind("Company_name,Company_name_jpn,Address,Address_jpn,Postal_code,Telephone,Fax")] Company company)
        {
            if (ModelState.IsValid)
            {
                _db.Add(company);
                await _db.SaveChangesAsync();

                TempData["toastMessage"] = "Successfully created Company!-success";
                return RedirectToAction(nameof(Company));
            }

            ViewData["companies"] = await _db.Companies.ToListAsync();
            return View(nameof(Company), company);
        }

        // POST: Info/EditCompany/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCompany(int id, [Bind("Company_ID,Company_name,Company_name_jpn,Address,Address_jpn,Postal_code,Telephone,Fax")] Company company)
        {
            if (id != company.Company_ID)
            {
                TempData["toastMessage"] = "Company not found!-danger";
                return RedirectToAction(nameof(Company));
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _db.Update(company);
                    await _db.SaveChangesAsync();

                    TempData["toastMessage"] = "Successfully updated Company!-success";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CompanyExists(company.Company_ID))
                        TempData["toastMessage"] = "Company not found!-danger";
                    else
                        TempData["toastMessage"] = "Something went wrong please try again.-danger";
                }

                return RedirectToAction(nameof(Company));
            }

            ViewData["companies"] = await _db.Companies.ToListAsync();
            return View(nameof(Company), company);
        }

        // POST: Info/DeleteCompany/5
        [HttpPost, ActionName("DeleteCompany")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCompanyConfirmed(int id)
        {
            var company = await _db.Companies.FindAsync(id);
            if (company == null)
            {
                TempData["toastMessage"] = "Delete unsuccessful. Company not found!-danger";
                return RedirectToAction(nameof(Company));
            }

            try
            {
                _db.Companies.Remove(company);
                await _db.SaveChangesAsync();

                TempData["toastMessage"] = "Successfully deleted Company!-success";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unable to delete Company: {id} - {ex}");
                TempData["toastMessage"] = "Something went wrong please try again.-danger";
            }

            return RedirectToAction(nameof(Company));
        }

        [HttpPost]
        public async Task<IActionResult> GetCompany(int? id)
        {
            if (id == null || _db.Companies == null)
            {
                return NoContent();
            }

            var company = await _db.Companies.FindAsync(id);
            if (company == null)
            {
                return NotFound(new { message = "Company not found!" });
            }

            return Ok(company);
        }

        private bool CompanyExists(int id)
        {
            return _db.Companies.Any(e => e.Company_ID == id);
        }

        [HttpPost]
        public async Task<IActionResult> AddCustomContactPerson(
            string Company_name, 
            string Company_name_jpn, 
            string Address, 
            string Address_jpn, 
            string Postal_code, 
            string Telephone, 
            string Fax, 
            string Contact_person, 
            string Contact_person_jpn
            )
        {
            try
            {
                var company = new Company
                {
                    Company_name = Company_name,
                    Company_name_jpn = Company_name_jpn,
                    Address = Address,
                    Address_jpn = Address_jpn,
                    Postal_code = Postal_code,
                    Telephone = Telephone,
                    Fax = Fax
                };

                await _db.Companies.AddAsync(company);
                await _db.SaveChangesAsync();

                var contactPerson = new Contact_person
                {
                    Contact_person_name = Contact_person,
                    Contact_person_name_jpn = Contact_person_jpn,
                    Company_ID = company.Company_ID
                };

                await _db.Contact_persons.AddAsync(contactPerson);
                await _db.SaveChangesAsync();

                var contactPersonName = contactPerson.Contact_person_name != null ? contactPerson.Contact_person_name : contactPerson.Contact_person_name_jpn != null ? contactPerson.Contact_person_name_jpn : "";

                return Ok( new
                {
                    contactPersonId = contactPerson.Contact_person_ID,
                    companyName = company.Company_name + (contactPersonName != "" ? ": " + contactPersonName : "")
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to save custom Company. " + ex.Message);
                return Json(new
                {
                    message = "Failed to save custom Company. Please try again"
                });
            }
        }
        #endregion


        #region Contact_person Functions
        // GET: ContactPerson
        public async Task<IActionResult> ContactPerson()
        {
            ViewData["contactPersons"] = await _db.Contact_persons.Include(s => s.Company).ToListAsync();
            ViewBag.CompanyList = new SelectList(_db.Companies.ToList(), "Company_ID", "Company_name");
            return View();
        }

        // POST: Info/CreateContactPerson
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateContactPerson([Bind("Company_ID,Contact_person_name,Contact_person_name_jpn,Key_words")] Contact_person contactPerson)
        {
            if (ModelState.IsValid)
            {
                _db.Add(contactPerson);
                await _db.SaveChangesAsync();

                TempData["toastMessage"] = "Successfully created Contact Person!-success";
                return RedirectToAction(nameof(ContactPerson));
            }

            ViewData["contactPersons"] = await _db.Contact_persons.Include(s => s.Company).ToListAsync();
            ViewBag.CompanyList = new SelectList(_db.Companies.ToList(), "Company_ID", "Company_name");
            return View(nameof(ContactPerson), contactPerson);
        }

        // POST: Info/EditContactPerson/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditContactPerson(int id, [Bind("Contact_person_ID,Contact_person_name,Contact_person_name_jpn,Company_ID,Key_words")] Contact_person contactPerson)
        {
            if (id != contactPerson.Contact_person_ID)
            {
                TempData["toastMessage"] = "Contact Person not found!-danger";
                return RedirectToAction(nameof(ContactPerson));
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _db.Update(contactPerson);
                    await _db.SaveChangesAsync();

                    TempData["toastMessage"] = "Successfully updated Contact Person.-success";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ContactPersonExists(contactPerson.Contact_person_ID))
                        TempData["toastMessage"] = "Contact Person not found!-danger";
                    else
                        TempData["toastMessage"] = "Something went wrong. Please try again.-danger";
                }

                return RedirectToAction(nameof(ContactPerson));
            }

            ViewData["contactPersons"] = await _db.Contact_persons.Include(s => s.Company).ToListAsync();
            ViewBag.CompanyList = new SelectList(_db.Companies.ToList(), "Company_ID", "Company_name");
            return View(nameof(ContactPerson), contactPerson);
        }

        // POST: Info/DeleteContactPerson/5
        [HttpPost, ActionName("DeleteContactPerson")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteContactPersonConfirmed(int id)
        {
            var contactPerson = await _db.Contact_persons.FindAsync(id);
            if (contactPerson == null)
            {
                TempData["toastMessage"] = "Delete unsuccessful. Contact Person not found!-danger";
                return RedirectToAction(nameof(ContactPerson));
            }

            try
            {
                _db.Contact_persons.Remove(contactPerson);
                await _db.SaveChangesAsync();

                TempData["toastMessage"] = "Successfully deleted Contact Person!-success";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unable to delete Contact Person: {id} - {ex}");
                TempData["toastMessage"] = "Something went wrong please try again.-danger";
            }

            return RedirectToAction(nameof(ContactPerson));
        }

        [HttpPost]
        public async Task<IActionResult> GetContactPerson(int? id)
        {
            if (id == null || _db.Contact_persons == null)
            {
                return NoContent();
            }

            var contactPerson = await _db.Contact_persons.Include(s => s.Company).FirstOrDefaultAsync(s => s.Contact_person_ID == id);
            if (contactPerson == null)
            {
                return NotFound(new { message = "Contact Person not found!" });
            }

            return Ok(contactPerson);
        }

        private bool ContactPersonExists(int id)
        {
            return _db.Contact_persons.Any(e => e.Contact_person_ID == id);
        }

        [HttpPost]
        public IActionResult GetContactPersonAddress(int id)
        {
            var contactPerson = _db.Contact_persons.Find(id);

            if (contactPerson == null)
            {
                return NotFound();
            }

            return GetDeliveryAddress((int)contactPerson.Company_ID);
        }

        [HttpPost]
        public IActionResult GetDeliveryAddress(int id)
        {
            var company = _db.Companies.Find(id);

            if (company == null)
            {
                return NotFound();
            }

            return Json(company.Address ?? company.Address_jpn);
        }

        #endregion


        #region Delivery term Functions
        // GET: DeliveryTerm
        public async Task<IActionResult> DeliveryTerm()
        {
            ViewData["deliveryTerms"] = await _db.Delivery_terms.ToListAsync();

            return View();
        }

        // POST: Info/CreateDeliveryTerm
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateDeliveryTerm([Bind("Delivery_term_name,Delivery_term_name_jpn,Key_words")] Delivery_term deliveryTerm)
        {
            if (ModelState.IsValid)
            {
                _db.Add(deliveryTerm);
                await _db.SaveChangesAsync();

                TempData["toastMessage"] = "Successfully created Delivery Term!-success";
                return RedirectToAction(nameof(DeliveryTerm));
            }

            ViewData["deliveryTerms"] = await _db.Delivery_terms.ToListAsync();

            return View(nameof(DeliveryTerm), deliveryTerm);
        }

        // POST: Info/EditDeliveryTerm/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditDeliveryTerm(int id, [Bind("Delivery_term_ID,Delivery_term_name,Delivery_term_name_jpn,Key_words")] Delivery_term deliveryTerm)
        {
            if (id != deliveryTerm.Delivery_term_ID)
            {
                TempData["toastMessage"] = "Delivery Term not found!-danger";
                return RedirectToAction(nameof(DeliveryTerm));
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _db.Update(deliveryTerm);
                    await _db.SaveChangesAsync();

                    TempData["toastMessage"] = "Successfully updated Delivery Term!-success";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DeliveryTermExists(deliveryTerm.Delivery_term_ID))
                        TempData["toastMessage"] = "Delivery Term not found!-danger";
                    else
                        TempData["toastMessage"] = "Something went worng. PLease try again.-danger";
                }

                return RedirectToAction(nameof(DeliveryTerm));
            }

            ViewData["deliveryTerms"] = await _db.Delivery_terms.ToListAsync();

            return View(nameof(DeliveryTerm), deliveryTerm);
        }

        // POST: Info/DeleteDeliveryTerm/5
        [HttpPost, ActionName("DeleteDeliveryTerm")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteDeliveryTermConfirmed(int id)
        {
            var deliveryTerm = await _db.Delivery_terms.FindAsync(id);
            if (deliveryTerm == null)
            {
                TempData["toastMessage"] = "Delete unsuccessful. Delivery Term not found!-danger";
                return RedirectToAction(nameof(DeliveryTerm));
            }

            try
            {
                _db.Delivery_terms.Remove(deliveryTerm);
                await _db.SaveChangesAsync();

                TempData["toastMessage"] = "Successfully deleted Delivery Term!-success";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unable to delete Delivery Term: {id} - {ex}");
                TempData["toastMessage"] = "Something went wrong please try again.-danger";
            }

            return RedirectToAction(nameof(DeliveryTerm));
        }

        [HttpPost]
        public async Task<IActionResult> GetDeliveryTerm(int id)
        {
            if (id == null || _db.Delivery_terms == null)
            {
                return NoContent();
            }

            var deliveryTerm = await _db.Delivery_terms.FindAsync(id);
            if (deliveryTerm == null)
            {
                return NotFound(new { message = "Delivery Term not found!-danger" });
            }

            return Ok(deliveryTerm);
        }

        private bool DeliveryTermExists(int id)
        {
            return _db.Delivery_terms.Any(e => e.Delivery_term_ID == id);
        }

        [HttpPost]
        public async Task<IActionResult> AddCustomDeliveryTerm(string DeliveryTermName, string DeliveryTermNameJpn)
        {
            try
            {
                var deliveryTerm = new Delivery_term
                {
                    Delivery_term_name = DeliveryTermName,
                    Delivery_term_name_jpn = DeliveryTermNameJpn
                };

                await _db.Delivery_terms.AddAsync(deliveryTerm);
                await _db.SaveChangesAsync();

                return Ok(new
                {
                    deliveryTermName = deliveryTerm.Delivery_term_name,
                    deliveryTermNameJpn = deliveryTerm.Delivery_term_name_jpn
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to save custom delivery term. " + ex.Message);
                return Json(new
                {
                    message = "Failed to save custom delivery term. Please try again"
                });
            }
        }
        #endregion


        #region Payment term Functions
        // GET: PaymentTerm
        public async Task<IActionResult> PaymentTerm()
        {
            ViewData["paymentTerms"] = await _db.Payment_terms.ToListAsync();

            return View();
        }

        // POST: Info/CreatePaymentTerm
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePaymentTerm([Bind("Payment_term_name,Payment_term_name_jpn,Key_words")] Payment_term paymentTerm)
        {
            if (ModelState.IsValid)
            {
                _db.Add(paymentTerm);
                await _db.SaveChangesAsync();

                TempData["toastMessage"] = "Successfully created Payment Term!-success";
                return RedirectToAction(nameof(PaymentTerm));
            }

            ViewData["paymentTerms"] = await _db.Payment_terms.ToListAsync();

            return View(nameof(PaymentTerm), paymentTerm);
        }

        // POST: Info/EditPaymentTerm/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPaymentTerm(int id, [Bind("Payment_term_ID,Payment_term_name,Payment_term_name_jpn,Key_words")] Payment_term paymentTerm)
        {
            if (id != paymentTerm.Payment_term_ID)
            {
                TempData["toastMessage"] = "Payment Term not found!-danger";
                return RedirectToAction(nameof(PaymentTerm));
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _db.Update(paymentTerm);
                    await _db.SaveChangesAsync();

                    TempData["toastMessage"] = "Successfully updated Payment Term!-success";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PaymentTermExists(paymentTerm.Payment_term_ID))
                        TempData["toastMessage"] = "Payment Term not found!-danger";
                    else
                        TempData["toastMessage"] = "Something went wrong. Please try again.-danger";
                }

                return RedirectToAction(nameof(PaymentTerm));
            }

            ViewData["paymentTerms"] = await _db.Payment_terms.ToListAsync();

            return View(nameof(PaymentTerm), paymentTerm);
        }

        // POST: Info/DeletePaymentTerm/5
        [HttpPost, ActionName("DeletePaymentTerm")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePaymentTermConfirmed(int id)
        {
            var paymentTerm = await _db.Payment_terms.FindAsync(id);
            if (paymentTerm == null)
            {
                TempData["toastMessage"] = "Delete unsuccessful. Payment Term not found!-danger";
                return RedirectToAction(nameof(PaymentTerm));
            }

            try
            {
                _db.Payment_terms.Remove(paymentTerm);
                await _db.SaveChangesAsync();

                TempData["toastMessage"] = "Successfully deleted Payment Term!-success";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unable to delete Payment Term: {id} - {ex}");
                TempData["toastMessage"] = "Something went wrong please try again.-danger";
            }

            return RedirectToAction(nameof(PaymentTerm));
        }

        [HttpPost]
        public async Task<IActionResult> GetPaymentTerm(int? id)
        {
            if (id == null || _db.Payment_terms == null)
            {
                return NoContent();
            }

            var paymentTerm = await _db.Payment_terms.FindAsync(id);
            if (paymentTerm == null)
            {
                return NotFound(new { message = "Payment Term not found!"});
            }

            return Ok(paymentTerm);
        }

        private bool PaymentTermExists(int id)
        {
            return _db.Payment_terms.Any(e => e.Payment_term_ID == id);
        }

        [HttpPost]
        public async Task<IActionResult> AddCustomPaymentTerm(string PaymentTermName, string PaymentTermNameJpn)
        {
            try
            {
                var paymentTerm = new Payment_term
                {
                    Payment_term_name = PaymentTermName,
                    Payment_term_name_jpn = PaymentTermNameJpn
                };

                await _db.Payment_terms.AddAsync(paymentTerm);
                await _db.SaveChangesAsync();

                return Ok(new
                {
                    paymentTermName = paymentTerm.Payment_term_name,
                    paymentTermNameJpn = paymentTerm.Payment_term_name_jpn
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to save custom payment term. " + ex);
                return Json(new
                {
                    message = "Failed to save custom payment term. Please try again"
                });
            }
        }
        #endregion
    }
}
