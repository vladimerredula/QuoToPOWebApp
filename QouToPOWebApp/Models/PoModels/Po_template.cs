using System.ComponentModel.DataAnnotations;

namespace QouToPOWebApp.Models.PoModels
{
    public class Po_template
    {
        [Key]
        public int Template_ID { get; set; }

        [Display(Name = "Template name")]
        public string Template_name { get; set; }

        public string Po_data_json { get; set; }

        [Display(Name = "Created on")]
        public DateTime Date_created { get; set; }

        [Display(Name = "Last modified")]
        public DateTime Date_modified { get; set; }

        [Display(Name = "Created by")]
        public int User_ID { get; set; }
    }
}
