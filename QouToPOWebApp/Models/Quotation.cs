using System.ComponentModel.DataAnnotations;

namespace QouToPOWebApp.Models
{
    public class Quotation
    {
        [Key]
        public int Quotation_ID { get; set; }
        public string? Quotation_number { get; set; }

        [Display(Name = "Quotation Number")]
        public string Quotation_number { get; set; }

        [Display(Name = "Quotation Date")]
        public DateOnly Quotation_date { get; set; }

        [Display(Name = "Supplier")]
        public int Supplier_ID { get; set; }

        [Display(Name = "Payment Terms")]
        public int Payment_term_ID { get; set; }

        public int? Supplier_ID { get; set; }
        public int? Payment_term_ID { get; set; }
        public int? Delivery_term_ID { get; set; }
        public string? File_name { get; set; }
        public string? File_path { get; set; }
        public int? Pdf_type_ID { get; set; }
        public int? Quotation_item_ID { get; set; }
    }
}
