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
        /// <summary>
        /// Web, Blazor
        /// GetIdentityResources()
        /// </summary>
        public static IEnumerable<IdentityResource> GetIds()
        { 
            return new List<IdentityResource>()
            { 
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResources.Email(),
                // TODO: 커스텀 아이덴터티 리소스
                new IdentityResource("CustomerInfo", new [] { "Customer" }),
                //new IdentityResource { 
                //    Name = "Role",
                //    UserClaims = new List<string>{ "Role" }
                //}
            };
        }

        /// <summary>
        /// Web API
        /// GetApiResources()
        /// </summary>
        public static IEnumerable<ApiResource> GetApis()
        {
            return new List<ApiResource>()
            { 
                new ApiResource("SingleSignOn.Apis", "SSO API"),
                new ApiResource 
                { 
                    Name = "My Web API", 
                    DisplayName = "My Web API", 
                    Description = "My Web API",
                    UserClaims = new List<string>{ "Guests", "Role" },
                    ApiSecrets = new List<Secret>{ new Secret("AbCdEfGhIjK".Sha256()) },
                    Scopes = new List<Scope>
                    { 
                        new Scope("My Web API.Create"),
                        new Scope("My Web API.Read"),
                        new Scope("My Web API.Update"),
                        new Scope("My Web API.Delete"),
                    },
                },
                // TODO: 
            };
        }
        
        public static IEnumerable<Client> GetClients()
        {
            return new Client[]
            {
                //[1] MVC 프로젝트
                new Client
                {
                    // MVC 프로젝트의 GUID 값과 일치하는 Guid.NewGuid().ToString() 값, 의미있는 간단한 값 사용 가능
                    ClientId = "73b933f9-821e-47df-866d-ef97d24c7506", // "MvcClient"

                    ClientName = "SingleSignOn.Mvc",

                    //AllowedGrantTypes = GrantTypes.ClientCredentials,
                    //AllowedGrantTypes = GrantTypes.Implicit,
                    AllowedGrantTypes = GrantTypes.Hybrid,

                    // IdentityServerConstants.StandardScopes의 3가지 상수 사용
                    AllowedScopes = { "openid", "profile", IdentityServerConstants.StandardScopes.Email, "Role", "My Web API.Create" },
                    
                    ClientSecrets = new List<Secret>() { new Secret("73b933f9-821e-47df-866d-ef97d24c7506".Sha256()) },
                    
                    // 로그인 후 원래 호출한 사이트의 어느 페이지(링크, 라우트)로 이동할건지
                    RedirectUris = { "https://localhost:44302/signin-oidc" },

                    // 로그아웃 후 원래 호출한 사이트의 어느 페이지로 이동할건지
                    PostLogoutRedirectUris = new List<string> { "https://localhost:44302/" },
                },
                //[2] Blazor 프로젝트
                new Client
                {
                    ClientId = "6a297776-c6ae-49c6-8cae-e6ef10a92cf0", // "BlazorClient" 
                    ClientName = "SingleSignOn.Blazor",
                    //AllowedGrantTypes = GrantTypes.Implicit,
                    AllowedGrantTypes = GrantTypes.Hybrid, // WorkFlow
                    AllowedScopes = { "openid", "profile", IdentityServerConstants.StandardScopes.Email },
                    ClientSecrets = new List<Secret>() { new Secret("6a297776-c6ae-49c6-8cae-e6ef10a92cf0".Sha256()) },
                    RedirectUris = { "https://localhost:44376/signin-oidc" },
                    //PostLogoutRedirectUris = new List<string> { "https://localhost:44376/" },
                    PostLogoutRedirectUris = new List<string> { "https://localhost:44376/signout-callback-oidc" },
                },
                //[3] Web API 프로젝트
                new Client
                {
                    ClientId = "__297776-c6ae-49c6-8cae-e6ef10a92cf0",
                    ClientName = "My Web API",
                    AllowedGrantTypes = GrantTypes.ClientCredentials, 
                    AllowedScopes = { "My Web API.Create", "My Web API.Read" },
                    ClientSecrets = new List<Secret>() { new Secret("__297776-c6ae-49c6-8cae-e6ef10a92cf0".Sha256()) },
                },
            };
        }

        /// <summary>
        /// 테스트 사용자 계정(아이디, 암호)
        /// </summary>
        public static List<TestUser> GetTestUsers()
        {
            return new List<TestUser>()
            {
                new TestUser
                {
                    // 의미있는 간단한 문자열 설정 가능
                    SubjectId = "0bf7162b-a2a2-4877-a2b3-fe51b0ac8399",
                    Username = "a@a.com",
                    Password = "Pa$$w0rd",
                    Claims =
                    {
                        new Claim(JwtClaimTypes.Name, "Angular"),
                        new Claim(JwtClaimTypes.Email, "a@a.com"),
                        new Claim(JwtClaimTypes.Role, "Users"),
                        new Claim("website", "http://angular.dul.me"),
                    },
                },
                new TestUser
                {
                    SubjectId = "3b24e0c0-b75c-43f0-80fc-5385120458ad",
                    Username = "b@b.com",
                    Password = "Pa$$w0rd",
                    Claims =
                    {
                        new Claim(JwtClaimTypes.Name, "Blazor"),
                        new Claim(JwtClaimTypes.Email, "b@b.com"),
                        new Claim(JwtClaimTypes.Role, "Users"),
                        new Claim("website", "http://www.dotnetnote.com"),
                    },
                },
            };
        }
    }
}
