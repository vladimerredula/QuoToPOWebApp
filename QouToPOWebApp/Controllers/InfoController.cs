using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QouToPOWebApp.Models;

namespace QouToPOWebApp.Controllers
{
    public class InfoController : Controller
    {
        private readonly ApplicationDbContext _db;

        public InfoController(ApplicationDbContext context)
        {
            _db = context;
        }

        #region Company Functions
        // GET: Company
        public async Task<IActionResult> Company()
        {
            return View("Company/Index", await _db.Companies.ToListAsync());
        }

        // GET: Info/DetailsCompany/5
        public async Task<IActionResult> DetailsCompany(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var company = await _db.Companies
                .FirstOrDefaultAsync(m => m.Company_ID == id);
            if (company == null)
            {
                return NotFound();
            }

            return PartialView("Company/_DisplayPartial", company);
        }

        // GET: Info/Create
        public IActionResult CreateCompany()
        {
            return PartialView("Company/_FormPartial");
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
                return RedirectToAction(nameof(Company));
            }

            return View("Company/Index", company);
        }

        // GET: Info/EditCompany/5
        public async Task<IActionResult> EditCompany(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var company = await _db.Companies.FindAsync(id);
            if (company == null)
            {
                return NotFound();
            }

            return PartialView("Company/_FormPartial", company);
        }

