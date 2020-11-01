using IdentityModel;
using Microsoft.AspNetCore.Identity;
using NachoTacos.Identity.STS.ViewModels.Account;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace NachoTacos.Identity.STS.Helpers
{
    public class UserValidator : IUserValidator
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;

        public UserValidator(SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        public async Task<bool> ValidateCredentialsAsync(string userName, string password)
        {
            var result = await _signInManager.PasswordSignInAsync(userName, password, false, false);
            if (result.Succeeded) { return true; }
            return false;
        }

        public ApplicationUser FindByUsername(string userName)
        {
            var user = _userManager.Users.FirstOrDefault(x => x.UserName == userName);
            if (user != null)
            {
                return new ApplicationUser
                {
                    SubjectId = user.Id,
                    Username = user.Email,
                    IsActive = !user.LockoutEnabled,
                    Claims =
                    {
                        new Claim(JwtClaimTypes.Email, user.Email)
                    }
                };
            }
            return null;
        }
    }
}
