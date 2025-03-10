using System.ComponentModel.DataAnnotations;

namespace QouToPOWebApp.Models.PoModels
{
    public class Po_item
    {
        [Key]
        public int Po_item_ID { get; set; }
        public int Po_ID { get; set; }
        public string? Item_name { get; set; }
        public int? Item_quantity { get; set; }
        public float? Item_price { get; set; }
        public string? Unit { get; set; }
        public int? Order { get; set; }
    }
}
