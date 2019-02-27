using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Net.Http;
using IdentityModel.Client;
using System.Web;
using PLEXOS.Core.Models;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using Microsoft.Azure.Graphs;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.Azure.Graphs.Elements;
using System.ComponentModel;
using PLEXOS.GraphAPI.Helpers;

namespace PLEXOS.GraphAPI.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    public class NotificationSettingsController : Controller
    {
        private const string AdminAppSecret = "0ac7b29a-c8f7-45e8-8168-3072b168c77a";
        private const string ApiResourceID = "1";

        private string _IdentToken;
        

        #region Controller actions
        // GET api/values
        [HttpGet]
        public async Task<NotificationSettings> Get()
        {
            return await GetNotificationSettings();
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {

            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
        #endregion

        #region Methods
        private async Task SetToken()
        {

            var disco = await DiscoveryClient.GetAsync("https://plexosidentity.azurewebsites.net");
            var tokenClient = new TokenClient(disco.TokenEndpoint, "PLEXOS_ADMIN", AdminAppSecret);
            var tokenResponse = await tokenClient.RequestClientCredentialsAsync("PLEXOSIdentity");
            if (tokenResponse.IsError)
            {
                _IdentToken = string.Empty;
                return;
            }
            _IdentToken = tokenResponse.AccessToken;
        } 
        private async Task<int> GetAccessLevel()
        {
            if (string.IsNullOrEmpty(_IdentToken))
            {
                await this.SetToken();
            }
            if (User.IsInRole("Admin"))
            {
                return 3;
            }
            //Get User ID
            if (!User.Identity.IsAuthenticated)
            {
                return 0;
            }
            if(User.HasClaim(x => x.Type.Equals("Sub", StringComparison.InvariantCultureIgnoreCase)))
            {
                int result = 0;
                var client = new HttpClient();
                client.SetBearerToken(_IdentToken);
                //    var builder = new UriBuilder("http://plexosidentity.azurewebsites.net/ApiAccess"); //http://localhost:64091/ApiAccess
                var builder = new UriBuilder("http://plexosidentity.azurewebsites.net/ApiAccess");
                builder.Port = -1;
                var query = HttpUtility.ParseQueryString(builder.Query);
                query["ResourceID"] = ApiResourceID;
                query["UserID"] = User.Claims.FirstOrDefault(x => x.Type.Equals("Sub", StringComparison.InvariantCultureIgnoreCase)).Value;

                builder.Query = query.ToString();
                string url = builder.ToString();
                var response = await client.GetAsync(url);
                var getIDStr = await response.Content.ReadAsStringAsync();
                int.TryParse(getIDStr, out result);
                return result;
            }
            else
            {
                return 0;
            }
        }
        private async Task<NotificationSettings> GetNotificationSettings()
        {
            NotificationSettings notificationSetting = new NotificationSettings();

            //Get User ID
            if (!User.Identity.IsAuthenticated)
            {
                notificationSetting.Status = "Error - Not Authenticated";
                return notificationSetting;
            }
            if (User.HasClaim(x => x.Type.Equals("Sub", StringComparison.InvariantCultureIgnoreCase)))
            {
                int result = 0;
                var client = new HttpClient();
                if (string.IsNullOrEmpty(_IdentToken))
                {
                    _IdentToken = await IdentityManager.GetIdentityToken();
                }
                client.SetBearerToken(_IdentToken);
             
                var builder = new UriBuilder("http://plexosidentity.azurewebsites.net/apiaccess/Notifications");
                builder.Port = -1;
                var query = HttpUtility.ParseQueryString(builder.Query);
                query["ResourceID"] = ApiResourceID;
                query["UserID"] = User.Claims.FirstOrDefault(x => x.Type.Equals("Sub", StringComparison.InvariantCultureIgnoreCase)).Value;

                builder.Query = query.ToString();
                string url = builder.ToString();
                var response = await client.GetAsync(url);
                var getIDStr = await response.Content.ReadAsStringAsync();
                notificationSetting = JsonConvert.DeserializeObject<NotificationSettings>(getIDStr);
                return notificationSetting;
            }
            else
            {
                notificationSetting.Status = "Error - Invalid User";
                return notificationSetting;
            }
        }
        #endregion
    }
}
