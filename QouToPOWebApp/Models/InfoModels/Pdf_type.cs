using System.ComponentModel.DataAnnotations;

namespace QouToPOWebApp.Models.InfoModels
{
    public class Pdf_type
    {
        [Key]
        public int Pdf_type_ID { get; set; }

        [Display(Name = "Pdf Type Name")]
        public string Pdf_type_name { get; set; }
    }
}
