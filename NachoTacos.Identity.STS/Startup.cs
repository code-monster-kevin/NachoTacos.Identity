using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NachoTacos.Identity.Admin.Service;
using NachoTacos.Identity.EntityFrameworkCore.SqlServer;
using NachoTacos.Identity.Model.Interfaces;
using NachoTacos.Identity.STS.Helpers;
using System.IO;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;

namespace NachoTacos.Identity.STS
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            var identityConnectionString = Configuration.GetConnectionString("IdentityConnection");
            services.RegisterAppIdentitySqlServer<AppIdentityContext>(identityConnectionString);
            services.AddIdentity<IdentityUser, IdentityRole>().AddEntityFrameworkStores<AppIdentityContext>();

            services.AddTransient<IAppIdentityContext, AppIdentityContext>();
            services.AddTransient<IAccountService, AccountService>();

            var currentDir = Directory.GetCurrentDirectory();
            var pfxFilePath = Configuration.GetSection("SignInCredentials:PFXFile").Value;
            var pfxPassword = Configuration.GetSection("SignInCredentials:Password").Value;
            var pfxFileFullPath = Path.Combine(currentDir, pfxFilePath);

            var idSvrConnectionString = Configuration.GetConnectionString("STSConnection");
            var assembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;

            services.AddIdentityServer()
                    .AddSigningCredential(new X509Certificate2(pfxFileFullPath, pfxPassword))
                    .AddResourceOwnerValidator<ResourceOwnerPasswordValidator>()
                    .AddConfigurationStore(options =>
                    {
                        options.ConfigureDbContext = b => b.UseSqlServer(idSvrConnectionString, sql => sql.MigrationsAssembly(assembly));
                    })
                    .AddOperationalStore(options =>
                    {
                        options.ConfigureDbContext = b => b.UseSqlServer(idSvrConnectionString, sql => sql.MigrationsAssembly(assembly));
                    });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            SeedIdentityServer.EnsurePopulated(app);

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Hello World!");
                });
            });
        }
    }
}
