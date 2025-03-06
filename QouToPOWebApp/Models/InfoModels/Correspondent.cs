using System.ComponentModel.DataAnnotations;

namespace QouToPOWebApp.Models.InfoModels
{
    public class Correspondent
    {
        [Key]
        public int Correspondent_ID { get; set; }
        public string? Correspondent_name { get; set; }
        public string? Correspondent_position { get; set; }
    }
}
