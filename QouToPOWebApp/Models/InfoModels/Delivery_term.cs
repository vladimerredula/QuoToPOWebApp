using System.ComponentModel.DataAnnotations;

namespace QouToPOWebApp.Models.InfoModels
{
    public class Delivery_term
    {
        [Key]
        public int Delivery_term_ID { get; set; }

        [Display(Name = "Delivery Term Name")]
        [Required]
        public string? Delivery_term_name { get; set; }

        [Display(Name = "Delivery Term Name (Japanese)")]
        public string? Delivery_term_name_jpn { get; set; }

        [Display(Name = "Key Words")]
        public string? Key_words { get; set; }
    }
}
