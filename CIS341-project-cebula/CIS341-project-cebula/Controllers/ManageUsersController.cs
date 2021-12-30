using CIS341_project_cebula.Areas.Identity.Data;
using CIS341_project_cebula.Data;
using CIS341_project_cebula.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CIS341_project_cebula.Controllers
{
    /// <summary>
    /// Manage Users controller for Admins
    /// </summary>
    [Authorize(Roles="ADMIN")]
    public class ManageUsersController : Controller
    {
        private readonly DataBitesContext _dataBitesContext;
        private readonly UserContext _userContext;
        private readonly UserManager<User> _userManager;
        public ManageUsersController(DataBitesContext dataBitesContext, UserContext userContext, UserManager<User> userManager)
        {
            _dataBitesContext = dataBitesContext;
            _userContext = userContext;
            _userManager = userManager;
        }

        #region Read
        /// <summary>
        /// View for all users
        /// </summary>
        /// <returns>View</returns>
        public IActionResult Index()
        {
            //Get all users and user accounts
            List<User> allUsers= _userContext.Users.ToList();
            List<UserAccount> allUserAccounts = _dataBitesContext.Users
                .Include(x => x.Topics)
                .Include(x => x.Posts)
                .Include(x => x.Likes)
                .ToList();

            ManageUserViewModel model = new ManageUserViewModel();
            //Fill view model with users converted to a related view model
            foreach(User user in allUsers)
            {
                model.Users.Add(new UserViewModel
                {
                    User = user,
                    UserAccount = allUserAccounts.Where(x => x.UserAccountId == user.UserAccountId).FirstOrDefault()
                });
            }
            return View(model);
        }
        #endregion

        #region Delete
        /// <summary>
        /// Delete method for user
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Json</returns>
        [HttpDelete]
        public async Task<IActionResult> DeleteUser(int id)
        {
            //Get the user and check if it exists
            User user = _userContext.Users.Where(x => x.UserAccountId == id).FirstOrDefault();
            if(user == null)
            {
                return NotFound();
            }
            //Get the users UserAccount object including all navigation properties
            UserAccount userAccount = _dataBitesContext.Users.Where(x => x.UserAccountId == id)
                .Include(x => x.Topics)
                .Include(x => x.Posts)
                .Include(x => x.Likes)
                .FirstOrDefault();

            //Clear all dependancies for the user
            ClearUserContent(userAccount);

            _dataBitesContext.Users.Remove(userAccount);
            _dataBitesContext.SaveChanges();

            await _userManager.DeleteAsync(user);

            //Return url for redirecting
            return Json(Url.Action("Index", "ManageUsers"));
        }
        #endregion

        #region Suspend
        /// <summary>
        /// Suspend post method for user
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Json</returns>
        [HttpPost]
        public IActionResult SuspendUser(int id)
        {
            //Get the UserAccount and check if it exists
            UserAccount userAccount = _dataBitesContext.Users.Where(x => x.UserAccountId == id).FirstOrDefault();
            if(userAccount == null)
            {
                return NotFound();
            }

            //Toggle the UserAccounts suspended state
            if (userAccount.Suspended)
            {
                userAccount.Suspended = false;
            }
            else
            {
                userAccount.Suspended = true;
            }
            _dataBitesContext.SaveChanges();

            //Return url for redirecting
            return Json(Url.Action("Index", "ManageUsers"));
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Delete all dependancies for user
        /// </summary>
        /// <param name="user"></param>
        private void ClearUserContent(UserAccount user)
        {
            //Topics
            if (user.Topics.Count() > 0)
            {
                _dataBitesContext.Topics.RemoveRange(user.Topics);
            }
            //Posts
            if (user.Posts.Count() > 0)
            {
                _dataBitesContext.Posts.RemoveRange(user.Posts);
            }
            //Likes
            if (user.Likes.Count() > 0)
            {
                _dataBitesContext.Likes.RemoveRange(user.Likes);
            }
        }
        #endregion
    }
}
