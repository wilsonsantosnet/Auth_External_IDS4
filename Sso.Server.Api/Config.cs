using Common.Domain.Base;
using IdentityModel;
using IdentityServer4.Models;
using Newtonsoft.Json;
using Sso.Server.Api.Model;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using static IdentityServer4.IdentityServerConstants;
using System.Linq;

namespace Sso.Server.Api
{
    public class Config
    {
        public static UserCredential MakeUsersAdmin()
        {
            return new UserCredential
            {
                SubjectId = "1",
                Username = "adm",
                Password = "123456",
                Claims = ClaimsForAdmin("adm", "adm@adm.com.br")
            };
        }
        public static UserCredential MakeUsersWilson()
        {
            return new UserCredential
            {
                SubjectId = "2",
                Username = "wilson",
                Claims = ClaimsForAdmin("wilson", "wilsonsantosnet@gmail.com")
            };
        }

        public static List<Claim> ClaimsForAdmin(string name, string email)
        {
            return new List<Claim>
            {
                new Claim(JwtClaimTypes.Name, name),
                new Claim(JwtClaimTypes.Email, email),
                new Claim("role", JsonConvert.SerializeObject(new List<string> {"admin"})),
                new Claim("typerole", "admin"),
            };
        }

        public static List<Claim> ClaimsForTenant(int tenantId, string name, string email)
        {
            return new List<Claim>
            {
                new Claim(JwtClaimTypes.Name, name),
                new Claim(JwtClaimTypes.Email, email),
                new Claim("role", JsonConvert.SerializeObject(new List<string> {"tenant"})),
                new Claim("typerole", "tenant"),
            };
        }

        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new List<ApiResource>
            {
                new ApiResource("ssosa", "Sso Basic")
                {
                    Scopes = new List<Scope>()
                    {
                        new Scope
                        {
                            UserClaims = new List<string> {"name", "openid", "email", "role", "tools","typerole"},
                            Name = "ssosa",
                            Description = "sso basic",
                        }
                    }
                }
            };
        }

        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResources.Email(),
            };
        }

        public static IEnumerable<Client> GetClients(ConfigSettingsBase settings)
        {
            return new List<Client>
            {
                new Client
                {
                    ClientId = "Seed",
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                    ClientSecrets =
                    {
                        new Secret("secret".Sha256())
                    },
                    AllowedScopes =
                    {
                        StandardScopes.OpenId,
                        StandardScopes.Profile,
                        StandardScopes.Email,
                        "ssosa"
                    }
                },

				new Client
                {
                    ClientId = "Seed-spa",
                    ClientSecrets = { new Secret("segredo".Sha256()) },

                    AllowedGrantTypes = GrantTypes.Implicit,
                    AllowAccessTokensViaBrowser = true,

					RedirectUris = settings.RedirectUris.ToList(),
                    PostLogoutRedirectUris =  settings.PostLogoutRedirectUris.ToList() ,
                    AllowedCorsOrigins =  settings.ClientAuthorityEndPoint.ToList(),

                    AllowedScopes =
                    {
                        StandardScopes.OpenId,
                        StandardScopes.Profile,
                        StandardScopes.Email,
                        "ssosa"
                    },
					RequireConsent = false,
                    AccessTokenLifetime = 43200
                },
                new Client
                {
                    ClientId = "Seed-spa-custom",
                    ClientSecrets = { new Secret("segredo".Sha256()) },

                    AllowedGrantTypes = GrantTypes.Implicit,
                    AllowAccessTokensViaBrowser = true,

					RedirectUris = settings.RedirectUris.ToList(),
                    PostLogoutRedirectUris =  settings.PostLogoutRedirectUris.ToList() ,
                    AllowedCorsOrigins =  settings.ClientAuthorityEndPoint.ToList(),

                    AllowedScopes =
                    {
                        StandardScopes.OpenId,
                        StandardScopes.Profile,
                        StandardScopes.Email,
                        "ssosa"
                    },
					RequireConsent = false,
                    AccessTokenLifetime = 43200
                },
				new Client
                {
                    ClientId = "hangfire-dash",
                    ClientName = "HangFire Dashboard",
                    ClientSecrets = { new Secret("segredo".Sha256()) },

                    AllowedGrantTypes = GrantTypes.Implicit,
                    AllowAccessTokensViaBrowser = true,
                    RedirectUris = settings.RedirectUris.ToList(),

                     AllowedScopes = {
                        StandardScopes.OpenId,
                        StandardScopes.Profile,
                        StandardScopes.Email,
                        "ssosa"
                    },
                    RequireConsent = false,
                    AccessTokenLifetime = 43200
                },
                new Client
                {
                    ClientId = "hangfire-api",
                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    ClientSecrets = {
                        new Secret("segredo".Sha256())
                    },
                    AllowedScopes = {
                        StandardScopes.OpenId,
                        StandardScopes.Profile,
                        StandardScopes.Email,
                        "ssosa"
                    },
                    Claims = new List<Claim>{
                        new Claim(JwtClaimTypes.Subject,"991")
                    },
                    AccessTokenLifetime = 43200
                },
				new Client
                {
                    ClientId = "swagger-dash",
                    ClientName = "swagger Dashboard",
                    ClientSecrets = { new Secret("segredo".Sha256()) },

                    AllowedGrantTypes = GrantTypes.Implicit,
                    AllowAccessTokensViaBrowser = true,
                    RedirectUris = settings.RedirectUris.ToList(),

                     AllowedScopes = {
                        StandardScopes.OpenId,
                        StandardScopes.Profile,
                        StandardScopes.Email,
                        "ssosa"
                    },
                    RequireConsent = false,
                    AccessTokenLifetime = 43200
                },
				new Client
                {
                    ClientId = "Seed-spa-anonymous",
                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    ClientSecrets = {
                        new Secret("4aa288ca-1603-45c2-85c3-b41a08d2bd0a".Sha256())
                    },
                    AllowedScopes = {
                        StandardScopes.OpenId,
                        StandardScopes.Profile,
                        StandardScopes.Email,
                        "ssosa"
                    },
                }
            };
        }

        public static List<UserCredential> GetUsers()
        {
            return new List<UserCredential>()
            {
                MakeUsersAdmin(),
                MakeUsersWilson(),
            };
        }

    }
}
