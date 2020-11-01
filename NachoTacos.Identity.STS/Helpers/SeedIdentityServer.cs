using IdentityServer4;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Linq;

namespace NachoTacos.Identity.STS.Helpers
{
    public static class SeedIdentityServer
    {
        public static void EnsurePopulated(IApplicationBuilder app)
        {
            using(var scope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                scope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>().Database.Migrate();
                var context = scope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
                context.Database.Migrate();

                if (!context.Clients.Any())
                {
                    foreach(var client in GetSeedClients())
                    {
                        context.Clients.Add(client.ToEntity());
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

            Client clientResourceOwner = new Client
            {
                ClientId = "client_resourceowner",
                ClientName = "Nacho Tacos Web",
                ClientSecrets = { new Secret("nachocheese".Sha256()) },
                AllowedGrantTypes = GrantTypes.ResourceOwnerPasswordAndClientCredentials,
                AllowedScopes = new List<string>
                {
                    "nachotacos"
                }
            };

            Client clientImplicit = new Client
            {
                ClientId = "client_implicit",
                ClientName = "Nacho Tacos Web",
                ClientSecrets = { new Secret("nachocheese".Sha256()) },
                AllowedGrantTypes = GrantTypes.Implicit,
                RedirectUris = { string.Format("{0}/signin-oidc", clientUri) },
                PostLogoutRedirectUris = { string.Format("{0}/signout-callback-oidc", clientUri) },
                AllowedScopes = new List<string>
                {
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile,
                    "nachotacos"
                }
            };

            Client clientCodeFlow = new Client
            {
                ClientId = "client_code",
                ClientName = "Nacho Tacos Web",
                AllowedGrantTypes = GrantTypes.Code,
                RedirectUris = { string.Format("{0}/signin-oidc", clientUri) },
                PostLogoutRedirectUris = { string.Format("{0}/signout-callback-oidc", clientUri) },
                RequireClientSecret = false,
                RequirePkce = true,
                AllowedScopes = new List<string>
                {
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile,
                    "nachotacos"
                }
            };

            Client clientHybrid = new Client
            {
                ClientId = "client_hybridclient",
                ClientName = "Nacho Tacos Web",
                AllowedGrantTypes = GrantTypes.HybridAndClientCredentials,
                ClientSecrets = { new Secret("nachocheese".Sha256()) },
                RedirectUris = { string.Format("{0}/signin-oidc", clientUri) },
                PostLogoutRedirectUris = { string.Format("{0}/signout-callback-oidc", clientUri) },
                AllowedScopes =
                {
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile,
                    "nachotacos"
                }
            };

            List<Client> clients = new List<Client>();
            clients.Add(clientResourceOwner);
            clients.Add(clientImplicit);
            clients.Add(clientCodeFlow);
            clients.Add(clientHybrid);

            return clients;
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
