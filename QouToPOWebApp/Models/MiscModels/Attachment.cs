using System.ComponentModel.DataAnnotations;

namespace QouToPOWebApp.Models.MiscModels
{
    public class Attachment
    {
        [Key]
        public int Attachment_ID { get; set; }
        public int? User_ID { get; set; }
        public string? File_name { get; set; }
        public string? File_path { get; set; }
        public string? Attachment_type { get; set; }
        public int? File_group_ID { get; set; }
        public DateTime? Date_uploaded { get; set; }
        public string? Status { get; set; }
    }
}
