using System.ComponentModel.DataAnnotations;

namespace QouToPOWebApp.Models
{
    public class Quotation
    {
        [Key]
        public int Quotation_ID { get; set; }
        public string? Quotation_number { get; set; }

        private DateTime? _quotationDate;
        public DateTime? Quotation_date 
        {
            get => _quotationDate; 
            set => _quotationDate = value; 
        }

        public string? Quotation_date_string
        {
            get => _quotationDate.HasValue ? _quotationDate.Value.ToString("yyyy-MM-dd") : string.Empty;
            set
            {
                if (DateTime.TryParse(value, out var parsedDate))
                {
                    _quotationDate = parsedDate;
                } 
                else
                {
                    _quotationDate = null;
                }
            }
        }

        public int? Supplier_ID { get; set; }
        public int? Payment_term_ID { get; set; }
        public int? Delivery_term_ID { get; set; }
        public string? File_name { get; set; }
        public string? File_path { get; set; }
        public int? Pdf_type_ID { get; set; }
        public int? Quotation_item_ID { get; set; }
    }
}
