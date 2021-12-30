using CIS341_project_cebula.Areas.Identity.Data;
using CIS341_project_cebula.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CIS341_project_cebula.Data
{
    public class UserInitializer
    {
        public static readonly string UserPassword = "User@123";
        public static readonly string AdminPassword = "Admin@123";
        public static async Task Initialize(IServiceProvider services)
        {
            DataBitesContext dataBitesContext = services.GetService<DataBitesContext>();
            List<UserAccount> initialUsers = dataBitesContext.Users.ToList();
            User user = await EnsureUser(services, UserPassword, "John@uwsp.edu", initialUsers[0].UserAccountId);
            User admin = await EnsureUser(services, AdminPassword, "AdminJohn@uwsp.edu", initialUsers[1].UserAccountId);
            await EnsureRole(services, admin);
        }
        private static async Task<User> EnsureUser(IServiceProvider services, string password, string userName, int userAccountId)
        {
            UserManager<User> userManager = services.GetService<UserManager<User>>();

            var user = await userManager.FindByNameAsync(userName);
            if(user == null)
            {
                    user = new User
                    {
                        UserName = userName,
                        Email = userName,
                        UserAccountId = userAccountId,
                        EmailConfirmed = true
                    };
                var a = await userManager.CreateAsync(user, password);
            }
            return user;
        }
        private static async Task EnsureRole(IServiceProvider services, User user)
        {
            UserManager<User> userManager = services.GetService<UserManager<User>>();
            RoleManager<IdentityRole> roleManager = services.GetService<RoleManager<IdentityRole>>();
            var role = await roleManager.CreateAsync(new IdentityRole
            {
                Name = "ADMIN"
            });
            await userManager.AddToRoleAsync(user, "Admin");

            return;
        }
    }
}
