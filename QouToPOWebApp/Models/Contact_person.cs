﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QouToPOWebApp.Models
{
    public class Contact_person
    {
        [Key]
        public int Contact_person_ID { get; set; }

        [ForeignKey(nameof(Company))]
        [Display(Name = "Company")]
        [Required]
        public int? Company_ID { get; set; }

        [Display(Name = "Contact Person Name")]
        public string? Contact_person_name { get; set; }

        [Display(Name = "Contact Person Name (Japanese)")]
        public string? Contact_person_name_jpn { get; set; }

        [Display(Name = "Key Words")]
        public string? Key_words { get; set; }

        public virtual Company? Company { get; set; }
    }
}
