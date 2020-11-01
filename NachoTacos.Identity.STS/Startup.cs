using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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
            services.AddLogging();
            services.AddControllersWithViews();

            var identityConnectionString = Configuration.GetConnectionString("IdentityConnection");
            services.RegisterAppIdentitySqlServer<AppIdentityContext>(identityConnectionString);
            services.AddIdentity<IdentityUser, IdentityRole>()
                    .AddEntityFrameworkStores<AppIdentityContext>()
                    .AddDefaultTokenProviders();

            services.AddTransient<IAppIdentityContext, AppIdentityContext>();
            services.AddTransient<IUserValidator, UserValidator>();

            ConfigureIdentityServer(services, Configuration);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            SeedIdentityServer.EnsurePopulated(app);

            app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseIdentityServer();            

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });
        }

        private static void ConfigureIdentityServer(IServiceCollection services, IConfiguration configuration)
        {
            var currentDir = Directory.GetCurrentDirectory();  // IMPORTANT NOTE: Do not use the project directory to store pfx file. This is only for ease of loading this project for training purposes.
            var pfxFilePath = configuration.GetSection("SignInCredentials:PFXFile").Value;
            var pfxPassword = configuration.GetSection("SignInCredentials:Password").Value;
            var pfxFileFullPath = Path.Combine(currentDir, pfxFilePath);

            var idSvrConnectionString = configuration.GetConnectionString("STSConnection");
            var assembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;

            services.AddIdentityServer()
                    .AddSigningCredential(new X509Certificate2(pfxFileFullPath, pfxPassword))
                    .AddConfigurationStore(options =>
                    {
                        options.ConfigureDbContext = b => b.UseSqlServer(idSvrConnectionString, sql => sql.MigrationsAssembly(assembly));
                    })
                    .AddOperationalStore(options =>
                    {
                        options.ConfigureDbContext = b => b.UseSqlServer(idSvrConnectionString, sql => sql.MigrationsAssembly(assembly));
                    });
        }
    }
}
