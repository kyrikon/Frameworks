using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Core.Models;

namespace PLEXOS.Identity.Models
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {

       public ApplicationUser()
        {
           
        }
        public bool IsAccountEnabled { get; set; } = false;
        public virtual Organization Organization
        {           
            get;
            set;
        }
       
    }
}
