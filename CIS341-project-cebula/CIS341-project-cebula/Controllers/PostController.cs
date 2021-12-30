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
    /// Post controller
    /// </summary>
    public class PostController : Controller
    {
        private readonly DataBitesContext _dataBitesContext;
        private readonly UserContext _userContext;
        private readonly UserManager<User> _userManager;
        public PostController(DataBitesContext context, UserContext userContext, UserManager<User> userManager)
        {
            _dataBitesContext = context;
            _userContext = userContext;
            _userManager = userManager;
        }
        #region Read
        /// <summary>
        /// Post detail view
        /// </summary>
        /// <param name="id"></param>
        /// <param name="loadEditor">Load comment editor on page load</param>
        /// <returns>View</returns>
        public async Task<IActionResult> Index(int id, bool loadEditor = false)
        {
            //Get currently logged in user
            User currentUser = await _userManager.GetUserAsync(HttpContext.User);

            //Initialize the model to either the TempData variable or an empty object
            PostDetailViewModel model = TempData["PostDetail"] != null ? JsonSerializer.Deserialize<PostDetailViewModel>(TempData["PostDetail"].ToString()) : new PostDetailViewModel();

            //Get post and it's nav props and check if it exists
            Post post = _dataBitesContext.Posts.Where(x => x.PostId == id && x.ContentType.Equals(Constants.ContentType.Post))
                .Include(x => x.Topic)
                .Include(x => x.Replies).ThenInclude(x => x.Replies)
                .Include(x => x.Replies).ThenInclude(x => x.Likes)
                .Include(x => x.Comments)
                .Include(x => x.Likes)
                .FirstOrDefault();

            if (post == null)
            {
                return NotFound();
            }

            //Get all users 
            List<User> allUsers = _userContext.Users.ToList();

            //Convert post to view model
            model.Post = new PostViewModel()
            {
                Post = post,
                ShouldShowTopic = true,
                Author = allUsers.Where(x => x.UserAccountId == post.UserAccountId).FirstOrDefault(),
                LikeCount = post.Likes.Count(),
                CommentCount = post.Comments.Count(),
                IsMutable = currentUser != null && currentUser.IsAuthorized(post) || User.IsInRole(Constants.Role.Admin),
                IsLiked = currentUser != null && post.Likes.Where(x => x.UserAccountId == currentUser.UserAccountId).Count() != 0,
                IsAuthenticated = currentUser.IsAuthenticated()
            };

            //Convert each comment to it's related view model then add it to the parent model
            foreach (var comment in post.Replies)
            {
                model.Comments.Add(new CommentViewModel()
                {
                    Comment = comment,
                    Author = allUsers.Where(x => x.UserAccountId == comment.UserAccountId).FirstOrDefault(),
                    IsMutable = currentUser != null && currentUser.IsAuthorized(comment) || User.IsInRole(Constants.Role.Admin),
                    LikeCount = comment.Likes.Count(),
                    ReplyCount = comment.Replies.Count(),
                    IsLiked = currentUser != null && comment.Likes.Where(x => x.UserAccountId == currentUser.UserAccountId).Count() != 0,
                    IsAuthenticated = currentUser.IsAuthenticated()
                });
            }

            //Loads comment editor on page
            model.loadEditor = loadEditor;

            return View(model);
        }
        #endregion

        #region Create
        /// <summary>
        /// Create get method for post
        /// </summary>
        /// <param name="topicId"></param>
        /// <returns>View</returns>
        [HttpGet]
        public async Task<IActionResult> Create(int topicId)
        {
            //Get currently logged in user and check if it exists
            User currentUser = await _userManager.GetUserAsync(User);
            if(!currentUser.IsAuthenticated())
            {
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }

            //Create and fill view model
            PostEditorViewModel model = new PostEditorViewModel();
            model.EditorType = Constants.EditorType.Create;
            model.Post.TopicId = topicId;
            model.Post.Topic = _dataBitesContext.Topics.Where(x => x.TopicId == topicId).FirstOrDefault();

            return View(model);
        }
        /// <summary>
        /// Create post method for post
        /// </summary>
        /// <param name="model"></param>
        /// <returns>View</returns>
        [HttpPost]
        public async Task<IActionResult> Create(PostEditorViewModel model)
        {
            if (ModelState.IsValid)
            {
                //Get currently logged in user and check if it exists
                User currentUser = await _userManager.GetUserAsync(User);
                if (!currentUser.IsAuthenticated())
                {
                    return RedirectToPage("/Account/Login", new { area = "Identity" });
                }

                //Get current users UserAccount
                UserAccount currentUserAccount = _dataBitesContext.Users.Where(x => x.UserAccountId == currentUser.UserAccountId).FirstOrDefault();
                
                //Get the topic for this post
                Topic topic = _dataBitesContext.Topics.Where(x => x.TopicId == model.Post.TopicId).FirstOrDefault();

                //Fill the post
                Post post = FillPost(model.Post, topic, currentUserAccount);

                currentUserAccount.Posts.Add(post);
                _dataBitesContext.SaveChanges();

                //Redirect to Post Detail for the new post
                return RedirectToAction("Index", "Post", new { id = post.PostId });
            }
            //Setup view model for creating and return to view with errors
            model.EditorType = Constants.EditorType.Create;
            model.Post.Topic = _dataBitesContext.Topics.Where(x => x.TopicId == model.Post.TopicId).FirstOrDefault();

            return View(model);
        }
        #endregion

        #region Edit
        /// <summary>
        /// Edit get method for post
        /// </summary>
        /// <param name="postId"></param>
        /// <returns>View</returns>
        [HttpGet]
        public async Task<IActionResult> Edit(int postId)
        {
            //Get currently logged in user and check if it exists
            User currentUser = await _userManager.GetUserAsync(User);
            if(!currentUser.IsAuthenticated())
            {
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }

            //Get post and check if it exists
            Post post = _dataBitesContext.Posts.Where(x => x.PostId == postId).Include(x => x.Topic).FirstOrDefault();
            if(post == null)
            {
                return NotFound();
            }

            //Check if current user has access to this post
            if(!currentUser.IsAuthorized(post) && !User.IsInRole(Constants.Role.Admin))
            {
                return Forbid();
            }

            //Create and fill view model
            PostEditorViewModel model = new PostEditorViewModel();
            model.Post = post;
            model.EditorType = Constants.EditorType.Edit;

            //Return to the create view with edit config
            return View("Create", model);
        }
        /// <summary>
        /// Edit post method for post
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
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

                //Get post and check if it exists
                Post post = _dataBitesContext.Posts.Where(x => x.PostId == model.Post.PostId).FirstOrDefault();
                if(post == null)
                {
                    return NotFound();
                }

                //Check if current user has access to this post
                if(!currentUser.IsAuthorized(post) && !User.IsInRole(Constants.Role.Admin))
                {
                    return Forbid();
                }

                //Fill post with user input
                post.Title = model.Post.Title;
                post.Body = model.Post.Body;
                _dataBitesContext.SaveChanges();

                //Redirect to Post Detail for relevant post
                return RedirectToAction("Index", "Post", new { id = post.PostId });
            }
            //Setup view model for editing and return to view with errors
            model.EditorType = Constants.EditorType.Edit;
            model.Post.Topic = _dataBitesContext.Topics.Where(x => x.TopicId == model.Post.TopicId).FirstOrDefault();

            return View("Create", model);
        }
        #endregion

        #region Delete
        /// <summary>
        /// Delete method for post
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            //Get currently logged in user and check if it exists
            User currentUser = await _userManager.GetUserAsync(User);
            if(!currentUser.IsAuthenticated())
            {
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }
            //Get post and it's comments and check if it exists
            Post post = _dataBitesContext.Posts.Where(x => x.PostId == id)
                .Include(x => x.Comments)
                .FirstOrDefault();
            if(post == null)
            {
                return NotFound();
            }

            //Check if current user has access to this post
            if(!currentUser.IsAuthorized(post) && !User.IsInRole(Constants.Role.Admin))
            {
                return Forbid();
            }

            //Delete all dependants of the post
            _dataBitesContext.RemoveRange(post.Comments);
            
            _dataBitesContext.Posts.Remove(post);
            _dataBitesContext.SaveChanges();

            //Return url for redirecting
            return Json(Url.Action("Index", "Topic", new { id = post.TopicId }));
        }
        #endregion

        #region Like
        /// <summary>
        /// Like post method for post
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Json</returns>
        [HttpPost]
        public async Task<ActionResult> Like(int id)
        {
            //Get currently logged in user and check if it exists
            User currentUser = await _userManager.GetUserAsync(HttpContext.User);
            if(!currentUser.IsAuthenticated())
            {
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }
            //Get current users UserAccount and it's likes and check if it exists
            UserAccount currentUserAccount = _dataBitesContext.Users.Where(x => x.UserAccountId == currentUser.UserAccountId).FirstOrDefault();
            Post post = _dataBitesContext.Posts.Where(x => x.PostId == id)
                .Include(x => x.Likes)
                .FirstOrDefault();
            if(post == null)
            {
                return NotFound();
            }

            //Attempt to get a like for this post from the current user
            Like like = _dataBitesContext.Likes.Where(x => x.PostId == post.PostId && x.UserAccountId == currentUser.UserAccountId).FirstOrDefault();

            //If like exists, remove it
            if (like != null)
            {
                _dataBitesContext.Likes.Remove(like);
            }
            //If like doesn't exist, add it
            else
            {
                like = new Like
                {
                    UserAccountId = currentUserAccount.UserAccountId,
                    UserAccount = currentUserAccount,
                    PostId = post.PostId,
                    Post = post
                };
                currentUserAccount.Likes.Add(like);
            }
            _dataBitesContext.SaveChanges();
            
            //Return like count
            return Json(post.Likes.Count());
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Fill a post object
        /// </summary>
        /// <param name="post">Post object</param>
        /// <param name="topic">Topic object</param>
        /// <param name="user">User object</param>
        /// <returns>Post</returns>
        private Post FillPost(Post post, Topic topic, UserAccount user)
        {
            return new Post
            {
                Title = post.Title,
                Body = post.Body,
                TopicId = topic.TopicId,
                Topic = topic,
                UserAccountId = user.UserAccountId,
                UserAccount = user,
                ContentType = Constants.ContentType.Post,
                DateCreated = DateTime.Now
            };
        }
        #endregion
    }
}