        // POST: Info/EditCompany/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCompany(int id, [Bind("Company_ID,Company_name,Company_name_jpn,Address,Address_jpn,Postal_code,Telephone,Fax")] Company company)
        {
            if (id != company.Company_ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _db.Update(company);
                    await _db.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CompanyExists(company.Company_ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Company));
            }
            return View("Company/Index", company);
        }

        // GET: Info/DeleteCompany/5
        public async Task<IActionResult> DeleteCompany(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var company = await _db.Companies
                .FirstOrDefaultAsync(m => m.Company_ID == id);
            if (company == null)
            {
                return NotFound();
            }

            return PartialView("Company/_DisplayPartial", company);
        }

        // POST: Info/DeleteCompany/5
        [HttpPost, ActionName("DeleteCompany")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCompanyConfirmed(int id)
        {
            var company = await _db.Companies.FindAsync(id);
            if (company != null)
            {
                _db.Companies.Remove(company);
            }

            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Company));
        }

        private bool CompanyExists(int id)
        {
            return _db.Companies.Any(e => e.Company_ID == id);
        }

        [HttpPost]
        public async Task<IActionResult> AddCustomSupplier(string Company_name, string Company_address, string Telephone, string Fax, string Contact_person)
        {
            try
            {
                var company = new Company
                {
                    Company_name = Company_name,
                    Address = Company_address,
                    Telephone = Telephone,
                    Fax = Fax
                };

                await _db.Companies.AddAsync(company);
                await _db.SaveChangesAsync();

                var supplier = new Supplier
                {
                    Contact_person = Contact_person,
                    Company_ID = company.Company_ID
                };

                await _db.Suppliers.AddAsync(supplier);
                await _db.SaveChangesAsync();

                return Ok( new
                {
                    supplerId = supplier.Supplier_ID,
                    companyName = company.Company_name
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    message = "Failed to save custom supplier. Please try again"
                });
            }
        }
        #endregion


        #region Supplier Functions
        // GET: Supplier
        public async Task<IActionResult> Supplier()
        {
            return View("Supplier/Index", await _db.Suppliers.Include(s => s.Company).ToListAsync());
        }

        // GET: Info/DetailsSupplier/5
        public async Task<IActionResult> DetailsSupplier(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var supplier = await _db.Suppliers
                .Include(s => s.Company)
                .FirstOrDefaultAsync(m => m.Supplier_ID == id);

            if (supplier == null)
            {
                return NotFound();
            }

            return PartialView("Supplier/_DisplayPartial", supplier);
        }

        // GET: Info/Create
        public IActionResult CreateSupplier()
        {
            ViewBag.CompanyList = new SelectList(_db.Companies.ToList(), "Company_ID", "Company_name");

            return PartialView("Supplier/_FormPartial");
        }

        // POST: Info/CreateSupplier
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateSupplier([Bind("Company_ID,Contact_person,Contact_person_jpn,Key_words")] Supplier supplier)
        {
            if (ModelState.IsValid)
            {
                _db.Add(supplier);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Supplier));
            }

            return View("Supplier/Index", supplier);
        }

        // GET: Info/EditSupplier/5
        public async Task<IActionResult> EditSupplier(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var supplier = await _db.Suppliers.FindAsync(id);
            if (supplier == null)
            {
                return NotFound();
            }

            ViewBag.CompanyList = new SelectList(_db.Companies.ToList(), "Company_ID", "Company_name", supplier.Company_ID);

            return PartialView("Supplier/_FormPartial", supplier);
        }

        // POST: Info/EditSupplier/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditSupplier(int id, [Bind("Supplier_ID,Contact_person,Contact_person_jpn,Company_ID,Key_words")] Supplier supplier)
        {
            if (id != supplier.Supplier_ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _db.Update(supplier);
                    await _db.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SupplierExists(supplier.Supplier_ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Supplier));
            }
            return View("Supplier/Index", supplier);
        }

        // GET: Info/DeleteSupplier/5
        public async Task<IActionResult> DeleteSupplier(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var supplier = await _db.Suppliers
                .Include(s => s.Company)
                .FirstOrDefaultAsync(m => m.Supplier_ID == id);
            if (supplier == null)
            {
                return NotFound();
            }

            return PartialView("Supplier/_DisplayPartial", supplier);
        }

        // POST: Info/DeleteSupplier/5
        [HttpPost, ActionName("DeleteSupplier")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteSupplierConfirmed(int id)
        {
            var supplier = await _db.Suppliers.FindAsync(id);
            if (supplier != null)
            {
                _db.Suppliers.Remove(supplier);
            }

            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Supplier));
        }

        private bool SupplierExists(int id)
        {
            return _db.Suppliers.Any(e => e.Supplier_ID == id);
        }

        [HttpPost]
        public IActionResult GetSupplierAddress(int id)
        {
            var supplier = _db.Suppliers.Find(id);

            if (supplier == null)
            {
                return NotFound();
            }

            return GetDeliveryAddress((int)supplier.Company_ID);
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
            return View("DeliveryTerm/Index", await _db.Delivery_terms.ToListAsync());
        }

        // GET: Info/DetailsDeliveryTerm/5
        public async Task<IActionResult> DetailsDeliveryTerm(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var deliveryTerm = await _db.Delivery_terms
                .FirstOrDefaultAsync(m => m.Delivery_term_ID == id);
            if (deliveryTerm == null)
            {
                return NotFound();
            }

            return PartialView("DeliveryTerm/_DisplayPartial", deliveryTerm);
        }

        // GET: Info/Create
        public IActionResult CreateDeliveryTerm()
        {
            return PartialView("DeliveryTerm/_FormPartial");
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
                return RedirectToAction(nameof(DeliveryTerm));
            }

            return View("DeliveryTerm/Index", deliveryTerm);
        }

        // GET: Info/EditDeliveryTerm/5
        public async Task<IActionResult> EditDeliveryTerm(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var deliveryTerm = await _db.Delivery_terms.FindAsync(id);
            if (deliveryTerm == null)
            {
                return NotFound();
            }

            return PartialView("DeliveryTerm/_FormPartial", deliveryTerm);
        }

        // POST: Info/EditDeliveryTerm/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditDeliveryTerm(int id, [Bind("Delivery_term_ID,Delivery_term_name,Delivery_term_name_jpn,Key_words")] Delivery_term deliveryTerm)
        {
            if (id != deliveryTerm.Delivery_term_ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _db.Update(deliveryTerm);
                    await _db.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DeliveryTermExists(deliveryTerm.Delivery_term_ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(DeliveryTerm));
            }
            return View("DeliveryTerm/Index", deliveryTerm);
        }

        // GET: Info/DeleteDeliveryTerm/5
        public async Task<IActionResult> DeleteDeliveryTerm(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var deliveryTerm = await _db.Delivery_terms
                .FirstOrDefaultAsync(m => m.Delivery_term_ID == id);
            if (deliveryTerm == null)
            {
                return NotFound();
            }

            return PartialView("DeliveryTerm/_DisplayPartial", deliveryTerm);
        }

        // POST: Info/DeleteDeliveryTerm/5
        [HttpPost, ActionName("DeleteDeliveryTerm")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteDeliveryTermConfirmed(int id)
        {
            var deliveryTerm = await _db.Delivery_terms.FindAsync(id);
            if (deliveryTerm != null)
            {
                _db.Delivery_terms.Remove(deliveryTerm);
            }

            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(DeliveryTerm));
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
                    deliveryTermId = deliveryTerm.Delivery_term_ID,
                    deliveryTermName = deliveryTerm.Delivery_term_name
                });
            }
            catch (Exception ex)
            {
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
            return View("PaymentTerm/Index", await _db.Payment_terms.ToListAsync());
        }

        // GET: Info/DetailsPaymentTerm/5
        public async Task<IActionResult> DetailsPaymentTerm(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var paymentTerm = await _db.Payment_terms
                .FirstOrDefaultAsync(m => m.Payment_term_ID == id);
            if (paymentTerm == null)
            {
                return NotFound();
            }

            return PartialView("PaymentTerm/_DisplayPartial", paymentTerm);
        }

        // GET: Info/Create
        public IActionResult CreatePaymentTerm()
        {
            return PartialView("PaymentTerm/_FormPartial");
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
                return RedirectToAction(nameof(PaymentTerm));
            }

            return View("PaymentTerm/Index", paymentTerm);
        }

        // GET: Info/EditPaymentTerm/5
        public async Task<IActionResult> EditPaymentTerm(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var paymentTerm = await _db.Payment_terms.FindAsync(id);
            if (paymentTerm == null)
            {
                return NotFound();
            }

            return PartialView("PaymentTerm/_FormPartial", paymentTerm);
        }

        // POST: Info/EditPaymentTerm/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPaymentTerm(int id, [Bind("Payment_term_ID,Payment_term_name,Payment_term_name_jpn,Key_words")] Payment_term paymentTerm)
        {
            if (id != paymentTerm.Payment_term_ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _db.Update(paymentTerm);
                    await _db.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PaymentTermExists(paymentTerm.Payment_term_ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(PaymentTerm));
            }
            return View("PaymentTerm/Index", paymentTerm);
        }

        // GET: Info/DeletePaymentTerm/5
        public async Task<IActionResult> DeletePaymentTerm(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var paymentTerm = await _db.Payment_terms
                .FirstOrDefaultAsync(m => m.Payment_term_ID == id);
            if (paymentTerm == null)
            {
                return NotFound();
            }

            return PartialView("PaymentTerm/_DisplayPartial", paymentTerm);
        }

        // POST: Info/DeletePaymentTerm/5
        [HttpPost, ActionName("DeletePaymentTerm")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePaymentTermConfirmed(int id)
        {
            var paymentTerm = await _db.Payment_terms.FindAsync(id);
            if (paymentTerm != null)
            {
                _db.Payment_terms.Remove(paymentTerm);
            }

            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(PaymentTerm));
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
                    paymentTermId = paymentTerm.Payment_term_ID,
                    paymentTermName = paymentTerm.Payment_term_name
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    message = "Failed to save custom payment term. Please try again"
                });
            }
        }
        #endregion
    }
}
