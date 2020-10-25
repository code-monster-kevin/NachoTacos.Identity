using Microsoft.EntityFrameworkCore;
using NachoTacos.Identity.Model.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace NachoTacos.Identity.Model.Interfaces
{
    public interface IAppIdentityContext
    {
        DbSet<LoginAudit> LoginAudits { get; set; }
        int SaveChanges();
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
