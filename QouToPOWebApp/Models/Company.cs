using System.ComponentModel.DataAnnotations;

namespace QouToPOWebApp.Models
{
    public class Company
    {
        [Key]
        public int Company_ID { get; set; }

        [Display(Name = "Company Name")]
        [Required]
        public string? Company_name { get; set; }

        [Display(Name = "Company Name (Japanese)")]
        public string? Company_name_jpn { get; set; }

        [Required]
        public string? Address { get; set; }

        [Display(Name = "Address (Japanese)")]
        public string? Address_jpn { get; set; }

        [Display(Name = "Postal Code")]
        public string? Postal_code { get; set; }
        public string? Telephone { get; set; }
        public string? Fax { get; set; }
    }
}
