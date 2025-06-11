using QouToPOWebApp.Models.InfoModels;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QouToPOWebApp.Models.PoModels
{
    public class Po
    {
        [Key]
        public int Po_ID { get; set; }

        [Display(Name = "PO number")]
        [Required]
        public string? Po_number { get; set; }

        [Display(Name = "Quotation number")]
        public string? Quotation_number { get; set; }

        [Display(Name = "Date")]
        [Required]
        public DateTime? Po_date { get; set; }

        [Display(Name = "Company")]
        [Required]
        public int? Contact_person_ID { get; set; }

        [Display(Name = "Payment term")]
        public string? Payment_term { get; set; }

        [Display(Name = "Delivery term")]
        public string? Delivery_term { get; set; }

        [Display(Name = "Custom term")]
        public string? Custom_term { get; set; }

        [Display(Name = "Delivery address")]
        [Required]
        public int? Delivery_address_ID { get; set; }
        public bool Include_tax { get; set; }

        [Display(Name = "Correspondent")]
        public int? Correspondent_ID { get; set; }

        [Display(Name = "PO title")]
        public string? Po_title { get; set; }

        [Display(Name = "PO language")]
        [Required]
        public string? Po_language { get; set; }

        [Display(Name = "Currency")]
        [Required]
        public string? Currency { get; set; }

        public string? File_name { get; set; }
        public string? File_path { get; set; }
        public int? File_group_ID { get; set; }

        [ForeignKey("Delivery_address_ID")]
        public virtual Company? Companies { get; set; }

        [ForeignKey("Contact_person_ID")]
        public virtual Contact_person? Contact_persons { get; set; }

        [ForeignKey("Correspondent_ID")]
        public virtual Correspondent? Correspondents { get; set; }
    }
}
