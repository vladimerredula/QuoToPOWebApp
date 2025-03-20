using System.ComponentModel.DataAnnotations;

namespace QouToPOWebApp.ViewModel
{
    public class TemplateViewModel
    {
        public int Template_ID { get; set; }

        [Display(Name = "Template name")]
        [Required]
        public string Template_name { get; set; }
        public PoViewModel? Po { get; set; }
    }
}
