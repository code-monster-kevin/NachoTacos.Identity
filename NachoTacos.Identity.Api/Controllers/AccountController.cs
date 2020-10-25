using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NachoTacos.Identity.Admin.Service;
using System;
using System.Threading.Tasks;

namespace NachoTacos.Identity.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;
        private readonly ILogger<AccountController> _logger;

        public AccountController(IAccountService accountService, ILogger<AccountController> logger)
        {
            _accountService = accountService;
            _logger = logger;
        }

        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login(string application, string email, string password)
        {
            try
            {
                var result = await _accountService.Login(application, email, password);
                if (result.Succeeded)
                {
                    return Ok();
                }
                else
                {
                    if (result.IsLockedOut)
                    {
                        return Ok("User account locked.");
                    }
                    return Ok("Login attempt invalid.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(0, ex, ex.Message);
                return Problem(ex.Message);
            }
        }
    }
}
