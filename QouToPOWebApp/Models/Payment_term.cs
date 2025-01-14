using System.ComponentModel.DataAnnotations;

namespace QouToPOWebApp.Models
{
    public class Payment_term
    {
        [Key]
        public int Payment_term_ID { get; set; }

        [Display(Name = "Payment Term Name")]
        public string Payment_term_name { get; set; }

        [Display(Name = "Payment Term Name (Japanese)")]
        public string Payment_term_name_jpn { get; set; }
    }
}
