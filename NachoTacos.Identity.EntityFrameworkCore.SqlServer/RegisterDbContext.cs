using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace NachoTacos.Identity.EntityFrameworkCore.SqlServer
{
    public static class RegisterDbContext
    {
        public static void RegisterAppIdentitySqlServer<T>(this IServiceCollection services, string connectionString)
            where T : AppIdentityContext
        {
            var migrationsAssembly = typeof(RegisterDbContext).GetTypeInfo().Assembly.GetName().Name;
            services.AddDbContext<T>(options => options.UseSqlServer(connectionString, sql => sql.MigrationsAssembly(migrationsAssembly)));

            services.Configure<IdentityOptions>(opts => {
                opts.Password.RequiredLength = 8;
                opts.Password.RequireNonAlphanumeric = true;
                opts.Password.RequireLowercase = true;
                opts.Password.RequireUppercase = true;
                opts.Password.RequireDigit = true;
                opts.User.RequireUniqueEmail = true;
            });
        }
    }
}
