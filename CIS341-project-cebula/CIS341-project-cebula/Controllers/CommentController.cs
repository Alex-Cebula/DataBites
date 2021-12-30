using CIS341_project_cebula.Areas.Identity.Data;
using CIS341_project_cebula.Data;
using CIS341_project_cebula.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace CIS341_project_cebula.Controllers
{
    /// <summary>
    /// Comment controller
    /// </summary>
    public class CommentController : Controller
    {
        private readonly DataBitesContext _dataBitesContext;
        private readonly UserManager<User> _userManager;
        public CommentController(DataBitesContext dataBitesContext, UserManager<User> userManager)
        {
            _dataBitesContext = dataBitesContext;
            _userManager = userManager;
        }
        /// <summary>
        /// View all immediate replies to a comment
        /// </summary>
        /// <param name="postId"></param>
        /// <returns>Comment drilldown view component</returns>
        [HttpGet]
        public IActionResult DrillDown(int postId)
        {
            return ViewComponent("CommentDrillDown", new { postId = postId });
        }

        #region Create
        /// <summary>
        /// Create get method for comment
        /// </summary>
        /// <param name="postId"></param>
        /// <returns>Comment Editor view component</returns>
        [HttpGet]
        public IActionResult Create(int postId)
        {
            return ViewComponent("CommentEditor", new { postId = postId, editorType = Constants.EditorType.Create });
        }
        /// <summary>
        /// Create post method for comment
        /// </summary>
        /// <param name="model"></param>
        /// <returns>View</returns>
        [HttpPost]
        public async Task<IActionResult> Create(PostEditorViewModel model)
        {
            if (ModelState.IsValid)
            {
                //Get currently logged in user and check if it exists
                User currentUser = await _userManager.GetUserAsync(HttpContext.User);
                if(!currentUser.IsAuthenticated())
                {
                    return RedirectToPage("/Account/Login", new { area = "Identity" });
                }

                //Get current users UserAccount
                UserAccount currentUserAccount = _dataBitesContext.Users.Where(x => x.UserAccountId == currentUser.UserAccountId).FirstOrDefault();
               
                //Get post and related nav props and check if it exists
                Post parentPost = _dataBitesContext.Posts.Where(x => x.PostId == model.Post.ParentPostId)
                    .Include(x => x.Topic)
                    .Include(x => x.RootPost)
                    .FirstOrDefault();
                if(parentPost == null)
                {
                    return NotFound();
                }

                //Fills post
                Post post = FillComment(model.Post, parentPost, currentUserAccount);

                currentUserAccount.Posts.Add(post);
                _dataBitesContext.SaveChanges();

                //Redirect to root post's detail page
                return RedirectToAction("Index", "Post", new { id = post.RootPostId });
            }
            //Set editor type
            model.EditorType = Constants.EditorType.Create;
            //Assign TempData variable to json object of the current model
            TempData["PostDetail"] = JsonSerializer.Serialize(new PostDetailViewModel
            {
                EditorContent = model
            });
            //Redirect to the same view with the content editor filled and displayed
            return RedirectToAction("Index", "Post", new { id = model.Post.ParentPostId, loadEditor = true });
        }
        #endregion

        #region Edit
        /// <summary>
        /// Edit get method for comment
        /// </summary>
        /// <param name="postId"></param>
        /// <returns>View</returns>
        [HttpGet]
        public async Task<IActionResult> Edit(int postId)
        {
            //Get currently logged in user and check if they exist
            User currentUser = await _userManager.GetUserAsync(User);
            if(!currentUser.IsAuthenticated())
            {
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }

            //Get post and check if it exists
            Post model = _dataBitesContext.Posts.Where(x => x.PostId == postId).FirstOrDefault();
            if(model == null)
            {
                return NotFound();
            }
            
            //Check if current user has access to this comment
            if(!currentUser.IsAuthorized(model) && !User.IsInRole(Constants.Role.Admin))
            {
                return Forbid();
            }
            
            //Return view component
            return ViewComponent("CommentEditor", new { postId = postId, editorType = Constants.EditorType.Edit, post = model});
        }
        /// <summary>
        /// Edit post method for comment
        /// </summary>
        /// <param name="model"></param>
        /// <returns>View</returns>
        [HttpPost]
        public async Task<IActionResult> Edit(PostEditorViewModel model)
        {
            if (ModelState.IsValid)
            {
                //Get currently logged in user and check if it exists
                User currentUser = await _userManager.GetUserAsync(User);
                if(!currentUser.IsAuthenticated())
                {
                    return RedirectToPage("/Account/Login", new { area = "Identity" });
                }

                //Get comment and check if it exists
                Post comment = _dataBitesContext.Posts.Where(x => x.PostId == model.Post.PostId).FirstOrDefault();
                if(comment == null)
                {
                    return NotFound();
                }

                //Check if current user has access to this comment
                if(!currentUser.IsAuthorized(comment) && !User.IsInRole(Constants.Role.Admin))
                {
                    return Forbid();
                }
                //Update existing post and save changes to database
                comment.Body = model.Post.Body;
                _dataBitesContext.SaveChanges();
                //Return to root post's detail view
                return RedirectToAction("Index", "Post", new { id = comment.RootPostId });
            }
            //Set editor type
            model.EditorType = Constants.EditorType.Edit;
            //Assign TempData variable to json object of current model
            TempData["PostDetail"] = JsonSerializer.Serialize(new PostDetailViewModel
            {
                EditorContent = model
            });

            //Return to root post's detail view with conditions to load the comment editor
            return RedirectToAction("Index", "Post", new { id = model.Post.ParentPostId, loadEditor = true });
        }
        #endregion

        #region Delete
        /// <summary>
        /// Delete method for comment
        /// </summary>
        /// <param name="postId"></param>
        /// <returns>View</returns>
        [HttpDelete]
        public async Task<IActionResult> Delete(int postId)
        {
            //Get currently logged in user and check if it exists
            User currentUser = await _userManager.GetUserAsync(User);
            if(!currentUser.IsAuthenticated())
            {
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }
            //Get comment and it's replies and check if they exist
            Post comment = _dataBitesContext.Posts.Where(x => x.PostId == postId)
                .Include(x => x.Replies)
                .FirstOrDefault();
            if(comment == null)
            {
                return NotFound();
            }

            //Check if current user has access to this comment
            if(!currentUser.IsAuthorized(comment) && User.IsInRole(Constants.Role.Admin))
            {
                return Forbid();
            }

            //Delete same table dependants
            DeleteCommentChildren(comment.PostId);

            //Delete post
            _dataBitesContext.Posts.Remove(comment);
            _dataBitesContext.SaveChanges();

            //Return url for redirecting
            return Json(Url.Action("Index", "Post", new { id = comment.RootPostId }));
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Fill Post object
        /// </summary>
        /// <param name="post"></param>
        /// <param name="parentPost"></param>
        /// <param name="user"></param>
        /// <returns>Post</returns>
        private Post FillComment(Post post, Post parentPost, UserAccount user)
        {
            return new Post
            {
                TopicId = parentPost.TopicId,
                Topic = parentPost.Topic,
                RootPostId = parentPost.RootPostId != null ? parentPost.RootPostId : parentPost.PostId,
                RootPost = parentPost.RootPost != null ? parentPost.RootPost : parentPost,
                ParentPostId = post.ParentPostId,
                ParentPost = parentPost,
                UserAccountId = user.UserAccountId,
                UserAccount = user,
                Body = post.Body,
                ContentType = parentPost.ContentType.Equals(Constants.ContentType.Post) ? Constants.ContentType.Comment : Constants.ContentType.Reply,
                DateCreated = DateTime.Now
            };
        }
        /// <summary>
        /// Recursively delete comment dependancies
        /// </summary>
        /// <param name="postId"></param>
        private void DeleteCommentChildren(int postId)
        {
            //Get post and it's replies
            Post post = _dataBitesContext.Posts.Where(x => x.PostId == postId).Include(x => x.Replies).FirstOrDefault();
            foreach (Post reply in post.Replies)
            {
                //Delete replies
                DeleteCommentChildren(reply.PostId);
                //Delete post
                _dataBitesContext.Posts.Remove(reply);
            }
        }
        #endregion
    }
}
