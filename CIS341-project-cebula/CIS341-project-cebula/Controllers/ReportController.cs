using CIS341_project_cebula.Areas.Identity.Data;
using CIS341_project_cebula.Data;
using CIS341_project_cebula.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CIS341_project_cebula.Controllers
{
    /// <summary>
    /// Report controller
    /// </summary>
    public class ReportController : Controller
    {
        private readonly DataBitesContext _dataBitesContext;
        private readonly UserManager<User> _userManager;
        private readonly UserContext _userContext;
        public ReportController(DataBitesContext dataBitesContext, UserContext userContext, UserManager<User> userManager)
        {
            _dataBitesContext = dataBitesContext;
            _userManager = userManager;
            _userContext = userContext;
        }

        #region Read
        /// <summary>
        /// View for all reports for admins
        /// </summary>
        /// <returns>View</returns>
        [Authorize(Roles = "ADMIN")]
        [HttpGet()]
        public IActionResult Index()
        {
            //Get all users and reports
            List<User> allUsers = _userContext.Users.ToList();
            List<Report> reports = _dataBitesContext.Reports.ToList();

            ReportsViewModel model = new ReportsViewModel();
            //Convert reports to view model
            foreach (Report report in reports)
            {
                model.Reports.Add(new ReportViewModel
                {
                    Report = report,
                    User = allUsers.Where(x => x.UserAccountId == report.UserAccountId).FirstOrDefault(),
                    ContentType = GetDisplayType(report.ContentType)
                });
            }
            return View(model);
        }
        /// <summary>
        /// View for specific report
        /// </summary>
        /// <param name="id"></param>
        /// <returns>View</returns>
        [Authorize(Roles = "ADMIN")]
        [HttpGet("Report/{id}")]
        public IActionResult Index(int id)
        {
            //Get report and check if it exists
            Report report = _dataBitesContext.Reports.Where(x => x.ReportId == id).FirstOrDefault();
            if (report == null)
            {
                return NotFound();
            }
            //Fill view model
            ReportDetailViewModel model = new ReportDetailViewModel();
            model.Report = report;
            var content = GetContent(report.ContentId, report.ContentType);
            if (content != null)
            {
                model.ReportedContent = content;
                model.ReportedUser = _userContext.Users.Where(x => x.UserAccountId == (int)model.ReportedContent.GetType().GetProperty(Constants.Reflection.ContentProperty.UserAccountId).GetValue(model.ReportedContent)).FirstOrDefault();
            }
            model.ReporteeUser = _userContext.Users.Where(x => x.UserAccountId == report.UserAccountId).FirstOrDefault();
            model.ContentType = GetDisplayType(report.ContentType);

            return View("ReportDetail", model);
        }
        #endregion

        #region Create
        /// <summary>
        /// Create get method for report
        /// </summary>
        /// <param name="contentId">generic id</param>
        /// <param name="contentType">generic type</param>
        /// <param name="returnUrl"></param>
        /// <returns>View</returns>
        [HttpGet]
        public IActionResult Create(int contentId, string contentType, string returnUrl)
        {
            //Get the content and check if it exists
            var content = GetContent(contentId, contentType);
            if (content == null)
            {
                return NotFound();
            }
            //Fill view model
            ReportEditorModel model = new ReportEditorModel();
            model.ReturnUrl = returnUrl;
            model.ContentType = GetDisplayType(contentType);
            model.Report.ContentId = contentId;
            model.Report.ContentType = contentType;

            return View(model);
        }
        /// <summary>
        /// Create post method for report
        /// </summary>
        /// <param name="model"></param>
        /// <returns>View</returns>
        [HttpPost]
        public async Task<IActionResult> Create(ReportEditorModel model)
        {
            if (ModelState.IsValid)
            {
                //Get content and check if it exists
                var content = GetContent(model.Report.ContentId, model.Report.ContentType);
                if (content == null)
                {
                    return NotFound();
                }
                //Get the current user and check if they are logged in
                User currentUser = await _userManager.GetUserAsync(User);
                if (currentUser.IsAuthenticated())
                {
                    //If logged in then link the user's UserAccount object to the report
                    UserAccount currentUserAccount = _dataBitesContext.Users.Where(x => x.UserAccountId == currentUser.UserAccountId).FirstOrDefault();
                    model.Report.UserAccountId = currentUserAccount.UserAccountId;
                    model.Report.UserAccount = currentUserAccount;
                }
                model.Report.DateCreated = DateTime.Now;
                
                _dataBitesContext.Reports.Add(model.Report);
                _dataBitesContext.SaveChanges();

                //If return url is not null return to it, else return to homepage
                if (model.ReturnUrl != null)
                {
                    return Redirect(model.ReturnUrl);
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            //Set content type and return to create view with errors
            model.ContentType = GetDisplayType(model.Report.ContentType);
            return View(model);
        }
        #endregion

        #region Delete
        /// <summary>
        /// Delete method for report
        /// </summary>
        /// <param name="id"></param>
        /// <returns>View</returns>
        [Authorize(Roles = "ADMIN")]
        [HttpDelete]
        public IActionResult Resolve(int id)
        {
            //Get the report and check if it exists
            Report report = _dataBitesContext.Reports.Where(x => x.ReportId == id).FirstOrDefault();
            if (report == null)
            {
                return NotFound();
            }

            _dataBitesContext.Reports.Remove(report);
            _dataBitesContext.SaveChanges();

            //Return url to redirect to
            return Json(Url.Action("Index", "Report"));
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Get's formatted content type text
        /// </summary>
        /// <param name="contentType"></param>
        /// <returns>string</returns>
        private string GetDisplayType(string contentType)
        {
            if (contentType.Equals(Constants.ContentType.Topic))
            {
                return "Topic";
            }
            else if (contentType.Equals(Constants.ContentType.Post))
            {
                return "Post";
            }
            else
            {
                return "Comment";
            }
        }
        /// <summary>
        /// Gets the content object based on it's type
        /// </summary>
        /// <param name="contentId"></param>
        /// <param name="contentType"></param>
        /// <returns>Type of content</returns>
        private Object GetContent(int contentId, string contentType)
        {
            //If it is a topic, query for the topic
            if (contentType.Equals(Constants.ContentType.Topic))
            {
                return _dataBitesContext.Topics.Where(x => x.TopicId == contentId).FirstOrDefault();
            }
            //If it is a post query for the post
            else if (contentType.Equals(Constants.ContentType.Post))
            {
                return _dataBitesContext.Posts.Where(x => x.PostId == contentId && x.ContentType.Equals(Constants.ContentType.Post)).FirstOrDefault();
            }
            //If it is a comment, query for the comment
            else
            {
                return _dataBitesContext.Posts.Where(x => x.PostId == contentId && x.ContentType != Constants.ContentType.Post).FirstOrDefault();
            }
        }
        #endregion
    }
}