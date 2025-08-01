using Microsoft.EntityFrameworkCore;
using QouToPOWebApp.Models.InfoModels;
using QouToPOWebApp.Models.MiscModels;
using QouToPOWebApp.Models.PoModels;
using QouToPOWebApp.Models.UserModels;

namespace QouToPOWebApp
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Company> Companies { get; set; }
        public DbSet<Contact_person> Contact_persons { get; set; }
        public DbSet<Po_item> Po_items { get; set; }
        public DbSet<Po> Pos { get; set; }
        public DbSet<Delivery_term> Delivery_terms { get; set; }
        public DbSet<Payment_term> Payment_terms { get; set; }
        public DbSet<Pdf_type> Pdf_types { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Correspondent> Correspondents { get; set; }
        public DbSet<Po_draft> Po_drafts { get; set; }
        public DbSet<Po_template> Po_templates { get; set; }
        public DbSet<File_group> File_groups { get; set; }
        public DbSet<Attachment> Attachments { get; set; }

        public DbSet<Session> Sessions { get; set; }
    }
}
