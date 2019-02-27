
using System;
using System.Collections.Generic;
using NETStandard.Library.Extensions.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;


namespace Core.Models
{


    public class UserApiAccess
    {
        public UserApiAccess()
        {
            ApiAccess = APIAccess.None;
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int ID { get; set; }

        [MaxLength(450)]
        [Required]        
        public string UserID { get; set; }

      
        public int APIResourceID { get; set; }
        
        [NotMapped]
        public string APIResourceDescription { get; set; }
        public APIAccess ApiAccess {get;set;}

        public string NotificationEndpoint { get; set; }
        public string DeviceID { get; set; }
        public string DeviceKey { get; set; }

    }
    public enum APIAccess
    {
        None = 0,
        ReadOnly = 1,
        All = 2
    }
}
