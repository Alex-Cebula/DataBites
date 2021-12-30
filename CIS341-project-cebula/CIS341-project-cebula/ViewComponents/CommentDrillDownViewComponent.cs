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

namespace CIS341_project_cebula.ViewComponents
{
    public class CommentDrillDownViewComponent : ViewComponent
    {
        private readonly DataBitesContext _dataBitesContext;
        private readonly UserContext _userContext;
        private readonly UserManager<User> _userManager;
        public CommentDrillDownViewComponent(DataBitesContext dataBitesContext, UserContext userContext, UserManager<User> userManager)
        {
            _dataBitesContext = dataBitesContext;
            _userContext = userContext;
            _userManager = userManager;
        }
        //Invoker for comment drilldown view component
        public async Task<IViewComponentResult> InvokeAsync(int postId)
        {
            //Get currently logged in user
            User currentUser = await _userManager.GetUserAsync(HttpContext.User);

            //Get immediate children of comment and their replies and likes
            List<Post> comments = _dataBitesContext.Posts.Where(x => x.ParentPostId == postId)
                .Include(x => x.Replies)
                .Include(x => x.Likes)
                .ToList();

            //Get all users
            List<User> allUsers = _userContext.Users.ToList();

            List<CommentViewModel> model = new List<CommentViewModel>();

            //Convert comments to their related view model and add them to the parent view model
            foreach (Post comment in comments)
            {
                model.Add(new CommentViewModel
                {
                    Author = allUsers.Where(x => x.UserAccountId == comment.UserAccountId).FirstOrDefault(),
                    Comment = comment,
                    LikeCount = comment.Likes.Count(),
                    ReplyCount = comment.Replies.Count(),
                    IsMutable = true,
                    IsLiked = currentUser != null && comment.Likes.Where(x => x.UserAccountId == currentUser.UserAccountId).Count() != 0,
                    IsAuthenticated = currentUser != null
                });
            }
            return View(model);
        }
    }
}
