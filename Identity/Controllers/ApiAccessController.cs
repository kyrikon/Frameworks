using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PLEXOS.Identity.Data;
using PLEXOS.Identity.Models;
using Microsoft.AspNetCore.Authorization;
using Core.Models;
using System.Security.Claims;

namespace PLEXOS.Identity.Controllers
{
   
    [Authorize(AuthenticationSchemes = "Bearer")]  //(AuthenticationSchemes ="Bearer", Roles = "Admin")
    public class ApiAccessController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ApiAccessController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: ApiAccess

        public async Task<int> Index(int ResourceID,string UserID)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return 0;
            }                                 
            UserApiAccess Access = await _context.UserApiAccess.FirstOrDefaultAsync(x => x.APIResourceID == ResourceID && x.UserID == UserID);
            if(Access == null)
            {
                return 0;
            }
            return (int)Access.ApiAccess;
        }   
        public async Task<NotificationSettings> Notifications(int ResourceID, string UserID)
        {
            NotificationSettings Notify = new NotificationSettings();
            if (!User.Identity.IsAuthenticated)
            {
                Notify.Status = "Error - Un Authorized";
                return Notify;
            }
            UserApiAccess Access = await _context.UserApiAccess.FirstOrDefaultAsync(x => x.APIResourceID == ResourceID && x.UserID == UserID);
            if (Access == null)
            {
                Notify.Status = "Error - No Access";
                return Notify;
            }
            Notify.NotificationEndpoint = Access.NotificationEndpoint;
            Notify.DeviceID = Access.DeviceID;
            Notify.DeviceKey = Access.DeviceKey;
            Notify.Status = "Found End Point";
            return Notify;
        }
        public async Task<string> GetApiEndpoint(string APIName)
        {
           
            if (!User.Identity.IsAuthenticated)
            {
                return "Not Authorized";
            }
            ApiResource rsrce = await _context.ApiResources.FirstOrDefaultAsync(x => x.Name == APIName);
            if (rsrce == null)
            {

                return "Unknown API";
            }
            
            return rsrce.DisplayName;
        }
        [AllowAnonymous]
        public async Task<IList<APIDetails>> GetAPIs()
        {
            IList<APIDetails> GetVals = new List<APIDetails>();
            foreach(ApiResource item in await _context.ApiResources.Where(x => x.Enabled).OrderBy(x => x.Description).ToListAsync())
            {
                GetVals.Add(new APIDetails() { ID = item.Id,Name = item.Name, Description = item.Description, EndPoint = item.DisplayName });
            }
            return GetVals;
        }
    }
}
