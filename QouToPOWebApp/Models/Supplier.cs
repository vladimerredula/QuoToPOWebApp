using System.ComponentModel.DataAnnotations;

namespace QouToPOWebApp.Models
{
    public class Supplier
    {
        [Key]
        public int Supplier_ID { get; set; }
        public int Company_ID { get; set; }

        [Display(Name = "Contact Person")]
        public string Contact_person { get; set; }

        [Display(Name = "Contact Person")]
        public string Contact_person_jpn { get; set; }

    }
}
