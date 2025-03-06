using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QouToPOWebApp.Models.PoModels
{
    public class Po
    {
        [Key]
        public int Po_ID { get; set; }

        [Display(Name = "PO Date")]
        public DateOnly Po_date { get; set; }

        [Display(Name = "PO number")]
        public string Po_number { get; set; }
    }
}
