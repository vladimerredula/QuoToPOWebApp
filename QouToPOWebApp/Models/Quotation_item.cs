using System.ComponentModel.DataAnnotations;

namespace QouToPOWebApp.Models
{
    public class Quotation_item
    {
        [Key]
        public int Quotation_item_ID { get; set; }
        public int Quotation_ID { get; set; }
        public string Item_name { get; set; }
        public int Item_quantity { get; set; }
        public float Item_price { get; set; }
    }
}
