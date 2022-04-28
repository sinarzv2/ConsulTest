using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthService.Domain;
using AuthService.Services.JwtServices;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.Controllers
{
    [Route("[controller]")]
    public class AccountController:ControllerBase
    {
        public AccountController(UserManager<User> userManager, IJwtService jwtService)
        {
            UserManager = userManager;
            JwtService = jwtService;
        }

        protected UserManager<User> UserManager { get; }
        protected IJwtService JwtService { get; }
        [HttpPost("[action]")]
        public async Task<IActionResult> Login(string username, string password)
        {
            var user = await UserManager.FindByNameAsync(username);
            if (user == null)
                return BadRequest("نام کاربری و رمز عبور اشتباه است.");
            var isPassValid = await UserManager.CheckPasswordAsync(user, password);
            if (!isPassValid)
                return BadRequest("نام کاربری و رمز عبور اشتباه است.");
            var token = await JwtService.GenerateAsync(user);
            return Ok(token);
        }
    }
}
