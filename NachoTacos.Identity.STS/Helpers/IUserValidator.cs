using NachoTacos.Identity.STS.ViewModels.Account;
using System.Threading.Tasks;

namespace NachoTacos.Identity.STS.Helpers
{
    public interface IUserValidator
    {
        ApplicationUser FindByUsername(string userName);
        Task<bool> ValidateCredentialsAsync(string userName, string password);
    }
}