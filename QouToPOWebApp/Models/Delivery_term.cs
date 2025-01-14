using System.ComponentModel.DataAnnotations;

namespace QouToPOWebApp.Models
{
    public class Delivery_term
    {
        [Key]
        public int Delivery_term_ID { get; set; }

        [Display(Name = "Delivery Term Name")]
        public string Delivery_term_name { get; set; }

        [Display(Name = "Delivery Term Name (Japanese)")]
        public string Delivery_term_name_jpn { get; set; }
    }
}
