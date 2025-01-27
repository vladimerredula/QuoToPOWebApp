using Microsoft.EntityFrameworkCore;
using QouToPOWebApp.Models;

namespace QouToPOWebApp.ViewModel
{
    [Keyless]
    public class PoViewModel
    {
        public Po Po { get; set; }
        public List<Quotation_item>? Quotation_items { get; set; }
    }
}
