using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using AuthService.Domain;
using Microsoft.AspNetCore.Identity;

namespace AuthService.Services.DataInitializer
{
    public class UserDataInitializer : IDataInitializer
    {
        public UserDataInitializer(UserManager<User> userManager)
        {
            UserManager = userManager;
        }

        protected UserManager<User> UserManager { get; }
        public void InitializeData()
        {
            var adminUser =  UserManager.FindByNameAsync("Admin").Result;
            if (adminUser == null)
            {
                var user = new User()
                {
                    UserName = "Admin",
                    Email = "admin@admin.com"
                };
                var result = UserManager.CreateAsync(user, "123456").Result;
                if (!result.Succeeded)
                {
                    throw new Exception(string.Join(" | ", result.Errors.Select(d => d.Description)));
                }

                var claimResult = UserManager.AddClaimsAsync(user, new List<Claim>()
                {
                    new Claim("Permission", "Customers/Get")
                }).Result;
                if (!claimResult.Succeeded)
                {
                    throw new Exception(string.Join(" | ", result.Errors.Select(d => d.Description)));
                }
            }
        }
    }
}
