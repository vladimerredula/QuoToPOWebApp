using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QouToPOWebApp.Models
{
    public class Supplier
    {
        [Key]
        public int Supplier_ID { get; set; }

        [ForeignKey(nameof(Company))]
        [Display(Name = "Company")]
        [Required]
        public int? Company_ID { get; set; }

        [Display(Name = "Contact Person")]
        public string? Contact_person { get; set; }

        [Display(Name = "Contact Person")]
        public string? Contact_person_jpn { get; set; }

        [Display(Name = "Key Words")]
        public string? Key_words { get; set; }

        public virtual Company? Company { get; set; }
    }
}
