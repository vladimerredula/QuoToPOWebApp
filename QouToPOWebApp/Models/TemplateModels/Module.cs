using System.ComponentModel.DataAnnotations;

namespace QouToPOWebApp.Models.TemplateModels
{
    public class Module
    {
        [Key]
        public int Module_ID { get; set; }

        [Required]
        [Display(Name = "Module name")]
        public string? Module_name { get; set; }
        public string? Controller_name { get; set; }
        public string App_name { get; set; }

        public ICollection<Page> Pages { get; set; } = new List<Page>();
    }
}
