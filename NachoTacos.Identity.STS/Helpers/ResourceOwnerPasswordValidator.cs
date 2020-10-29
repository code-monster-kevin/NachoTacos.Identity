using IdentityServer4.Models;
using IdentityServer4.Validation;
using NachoTacos.Identity.Admin.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NachoTacos.Identity.STS.Helpers
{
    public class ResourceOwnerPasswordValidator : IResourceOwnerPasswordValidator
    {
        private readonly IAccountService _accountService;

        public ResourceOwnerPasswordValidator(IAccountService accountService)
        {
            _accountService = accountService;
        }

        public async Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
        {
            var result = await _accountService.Login("", context.UserName, context.Password);
            if (result.Succeeded)
            {
                context.Result = new GrantValidationResult(context.UserName, authenticationMethod: "custom");
            }
            else
            {
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, "Invalid Credentials");
            }
        }
    }
}
