using System.ComponentModel.DataAnnotations;

namespace QouToPOWebApp.Models.PoModels
{
    public class Po_draft
    {
        [Key]
        public int Draft_ID { get; set; }
        public int User_ID { get; set; }
        public string Po_data_json { get; set; }
        public DateTime Last_saved { get; set; }
        public Boolean Is_completed { get; set; }
    }
}
