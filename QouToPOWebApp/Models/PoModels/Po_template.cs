using QouToPOWebApp.Models.InfoModels;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QouToPOWebApp.Models.PoModels
{
    public class Po_template
    {
        [Key]
        public int Template_ID { get; set; }

        [Display(Name = "Template name")]
        public string Template_name { get; set; }

        [Display(Name = "Company")]
        public int? Contact_person_ID { get; set; }

        public string Po_data_json { get; set; }

        [Display(Name = "Created on")]
        public DateTime Date_created { get; set; }

        [Display(Name = "Last modified")]
        public DateTime Date_modified { get; set; }

        [Display(Name = "Created by")]
        public int User_ID { get; set; }

        [ForeignKey("Contact_person_ID")]
        public virtual Contact_person? Contact_person { get; set; }
    }
}
