using Microsoft.AspNetCore.Identity;
using Core.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;


namespace PLEXOS.Identity.Models.ManageViewModels
{
    public class IndexViewModel
    {
       public IndexViewModel()
        {
            
           
            Organizations = new List<Organization>();
          
        }
        public string Username { get; set; }

        public bool IsEmailConfirmed { get; set; } = false;

        [Display(Name = "Account Enabled")]
        public bool IsAccountEnabled { get; set; } = false;

        public string OrganizationID { get; set; }

        public Organization Organization { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Phone]
        [Display(Name = "Phone number")]
        public string PhoneNumber { get; set; }

        public List<Organization> Organizations { get; set; }

        public string Role { get; set; }

        public List<IdentityRole> AspNetRoles { get; set; }

        public List<UserApiAccess> ApiAccess { get; set; }

        public string StatusMessage { get; set; }

    


    }
}
