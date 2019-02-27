using Microsoft.EntityFrameworkCore;
using PLEXOS.Identity.Data;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace PLEXOS.Identity.Models
{

 
    public class Organisation
    {
        public Organisation()
        {
            Members = new List<string>();
        }
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [MaxLength(450)]
        public string ID { get; set; }
        [MaxLength(50)]
        [Required]
        [Display(Name ="Organisation")]
        public string Name { get; set; }
        public string Description { get; set; }

        [NotMapped]
        public IEnumerable<string> Members { get; set; }

    }
}
