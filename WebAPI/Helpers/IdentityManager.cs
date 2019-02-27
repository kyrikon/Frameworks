using IdentityModel.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;

namespace PLEXOS.GraphAPI.Helpers
{
    public static class IdentityManager
    {
        private const string AdminAppSecret = "0ac7b29a-c8f7-45e8-8168-3072b168c77a";
        private const string ApiResourceID = "1";

        public async static Task<string> GetIdentityToken()
        {
            var disco = await DiscoveryClient.GetAsync("https://plexosidentity.azurewebsites.net");
            var tokenClient = new TokenClient(disco.TokenEndpoint, "PLEXOS_ADMIN", AdminAppSecret);
            var tokenResponse = await tokenClient.RequestClientCredentialsAsync("PLEXOSIdentity");
            if (tokenResponse.IsError)
            {              
                return "Authorization Issue";
            }
            return  tokenResponse.AccessToken;

        }
        public async static Task<int> GetAccessLevel(string _IdentToken,ClaimsPrincipal User)
        {
            if (string.IsNullOrEmpty(_IdentToken))
            {
                 _IdentToken = await  GetIdentityToken();
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
            if (User.HasClaim(x => x.Type.Equals("Sub", StringComparison.InvariantCultureIgnoreCase)))
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
    }
}
