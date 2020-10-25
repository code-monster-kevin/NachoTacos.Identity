using Microsoft.AspNetCore.Identity;
using NachoTacos.Identity.Admin.Service.DTO;
using NachoTacos.Identity.Model.Constants;
using NachoTacos.Identity.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NachoTacos.Identity.Admin.Service.Helpers
{
    public static class AccountHelpers
    {
        public static int GetLoginStatus(SignInResult result)
        {
            int loginStatus = 0;

            if (result.Succeeded)
            {
                loginStatus = (int)LoginStatus.SUCCEEDED;
            }
            if (result.IsLockedOut)
            {
                loginStatus = (int)LoginStatus.IS_LOCKED_OUT;
            }
            if (result.IsNotAllowed)
            {
                loginStatus = (int)LoginStatus.IS_NOT_ALLOWED;
            }
            if (result.RequiresTwoFactor)
            {
                loginStatus = (int)LoginStatus.REQUIRES_TWO_FACTOR;
            }
            if (loginStatus == 0)
            {
                loginStatus = (int)LoginStatus.FAILED_LOGIN;
            }
            return loginStatus;
        }

        public static string GetLoginStatusDescription(int loginStatus)
        {
            switch(loginStatus)
            {
                case (int)LoginStatus.SUCCEEDED:
                    return "SUCCEEDED";
                case (int)LoginStatus.IS_LOCKED_OUT:
                    return "IS_LOCKED_OUT";
                case (int)LoginStatus.IS_NOT_ALLOWED:
                    return "IS_NOT_ALLOWED";
                case (int)LoginStatus.REQUIRES_TWO_FACTOR:
                    return "REQUIRES_TWO_FACTOR";
                case (int)LoginStatus.FAILED_LOGIN:
                    return "FAILED_LOGIN";
                default:
                    return "NO_LOGIN_STATUS";
            }
        }

        public static List<AppUser> MapAppUsers(IAppIdentityContext appIdentityContext, IList<IdentityUser> users)
        {
            List<AppUser> appUsers = new List<AppUser>();
            foreach (IdentityUser user in users)
            {
                AppUser appUser = MapAppUser(appIdentityContext, user);
                appUsers.Add(appUser);
            }
            return appUsers;
        }

        public static AppUser MapAppUser(IAppIdentityContext appIdentityContext, IdentityUser user)
        {
            string loginStatus = string.Empty;
            DateTime? lastLoginDate = null;
            var loginAudit = appIdentityContext.LoginAudits.OrderByDescending(x => x.CreatedDate).FirstOrDefault(x => x.Email == user.Email);
            if (loginAudit != null)
            {
                loginStatus = GetLoginStatusDescription(loginAudit.LoginStatus);
                lastLoginDate = loginAudit.CreatedDate;
            }
            return AppUser.Create(user.Id, user.Email, user.AccessFailedCount, lastLoginDate, loginStatus);
        }

        public static string IdentityResultErrors(IdentityResult result)
        {
            StringBuilder errorResults = new StringBuilder("");
            foreach (var error in result.Errors)
            {
                errorResults.AppendLine(error.Description);
            }
            return errorResults.ToString();
        }
    }


}