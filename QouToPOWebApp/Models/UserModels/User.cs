using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace QouToPOWebApp.Models.UserModels
{
    public class User
    {
        [Key]
        [Display(Name = "Personnel ID")]
        public int Personnel_ID { get; set; }

        [Column(TypeName = "VARCHAR")]
        [Display(Name = "Username")]
        [StringLength(20)]
        public string? Username { get; set; }

        [Column(TypeName = "VARCHAR")]
        [Display(Name = "Password")]
        [StringLength(20)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public string? Password { get; set; }

        [Column(TypeName = "VARCHAR")]
        [Display(Name = "First name")]
        [StringLength(50)]
        public string? First_name { get; set; }

        [Column(TypeName = "VARCHAR")]
        [Display(Name = "Last name")]
        [StringLength(50)]
        public string? Last_name { get; set; }

        [Display(Name = "Privilege")]
        [ForeignKey("Privileges")]
        public int Privilege_ID { get; set; }

        public string? Employment_type { get; set; } // FullTime, PartTime
        public DateTime? Date_hired { get; set; }
        public DateTime? Last_day { get; set; }

        public int Status { get; set; } = 0;
        public DateTime? Last_password_changed { get; set; }
        public int? Qtp_template_ID { get; set; }

        [Display(Name = "Name")]
        public string? Full_name
        {
            get
            {
                if (First_name != null && Last_name != null)
                    return First_name + " " + Last_name;
                else
                    return null;
            }
        }
    }
}