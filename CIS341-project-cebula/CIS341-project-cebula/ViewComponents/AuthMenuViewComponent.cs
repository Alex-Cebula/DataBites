using CIS341_project_cebula.Areas.Identity.Data;
using CIS341_project_cebula.Data;
using CIS341_project_cebula.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CIS341_project_cebula.ViewComponents
{
    public class AuthMenuViewComponent : ViewComponent
    {
        private readonly UserManager<User> _userManager;

        public AuthMenuViewComponent(UserManager<User> userManager)
        {
            _userManager = userManager;
        }
        //Invoker authentication dropdown menu view component
        public async Task<IViewComponentResult> InvokeAsync()
        {
            UserViewModel model = new UserViewModel();
            //Get currently logged in user and check if they exist
            model.User = await _userManager.GetUserAsync(HttpContext.User);
            if (model.User.IsAuthenticated())
            {
                model.IsAuthenticated = User.Identity.IsAuthenticated;
                //Setting up roles for display
                model.Roles = (await _userManager.GetRolesAsync(model.User)).ToList();
                if (model.Roles.Contains(Constants.Role.Admin))
                {
                    model.HighestRole = Constants.Role.Admin;
                }
                else
                {
                    model.HighestRole = Constants.Role.User;
                }
            }
            return View(model);
        }
    }
}
