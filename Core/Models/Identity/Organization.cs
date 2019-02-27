
using System;
using System.Collections.Generic;
using NETStandard.Library.Extensions.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Core.Models
{

 
    public class Organization
    {
        public Organization()
        {
            Members = new List<string>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [MaxLength(450)]
        public string ID { get; set; }

        [MaxLength(50)]
        [Required]
        [Display(Name = "Organisation")]
        public string Name { get; set; }
        public string Description { get; set; }
        [NotMapped]
        public IEnumerable<string> Members { get; set; }

    }
}
