using Microsoft.AspNetCore.Identity;
using System;

namespace NachoTacos.Identity.Admin.Service.DTO
{
    public class AppUser : IdentityUser
    {
        public DateTime? LastLoginAttempt { get; set; }
        public string LoginStatus { get; set; }

        public static AppUser Create(IdentityUser user, DateTime? lastLoginAttempt, string loginStatus)
        {
            AppUser appUser = (AppUser) user;

            appUser.LastLoginAttempt = lastLoginAttempt;
            appUser.LoginStatus = loginStatus;

            return appUser;
        }
    }
}
