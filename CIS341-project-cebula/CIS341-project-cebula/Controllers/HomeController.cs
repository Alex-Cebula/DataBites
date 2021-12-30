using CIS341_project_cebula.Areas.Identity.Data;
using CIS341_project_cebula.Data;
using CIS341_project_cebula.Models;
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
    /// Home page's controller
    /// </summary>
    public class HomeController : Controller
    {
        private readonly DataBitesContext _dataBitesContext;
        private readonly UserContext _userContext;
        private readonly UserManager<User> _userManager;
        public HomeController(DataBitesContext dataBitesContext, UserContext userContext, UserManager<User> userManager)
        {
            _dataBitesContext = dataBitesContext;
            _userContext = userContext;
            _userManager = userManager;
        }

        #region Read
        /// <summary>
        /// Recent Activity page
        /// </summary>
        /// <returns>View</returns>
        public async Task<IActionResult> Index()
        {
            //Get current user if user is logged in
            User currentUser = await _userManager.GetUserAsync(HttpContext.User);
            HomeViewModel model = new HomeViewModel();

            //Get all posts and their navigation props
            List<Post> posts = _dataBitesContext.Posts
                .Where(x => x.ContentType == Constants.ContentType.Post)
                .Include(x => x.Topic)
                .Include(x => x.Comments)
                .Include(x => x.Likes)
                .ToList();

            //Get all users
            List<User> allUsers = _userContext.Users.ToList();

            //Convert each post to their related view model and then add it to the parent view model
            foreach (Post post in posts)
            {
                model.Posts.Add(new PostViewModel
                {
                    Post = post,
                    IsQuickView = true,
                    ShouldShowTopic = true,
                    Author = allUsers.Where(x => x.UserAccountId == post.UserAccountId).FirstOrDefault(),
                    LikeCount = post.Likes.Count(),
                    CommentCount = post.Comments.Count(),
                    IsMutable = currentUser != null && currentUser.IsAuthorized(post) || User.IsInRole(Constants.Role.Admin),
                    IsLiked = currentUser != null && post.Likes.Where(x => x.UserAccountId == currentUser.UserAccountId).Count() != 0,
                    IsAuthenticated = currentUser != null
                });
            }
            //Sorts by recent activity
            //Either by most recent comment creation date on a post or by the most recent post creation date
            if (model.Posts.Count() > 0)
            {
                if (model.Posts.All(x => x.Post.Comments.Count() > 0))
                {
                    model.Posts = model.Posts.OrderByDescending(x => x.Post.Comments.Max(x => x.DateCreated)).ThenByDescending(x => x.Post.DateCreated).ToList();
                }
                else
                {
                    model.Posts = model.Posts.OrderByDescending(x => x.Post.DateCreated).ToList();
                }
            }
            return View(model);
        }
        #endregion
    }
}
