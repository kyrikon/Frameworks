// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using IdentityServer4;
using IdentityServer4.Models;
using System.Collections.Generic;
using System.Security.Claims;

namespace PLEXOS.Identity
{
    public class Config
    {
        // scopes define the resources in your system
        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            IdentityResource OID = new IdentityResources.OpenId();
            OID.UserClaims.Add("name");
            OID.ShowInDiscoveryDocument = true;
  

            return new List<IdentityResource>
            {
                OID ,
                new IdentityResources.Profile(),
               
            };
        }

        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new List<ApiResource>
            {
                new ApiResource
                {
                     Name = "PLEXOSGraph"
                    ,Description = "PLEXOS Graph API"
                   
                //    ,UserClaims = new List<string> { "name", "Organization" }
                    
                   // ,Scopes = new List<Scope> {new Scope("GraphRead"),new Scope("GraphWrite")}
                }

            };
        }

        // clients want to access resources (aka scopes)
        public static IEnumerable<Client> GetClients()
        {
            // client credentials client
            return new List<Client>
            {
                new Client
                {
                    ClientId = "PLEXOS",
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                     RequireConsent = false,                   
                    AllowedScopes = {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,                       
                        "PLEXOSGraph",
                        "PLEXOSIdentity"
                    }
                   ,AllowOfflineAccess = true
                    
                },

                // resource owner password grant client
                //new Client
                //{
                //    ClientId = "ro.client",
                //    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,

                //    ClientSecrets = 
                //    {
                //        new Secret("secret".Sha256())
                //    },
                //    AllowedScopes = { "PLEXOSGraph" }
                //},

                #region OpenID Connect hybrid flow and client credentials client (MVC)
		// OpenID Connect hybrid flow and client credentials client (MVC)
                //new Client
                //{
                //    ClientId = "mvc",
                //    ClientName = "MVC Client",
                //    AllowedGrantTypes = GrantTypes.HybridAndClientCredentials,

                //    RequireConsent = true,

                //    ClientSecrets =
                //    {
                //        new Secret("secret".Sha256())
                //    },

                //    RedirectUris = { "http://localhost:5002/signin-oidc" },
                //    PostLogoutRedirectUris = { "http://localhost:5002/signout-callback-oidc" },

                //    AllowedScopes =
                //    {
                //        IdentityServerConstants.StandardScopes.OpenId,
                //        IdentityServerConstants.StandardScopes.Profile,
                //        "api1"
                //    },
                //    AllowOfflineAccess = true
                //} 
	#endregion
            };
        }
    }
}