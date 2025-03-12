using System.ComponentModel.DataAnnotations;

namespace QouToPOWebApp.Models.MiscModels
{
    public class File_group
    {
        [Key]
        public int File_group_ID { get; set; }
        public DateTime Date_created { get; set; }
        public string Directory_path { get; set; }
        public int User_ID { get; set; }

    }
}
