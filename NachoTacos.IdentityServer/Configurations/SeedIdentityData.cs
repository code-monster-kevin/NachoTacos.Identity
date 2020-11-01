using IdentityServer4;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Linq;

namespace NachoTacos.IdentityServer.Configurations
{
    public static class SeedIdentityData
    {
        public static void EnsurePopulated(IApplicationBuilder app)
        {
            using (var scope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                scope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>().Database.Migrate();
                var context = scope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
                context.Database.Migrate();

                if (!context.Clients.Any())
                {
                    foreach (var client in GetSeedClients())
                    {
                        context.Clients.Add(client.ToEntity());
                    }
                    context.SaveChanges();
                }

                if(!context.ApiScopes.Any())
                {
                    foreach(var apiScope in GetSeedApiScopes())
                    {
                        context.ApiScopes.Add(apiScope.ToEntity());
                    }
                    context.SaveChanges();
                }

                if (!context.IdentityResources.Any())
                {
                    foreach (var resource in GetSeedIdentityResources())
                    {
                        context.IdentityResources.Add(resource.ToEntity());
                    }
                    context.SaveChanges();
                }

                if (!context.ApiResources.Any())
                {
                    foreach (var resource in GetSeedApiResources())
                    {
                        context.ApiResources.Add(resource.ToEntity());
                    }
                    context.SaveChanges();
                }
            }
        }

        private static List<Client> GetSeedClients()
        {
            string clientUri = "https://localhost:44371";

            Client clientMachine2Machine = new Client
            {
                ClientId = "client_m2m",
                ClientName = "Nacho Tacos API",
                AllowedGrantTypes = GrantTypes.ClientCredentials,
                ClientSecrets = { new Secret("511536EF-F270-4058-80CA-1C89C192F69A".Sha256()) },
                AllowedScopes = { "nachoclient" }
            };

            Client clientCodeFlow = new Client
            {
                ClientId = "client_code",
                ClientName = "Nacho Tacos Client",
                //ClientSecrets = { new Secret("49C1A7E1-0C79-4A89-A3D6-A37998FB86B0".Sha256()) },

                AllowedGrantTypes = GrantTypes.Code,
                RedirectUris = { string.Format("{0}/signin-oidc", clientUri) },
                FrontChannelLogoutUri = string.Format("{0}/signout-oidc", clientUri),
                PostLogoutRedirectUris = { string.Format("{0}/signout-callback-oidc", clientUri) },
                RequirePkce = true,
                RefreshTokenUsage = TokenUsage.ReUse,
                AllowOfflineAccess = true,
                AllowedScopes = { 
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile, 
                    "nachocode" 
                }                
            };

            Client clientHybrid = new Client
            {
                ClientId = "client_hybrid",
                ClientName = "Nacho Tacos Web Client",
                ClientSecrets = { new Secret("9C1A7E1-0C79-4A89-A3D6-A37998FB86B".Sha256()) },
                AllowedGrantTypes = GrantTypes.HybridAndClientCredentials,
                RedirectUris = { string.Format("{0}/signin-oidc", clientUri) },
                FrontChannelLogoutUri = string.Format("{0}/signout-oidc", clientUri),
                PostLogoutRedirectUris = { string.Format("{0}/signout-callback-oidc", clientUri) },
                AllowOfflineAccess = true,
                AllowedScopes = {
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile,
                    "nachocode"
                }
            };

            List<Client> clients = new List<Client>();
            clients.Add(clientMachine2Machine);
            clients.Add(clientCodeFlow);
            clients.Add(clientHybrid);
            return clients;
        }

        private static List<ApiScope> GetSeedApiScopes()
        {
            List<ApiScope> apiScopes = new List<ApiScope>();

            apiScopes.Add(new ApiScope("nachoclient"));
            apiScopes.Add(new ApiScope("nachocode"));

            return apiScopes;
        }

        private static List<ApiResource> GetSeedApiResources()
        {
            ApiResource apiResource1 = new ApiResource("nachotacos", "Nacho Tacos");

            List<ApiResource> apiResources = new List<ApiResource>();
            apiResources.Add(apiResource1);
            return apiResources;
        }

        private static List<IdentityResource> GetSeedIdentityResources()
        {
            return new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile()
            };
        }
    }
}
