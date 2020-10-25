using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using NachoTacos.Identity.Admin.Service.DTO;
using NachoTacos.Identity.Admin.Service.Helpers;
using NachoTacos.Identity.Model.Constants;
using NachoTacos.Identity.Model.Entities;
using NachoTacos.Identity.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NachoTacos.Identity.Admin.Service
{
    public class AccountService : IAccountService
    {
        private readonly IAppIdentityContext _appIdentityContext;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ILogger<object> _logger;

        public AccountService(
            IAppIdentityContext appIdentityContext,
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            SignInManager<IdentityUser> signInManager,
            ILogger<object> logger)
        {
            _appIdentityContext = appIdentityContext;
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        public async Task<SignInResult> Login(string application, string email, string password)
        {
            try
            {
                var result = await _signInManager.PasswordSignInAsync(email, password, true, true);
                int loginStatus = AccountHelpers.GetLoginStatus(result);

                _appIdentityContext.LoginAudits.Add(LoginAudit.Create(application, email, loginStatus));
                await _appIdentityContext.SaveChangesAsync();
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(0, ex, ex.Message);
                throw new Exception(ex.Message);
            }
        }

        public List<IdentityRole> GetRoles()
        {
            try
            {
                List<IdentityRole> roles = _roleManager.Roles.ToList();
                return roles;
            }
            catch (Exception ex)
            {
                _logger.LogError(0, ex, ex.Message);
                throw new Exception(ex.Message);
            }
        }

        public List<AppUser> GetUsers()
        {
            try
            {
                List<IdentityUser> users = _userManager.Users.ToList();
                List<AppUser> appUsers = AccountHelpers.MapAppUsers(_appIdentityContext, users);
                return appUsers;
            }
            catch (Exception ex)
            {
                _logger.LogError(0, ex, ex.Message);
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<AppUser>> GetUsersByRole(string roleName)
        {
            try
            {
                if (await _roleManager.RoleExistsAsync(roleName))
                {
                    IList<IdentityUser> users = await _userManager.GetUsersInRoleAsync(roleName);
                    List<AppUser> appUsers = AccountHelpers.MapAppUsers(_appIdentityContext, users);
                    return appUsers;
                }
                throw new ArgumentException("Role doesn't exists");
            }
            catch (Exception ex)
            {
                _logger.LogError(0, ex, ex.Message);
                throw new Exception(ex.Message);
            }
        }

        public async Task<IList<string>> GetUserRoles(string email)
        {
            try
            {
                var user = await _userManager.FindByNameAsync(email);
                if (user == null)
                {
                    throw new ArgumentException("User email doesn't exists");
                }

                var roles = await GetUserRoles(user);
                return roles;
            }
            catch (Exception ex)
            {
                _logger.LogError(0, ex, ex.Message);
                throw new Exception(ex.Message);
            }
        }

        public async Task<IList<string>> GetUserRoles(IdentityUser user)
        {
            try
            {
                var roles = await _userManager.GetRolesAsync(user);
                return roles;
            }
            catch (Exception ex)
            {
                _logger.LogError(0, ex, ex.Message);
                throw new Exception(ex.Message);
            }
        }

        public async Task<IdentityRole> CreateRole(string roleName)
        {
            try
            {
                if (await _roleManager.RoleExistsAsync(roleName))
                {
                    throw new ArgumentException("Role already exists");
                }

                string roleId = Guid.NewGuid().ToString();
                IdentityRole role = new IdentityRole
                {
                    Id = roleId,
                    Name = roleName,
                    NormalizedName = roleName.ToUpperInvariant()
                };

                var result = await _roleManager.CreateAsync(role);
                if (result.Succeeded)
                {
                    return role;
                }

                throw new ArgumentException(AccountHelpers.IdentityResultErrors(result));
            }
            catch (Exception ex)
            {
                _logger.LogError(0, ex, ex.Message);
                throw new Exception(ex.Message);
            }
        }

        public async Task<AppUser> CreateUser(string email, string password)
        {
            try
            {
                var user = new IdentityUser { UserName = email, Email = email, EmailConfirmed = true };
                var result = await _userManager.CreateAsync(user, password);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, RoleType.GUEST);
                    AppUser appUser = AccountHelpers.MapAppUser(_appIdentityContext, user);
                    return appUser;
                }

                throw new ArgumentException(AccountHelpers.IdentityResultErrors(result));
            }
            catch (Exception ex)
            {
                _logger.LogError(0, ex, ex.Message);
                throw new Exception(ex.Message);
            }
        }

        public async Task<IList<string>> AddUserToRole(string email, string roleName)
        {
            try
            {
                var user = await _userManager.FindByNameAsync(email);
                if (user == null)
                {
                    throw new ArgumentException("User email doesn't exists");
                }
                if (await _roleManager.RoleExistsAsync(roleName))
                {
                    var result = await _userManager.AddToRoleAsync(user, roleName);
                    if (result.Succeeded)
                    {
                        return await GetUserRoles(user);
                    }
                    throw new ArgumentException(AccountHelpers.IdentityResultErrors(result));
                }
                throw new ArgumentException("Role doesn't exists");
            }
            catch (Exception ex)
            {
                _logger.LogError(0, ex, ex.Message);
                throw new Exception(ex.Message);
            }
        }
    }
}
