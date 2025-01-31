using Microsoft.EntityFrameworkCore;
using QouToPOWebApp.Models;

namespace QouToPOWebApp.ViewModel
{
    [Keyless]
    public class PoViewModel
    {
        public string? Quotation_number { get; set; }
        public DateTime? Quotation_date { get; set; }
        public int? Supplier_ID { get; set; }
        public int? Payment_term_ID { get; set; }
        public int? Delivery_term_ID { get; set; }
        public List<Quotation_item>? Quotation_items { get; set; }
    }
}
