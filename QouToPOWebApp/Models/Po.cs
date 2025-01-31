using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QouToPOWebApp.Models
{
    public class Po
    {
        [Key]
        public int Po_ID { get; set; }

        [Display(Name = "PO Date")]
        public DateOnly Po_date { get; set; }

        [Display(Name = "PO number")]
        public string Po_number { get; set; }

        [ForeignKey("Quotations")]
        public int Quotation_ID { get; set; }

        public virtual Quotation? Quotations { get; set; }
    }
}
