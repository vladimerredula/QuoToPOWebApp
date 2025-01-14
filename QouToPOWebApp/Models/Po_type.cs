using System.ComponentModel.DataAnnotations;

namespace QouToPOWebApp.Models
{
    public class Po_type
    {
        [Key]
        public int Po_type_ID { get; set; }

        [Display(Name = "PO Type Name")]
        public string Po_type_name { get; set; }
    }
}
