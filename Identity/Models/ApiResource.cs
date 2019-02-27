using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace PLEXOS.Identity.Models
{
    public class ApiResource
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string Description { get; set; }
        public string DisplayName { get; set; }
        public bool Enabled { get; set; }
        public string Name { get; set; }
    }
}
