using IdentityModel;
using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Test;
using System.Collections.Generic;
using System.Security.Claims;

namespace SingleSignOn.Configurations
{
    public static class Config
    {
        public static IEnumerable<IdentityResource> GetIds()
        { 
            return new List<IdentityResource>()
            { 
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResources.Email(),
                // TODO: 
            };
        }

        public static IEnumerable<ApiResource> GetApis()
        {
            return new List<ApiResource>()
            { 
                new ApiResource("SingleSignOn.Apis", "SSO API"),
                // TODO: 
            };
        }

        public static IEnumerable<Client> GetClients()
        {
            return new Client[]
            {
                new Client
                { 
                    ClientId = "73b933f9-821e-47df-866d-ef97d24c7506",
                    ClientName = "SingleSignOn.Blazor",
                    AllowedGrantTypes = GrantTypes.Hybrid,
                    AllowedScopes = { "openid", "profile", IdentityServerConstants.StandardScopes.Email },
                    ClientSecrets = new List<Secret>() { new Secret("73b933f9-821e-47df-866d-ef97d24c7506".Sha256()) },
                    RedirectUris = { "https://localhost:XXXX/signin-oidc" },
                    PostLogoutRedirectUris = new List<string> { "https://localhost:XXXX" },
                },
            };
        }

        public static List<TestUser> GetTestUsers()
        {
            return new List<TestUser>()
            {
                new TestUser
                {
                    SubjectId = "0bf7162b-a2a2-4877-a2b3-fe51b0ac8399",
                    Username = "a@a.com",
                    Password = "Pa$$w0rd",
                    Claims =
                    {
                        new Claim(JwtClaimTypes.Email, "a@a.com"),
                        new Claim(JwtClaimTypes.Name, "a@a.com"),
                        new Claim(JwtClaimTypes.Role, "Users"),
                    },
                },
                new TestUser
                {
                    SubjectId = "3b24e0c0-b75c-43f0-80fc-5385120458ad",
                    Username = "b@b.com",
                    Password = "Pa$$w0rd",
                    Claims =
                    {
                        new Claim(JwtClaimTypes.Email, "b@b.com"),
                        new Claim(JwtClaimTypes.Name, "b@b.com"),
                        new Claim(JwtClaimTypes.Role, "Users"),
                    },
                },
            };
        }
    }
}
