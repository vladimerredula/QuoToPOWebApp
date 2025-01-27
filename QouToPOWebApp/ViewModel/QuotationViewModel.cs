using Microsoft.EntityFrameworkCore;
using QouToPOWebApp.Models;

namespace QouToPOWebApp.ViewModel
{
    [Keyless]
    public class QuotationViewModel
    {
        public Quotation Quotation { get; set; }
        public List<Quotation_item>? Items { get; set; }
    }
}
