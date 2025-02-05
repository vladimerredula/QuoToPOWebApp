using Microsoft.EntityFrameworkCore;
using QouToPOWebApp.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace QouToPOWebApp.ViewModel
{
    [Keyless]
    public class QuotationViewModel
    {
        [Display(Name = "Quotation number")]
        [Required]
        public string? Quotation_number { get; set; }

        [Display(Name = "Date")]
        [Required]
        public DateTime? Quotation_date { get; set; }

        [Display(Name = "Suppliers")]
        [ForeignKey("Supplier")]
        [Required]
        public int? Supplier_ID { get; set; }

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
        
        public List<Quotation_item>? Quotation_items { get; set; }
        
        public string? File_name { get; set; }
        public string? File_path { get; set; }

        [ForeignKey("Pdf_types")]
        public int? Pdf_type_ID { get; set; }

        public string? ExtractMode { get; set; }

        public virtual Company? Companies { get; set; }
        public virtual Supplier? Suppliers { get; set; }
        public virtual Payment_term? Payment_terms { get; set; }
        public virtual Delivery_term? Delivery_terms { get; set; }
    }
}
