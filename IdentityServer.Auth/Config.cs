using IdentityModel;
using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Test;
using System.Security.Claims;

namespace IdentityServer.Auth
{
    public static class Config
    {
        public static IEnumerable<Client> Clients => new List<Client> {
            // to get identity token jwt
            new Client
                   {
                        ClientId = "movieClient",
                        AllowedGrantTypes = GrantTypes.ClientCredentials,
                        ClientSecrets =
                        {
                            new Secret("secret".Sha256())
                        },
                        AllowedScopes = { "movieAPI" }
                   },
            // to use openid connect
            new Client {
                   ClientId = "movies_mvc_client",
                   ClientName = "Movies MVC Web App",
                   AllowedGrantTypes = GrantTypes.Hybrid, // old: GrantTypes.Code
                   // GrantTypes.Code dont need  RequirePkce = false cause it default is true
                   RequirePkce = false,
                   AllowRememberConsent = false,
                   RedirectUris = new List<string>()
                   {
                       // client url and port
                       "https://localhost:5002/signin-oidc" 
                   },
                   PostLogoutRedirectUris = new List<string>()
                   {
                       "https://localhost:5002/signout-callback-oidc"
                   },
                   ClientSecrets = new List<Secret>
                   {
                       new Secret("secret".Sha256())
                   },
                   AllowedScopes = new List<string>
                   {
                       // OpenId and profile for type code, the rest addittion for hybrid
                       IdentityServerConstants.StandardScopes.OpenId,
                       IdentityServerConstants.StandardScopes.Profile,
                       IdentityServerConstants.StandardScopes.Address,
                       IdentityServerConstants.StandardScopes.Email,
                       "movieAPI",
                       "roles"
                   }
            }
        };

        public static IEnumerable<IdentityResource> IdentityResources => new List<IdentityResource> {
            new IdentityResources.OpenId(),
            new IdentityResources.Profile(),
        };

        public static IEnumerable<ApiResource> ApiResources => new List<ApiResource> {
          //  new ApiResource
        };

        public static IEnumerable<ApiScope> ApiScopes => new List<ApiScope> {
            new ApiScope("movieAPI", "Movie API by Tung Dao")
        };

        // AddTestUsers require list instead of IEnumerable
        public static List<TestUser> TestUsers => new List<TestUser> {
          new TestUser
                {
                    SubjectId = "5BE86359-073C-434B-AD2D-A3932222DABE",
                    Username = "tungdao",
                    Password = "1102",
                    Claims = new List<Claim>
                    {
                        new Claim(JwtClaimTypes.GivenName, "tung"),
                        new Claim(JwtClaimTypes.FamilyName, "dao")
                    }
                }
        };
    }
}
