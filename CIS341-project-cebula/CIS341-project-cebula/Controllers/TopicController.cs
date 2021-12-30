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
    /// Topic controller
    /// </summary>
    public class TopicController : Controller
    {
        private readonly DataBitesContext _dataBitesContext;
        private readonly UserContext _userContext;
        private readonly UserManager<User> _userManager;
        public TopicController(DataBitesContext dataBitesContext, UserContext userContext, UserManager<User> userManager)
        {
            _dataBitesContext = dataBitesContext;
            _userContext = userContext;
            _userManager = userManager;
        }

        #region Read
        /// <summary>
        /// View for all topics
        /// </summary>
        /// <returns>View</returns>
        [HttpGet()]
        public IActionResult Index()
        {
            //Get all topics
            List<Topic> topics = _dataBitesContext.Topics
                .Include(x => x.Posts)
                .ToList();

            //Get all users
            List<User> allUsers = _userContext.Users.ToList();

            TopicsViewModel model = new TopicsViewModel();
            //Convert each topic to their related view model then add it to the parent view model
            foreach(Topic topic in topics)
            {
                //Get all posts where content type is post (no comments or replies)
                topic.Posts = topic.Posts.Where(x => x.ContentType.Equals(Constants.ContentType.Post)).ToList();
                model.Topics.Add(new TopicViewModel
                    {
                        Author = allUsers.Where(x => x.UserAccountId == topic.UserAccountId).FirstOrDefault(),
                        Topic = topic,
                        IsQuickView = true
                });
            }
            return View("Topics", model);
        }
        /// <summary>
        /// View for specific topic
        /// </summary>
        /// <param name="id"></param>
        /// <returns>View</returns>
        [HttpGet("Topic/{id}")]
        public async Task<IActionResult> Index(int id)
        {
            //Get the currently logged in user
            User currentUser = await _userManager.GetUserAsync(User);
            //Get topic and it's nav props and check if it exists
            Topic topic = _dataBitesContext.Topics.Where(x => x.TopicId == id)
                .Include(x => x.Posts).ThenInclude(x => x.Likes)
                .Include(x => x.Posts).ThenInclude(x => x.Comments)
                .FirstOrDefault();
            if (topic == null)
            {
                return NotFound();
            }

            //Get all users
            List<User> allUsers = _userContext.Users.ToList();

            //Create view model and fill it with initial info
            TopicDetailViewModel model = new TopicDetailViewModel
            {
                Topic = new TopicViewModel
                {
                    Topic = topic,
                    Author = allUsers.Where(x => x.UserAccountId == topic.UserAccountId).FirstOrDefault(),
                    IsMutable = currentUser != null && currentUser.IsAuthorized(topic) || User.IsInRole(Constants.Role.Admin)
                }
            };
            //Convert each post to it's related view model and add it to the parent view model
            foreach (Post post in topic.Posts)
                if (post.ContentType.Equals(Constants.ContentType.Post))
                {
                    model.Posts.Add(new PostViewModel
                    {
                        Post = post,
                        IsQuickView = true,
                        Author = allUsers.Where(x => x.UserAccountId == post.UserAccountId).FirstOrDefault(),
                        LikeCount = post.Likes.Count,
                        CommentCount = post.Comments.Count,
                        IsMutable = currentUser != null && currentUser.IsAuthorized(post) || User.IsInRole(Constants.Role.Admin),
                        IsLiked = currentUser != null && post.Likes.Where(x => x.UserAccountId == currentUser.UserAccountId).Count() != 0,
                        IsAuthenticated = currentUser != null
                    });
                }
            return View(model);
        }
        #endregion

        #region Create
        /// <summary>
        /// Create get method for topic
        /// </summary>
        /// <returns>View</returns>
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            //Get currently logged in user and check if they exist
            User currentUser = await _userManager.GetUserAsync(User);
            if (!currentUser.IsAuthenticated())
            {
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }
            //Create and fill viewmodel
            TopicEditorViewModel model = new TopicEditorViewModel();
            model.EditorType = Constants.EditorType.Create;

            return View(model);
        }
        /// <summary>
        /// Create post method for topic
        /// </summary>
        /// <param name="model"></param>
        /// <returns>View</returns>
        [HttpPost]
        public async Task<IActionResult> Create(TopicEditorViewModel model)
        {
            if (ModelState.IsValid)
            {
                //Get currently logged in user and check if they exist
                User currentUser = await _userManager.GetUserAsync(User);
                if (!currentUser.IsAuthenticated())
                {
                    return RedirectToPage("/Account/Login", new { area = "Identity" });
                }
                //Get current users UserAccount 
                UserAccount currentUserAccount = _dataBitesContext.Users.Where(x => x.UserAccountId == currentUser.UserAccountId).FirstOrDefault();

                //Fill the topic and save to database
                Topic topic = FillTopic(model.Topic, currentUserAccount);

                currentUserAccount.Topics.Add(topic);
                _dataBitesContext.SaveChanges();

                //Redirect to the new topic
                return RedirectToAction("Index", "Topic", new { id = topic.TopicId });
            }
            return View(model);
        }
        #endregion

        #region Edit
        /// <summary>
        /// Edit get method for topic
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            //Get currently logged in user and check if they exist
            User currentUser = await _userManager.GetUserAsync(User);
            if (!currentUser.IsAuthenticated())
            {
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }
            
            //Get the topic and check if it exists
            Topic topic = _dataBitesContext.Topics.Where(x => x.TopicId == id).FirstOrDefault();
            if(topic == null)
            {
                return NotFound();
            }

            //Check if the current user has access to this topic
            if (!currentUser.IsAuthorized(topic) && !User.IsInRole(Constants.Role.Admin))
            {
                return Forbid();
            }

            //Create and fill view model
            TopicEditorViewModel model = new TopicEditorViewModel();
            model.Topic = topic;
            model.EditorType = Constants.EditorType.Edit;

            //Return create view with edit config
            return View("Create", model);
        }
        /// <summary>
        /// Edit post method for topic
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Edit(TopicEditorViewModel model)
        {
            if (ModelState.IsValid)
            {
                //Get currently logged in user and check if they exist
                User currentUser = await _userManager.GetUserAsync(User);
                if (!currentUser.IsAuthenticated())
                {
                    return RedirectToPage("/Account/Login", new { area = "Identity" });
                }

                //Get existing topic and check if it exists
                Topic topic = _dataBitesContext.Topics.Where(x => x.TopicId == model.Topic.TopicId).FirstOrDefault();
                if(topic == null)
                {
                    return NotFound();
                }

                //Check if current user has access to this topic
                if (!currentUser.IsAuthorized(topic) && !User.IsInRole(Constants.Role.Admin))
                {
                    return Forbid();
                }

                //Fill topic with user input and save to database
                topic.Title = model.Topic.Title;
                topic.Description = model.Topic.Description;
                _dataBitesContext.SaveChanges();

                //Redirect to topic detail for edited topic
                return RedirectToAction("Index", "Topic", new { id = topic.TopicId });
            }
            //Setup view model for editing and return with errors
            model.EditorType = Constants.EditorType.Edit;
            return View("Create", model);
        }
        #endregion

        #region Delete
        /// <summary>
        /// Delete method for topic
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            //Get currently logged in user and check if they exist
            User currentUser = await _userManager.GetUserAsync(User);
            if (!currentUser.IsAuthenticated())
            {
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }

            //Get topic and check if it exists
            Topic topic = _dataBitesContext.Topics.Where(x => x.TopicId == id).FirstOrDefault();
            if(topic == null)
            {
                return NotFound();
            }

            //Check if current user has access to this topic
            if (!currentUser.IsAuthorized(topic) && !User.IsInRole(Constants.Role.Admin))
            {
                return Forbid();
            }

            _dataBitesContext.Topics.Remove(topic);
            _dataBitesContext.SaveChanges();

            //Return url for redirecting
            return Json(Url.Action("Index", "Home"));
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Fill topic object
        /// </summary>
        /// <param name="topic">Topic object</param>
        /// <param name="user">User object</param>
        /// <returns>Topic object</returns>
        private Topic FillTopic(Topic topic, UserAccount user)
        {
            return new Topic
            {
                Title = topic.Title,
                Description = topic.Description,
                UserAccountId = user.UserAccountId,
                UserAccount = user,
                DateCreated = DateTime.Now
            };
        }
        #endregion
    }
}
