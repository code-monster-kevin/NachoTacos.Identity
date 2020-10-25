using Microsoft.AspNetCore.Identity;
using NachoTacos.Identity.Admin.Service.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NachoTacos.Identity.Admin.Service
{
    public interface IAccountService
    {
        Task<IdentityRole> CreateRole(string roleName);
        Task<AppUser> CreateUser(string email, string password);
        List<IdentityRole> GetRoles();
        Task<IList<string>> GetUserRoles(IdentityUser user);
        Task<IList<string>> GetUserRoles(string email);
        List<AppUser> GetUsers();
        Task<List<AppUser>> GetUsersByRole(string roleName);
        Task<SignInResult> Login(string application, string email, string password);
        Task<IList<string>> AddUserToRole(string email, string roleName);
    }
}