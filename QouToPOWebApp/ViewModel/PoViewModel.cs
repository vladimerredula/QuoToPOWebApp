using Microsoft.EntityFrameworkCore;
using QouToPOWebApp.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QouToPOWebApp.ViewModel
{
    [Keyless]
    public class PoViewModel
    {
        [Display(Name = "PO number")]
        [Required]
        public string? Po_number { get; set; }

        [Display(Name = "Quotation number")]
        public string? Quotation_number { get; set; }

        [Display(Name = "Date")]
        [Required]
        public DateTime? Quotation_date { get; set; }

        [Display(Name = "Company")]
        [ForeignKey("Contact_persons")]
        [Required]
        public int? Contact_person_ID { get; set; }

        [Display(Name = "Payment term")]
        [ForeignKey("Payment_terms")]
        [Required]
        public int? Payment_term_ID { get; set; }

        [Display(Name = "Delivery term")]
        [ForeignKey("Delivery_terms")]
        [Required]
        public int? Delivery_term_ID { get; set; }

        [Display(Name = "Delivery address")]
        [ForeignKey("Companies")]
        [Required]
        public int? Delivery_address_ID { get; set; }
        public bool Include_tax { get; set; }

        [Display(Name = "Correspondent")]
        [ForeignKey("Correspondents")]
        public int? Correspondent_ID { get; set; }
        public string? Email { get; set; }

        [Display(Name = "PO title")]
        [Required]
        public string? Po_title { get; set; }

        [Display(Name = "PO language")]
        [Required]
        public string? Po_language { get; set; }

        public string? File_name { get; set; }
        public string? File_path { get; set; }

        [ForeignKey("Pdf_types")]
        public int? Pdf_type_ID { get; set; }

        public string? Extract_mode { get; set; }
        public List<Quotation_item>? Quotation_items { get; set; }

        public virtual Company? Companies { get; set; }
        public virtual Contact_person? Contact_persons { get; set; }
        public virtual Correspondent? Correspondents { get; set; }
        public virtual Payment_term? Payment_terms { get; set; }
        public virtual Delivery_term? Delivery_terms { get; set; }
    }
}
