using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NachoTacos.Identity.Admin.Service;
using System;
using System.Threading.Tasks;

namespace NachoTacos.Identity.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IAccountService _accountService;
        private readonly ILogger<AccountController> _logger;

        public AdminController(IAccountService accountService, ILogger<AccountController> logger)
        {
            _accountService = accountService;
            _logger = logger;
        }

        [HttpGet]
        [Route("User/List")]
        public IActionResult GetUsers()
        {
            try
            {
                var users = _accountService.GetUsers();
                return Ok(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(0, ex, ex.Message);
                return Problem(ex.Message);
            }
        }

        [HttpGet]
        [Route("Role/List")]
        public IActionResult GetRoles()
        {
            try
            {
                var users = _accountService.GetRoles();
                return Ok(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(0, ex, ex.Message);
                return Problem(ex.Message);
            }
        }

        [HttpGet]
        [Route("Role/Users")]
        public async Task<IActionResult> GetUsersByRole(string roleName)
        {
            try
            {
                var roles = await _accountService.GetUsersByRole(roleName);
                return Ok(roles);
            }
            catch(ArgumentException argex)
            {
                return BadRequest(argex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(0, ex, ex.Message);
                return Problem(ex.Message);
            }
        }

        [HttpGet]
        [Route("User/Role")]
        public async Task<IActionResult> GetUserRoles(string email)
        {
            try
            {
                var roles = await _accountService.GetUserRoles(email);
                return Ok(roles);
            }
            catch (ArgumentException argex)
            {
                return BadRequest(argex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(0, ex, ex.Message);
                return Problem(ex.Message);
            }
        }

        [HttpPost]
        [Route("Role/Create")]
        public async Task<IActionResult> CreateRole(string roleName)
        {
            try
            {
                var role = await _accountService.CreateRole(roleName);
                return Ok(role);
            }
            catch (ArgumentException argex)
            {
                return BadRequest(argex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(0, ex, ex.Message);
                return Problem(ex.Message);
            }
        }

        [HttpPost]
        [Route("User/Create")]
        public async Task<IActionResult> CreateUser(string email, string password)
        {
            try
            {
                var user = await _accountService.CreateUser(email, password);
                return Ok(user);
            }
            catch (ArgumentException argex)
            {
                return BadRequest(argex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(0, ex, ex.Message);
                return Problem(ex.Message);
            }
        }

        [HttpPost]
        [Route("User/Role/Add")]
        public async Task<IActionResult> AddUserToRole(string email, string roleName)
        {
            try
            {
                var roles = await _accountService.AddUserToRole(email, roleName);
                return Ok(roles);
            }
            catch (ArgumentException argex)
            {
                return BadRequest(argex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(0, ex, ex.Message);
                return Problem(ex.Message);
            }
        }
    }
}
