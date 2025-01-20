using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QouToPOWebApp.Models;

namespace QouToPOWebApp.Controllers
{
    public class InfoController : Controller
    {
        private readonly ApplicationDbContext _context;

        public InfoController(ApplicationDbContext context)
        {
            _context = context;
        }

        #region Company Functions
        // GET: Company
        public async Task<IActionResult> Company()
        {
            return View("Company/Index", await _context.Companies.ToListAsync());
        }

        // GET: Info/DetailsCompany/5
        public async Task<IActionResult> DetailsCompany(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var company = await _context.Companies
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
                _context.Add(company);
                await _context.SaveChangesAsync();
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

            var company = await _context.Companies.FindAsync(id);
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
                    _context.Update(company);
                    await _context.SaveChangesAsync();
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

            var company = await _context.Companies
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
            var company = await _context.Companies.FindAsync(id);
            if (company != null)
            {
                _context.Companies.Remove(company);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Company));
        }

        private bool CompanyExists(int id)
        {
            return _context.Companies.Any(e => e.Company_ID == id);
        }
        #endregion


        #region Delivery term Functions
        // GET: DeliveryTerm
        public async Task<IActionResult> DeliveryTerm()
        {
            return View("DeliveryTerm/Index", await _context.Delivery_terms.ToListAsync());
        }

        // GET: Info/DetailsDeliveryTerm/5
        public async Task<IActionResult> DetailsDeliveryTerm(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var deliveryTerm = await _context.Delivery_terms
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
        public async Task<IActionResult> CreateDeliveryTerm([Bind("Delivery_term_name,Delivery_term_name_jpn")] Delivery_term deliveryTerm)
        {
            if (ModelState.IsValid)
            {
                _context.Add(deliveryTerm);
                await _context.SaveChangesAsync();
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

            var deliveryTerm = await _context.Delivery_terms.FindAsync(id);
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
        public async Task<IActionResult> EditDeliveryTerm(int id, [Bind("Delivery_term_ID,Delivery_term_name,Delivery_term_name_jpn")] Delivery_term deliveryTerm)
        {
            if (id != deliveryTerm.Delivery_term_ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(deliveryTerm);
                    await _context.SaveChangesAsync();
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

            var deliveryTerm = await _context.Delivery_terms
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
            var deliveryTerm = await _context.Delivery_terms.FindAsync(id);
            if (deliveryTerm != null)
            {
                _context.Delivery_terms.Remove(deliveryTerm);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(DeliveryTerm));
        }

        private bool DeliveryTermExists(int id)
        {
            return _context.Delivery_terms.Any(e => e.Delivery_term_ID == id);
        }
        #endregion
    }
}
