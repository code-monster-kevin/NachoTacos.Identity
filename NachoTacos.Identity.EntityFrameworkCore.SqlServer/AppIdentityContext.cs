using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NachoTacos.Identity.Model.Entities;
using NachoTacos.Identity.Model.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace NachoTacos.Identity.EntityFrameworkCore.SqlServer
{
    public class AppIdentityContext : IdentityDbContext<IdentityUser>, IAppIdentityContext
    {
        public AppIdentityContext(DbContextOptions<AppIdentityContext> options)
            : base(options) { }

        public DbSet<LoginAudit> LoginAudits { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            SeedDefaultAdminRole.EnsurePopulated(modelBuilder);
            base.OnModelCreating(modelBuilder);
        }

        public override int SaveChanges()
        {
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return base.SaveChangesAsync(cancellationToken);
        }
    }
}
