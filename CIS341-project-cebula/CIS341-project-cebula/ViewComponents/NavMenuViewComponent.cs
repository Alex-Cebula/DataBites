using CIS341_project_cebula.Areas.Identity.Data;
using CIS341_project_cebula.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CIS341_project_cebula.ViewComponents
{
    public class NavMenuViewComponent : ViewComponent
    {
        private readonly UserManager<User> _userManager;
        public NavMenuViewComponent(UserManager<User> userManager)
        {
            _userManager = userManager;
        }
        //Invoker for nav menu view component
        public async Task<IViewComponentResult> InvokeAsync()
        {
            //Fill view model
            UserViewModel model = new UserViewModel();
            model.IsAuthenticated = HttpContext.User.Identity.IsAuthenticated;
            //Get currently logged in user and check if they exist
            model.User = await _userManager.GetUserAsync(HttpContext.User);
            if (model.User != null)
            {
                //Get user's roles
                model.Roles = (await _userManager.GetRolesAsync(model.User)).ToList();
            }
            return View(model);
        }
    }
}
