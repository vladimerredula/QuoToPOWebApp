using Microsoft.EntityFrameworkCore;
using QouToPOWebApp.Models;

namespace QouToPOWebApp
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Company> Companies { get; set; }
        public DbSet<Contact_person> Contact_persons { get; set; }
        public DbSet<Quotation> Quotations { get; set; }
        public DbSet<Quotation_item> quotation_items { get; set; }
        public DbSet<Po> Pos { get; set; }
        public DbSet<Po_type> Pos_types { get; set; }
        public DbSet<Delivery_term> Delivery_terms { get; set; }
        public DbSet<Payment_term> Payment_terms { get; set; }
        public DbSet<Pdf_type> Pdf_types { get; set; }
        public DbSet<User> Users { get; set; }


    }
}
