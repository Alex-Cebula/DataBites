using CIS341_project_cebula.Data;
using CIS341_project_cebula.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CIS341_project_cebula.ViewComponents
{
    public class CommentEditorViewComponent : ViewComponent
    {
        private readonly UserContext _userContext;
        private readonly DataBitesContext _dataBitesContext;
        public CommentEditorViewComponent(UserContext userContext, DataBitesContext dataBitesContext) {
            _userContext = userContext;
            _dataBitesContext = dataBitesContext;
        }
        //Invoker for comment editor view component
        public async Task<IViewComponentResult> InvokeAsync(int postId, string editorType, Post post)
        {
            //Get parent post of comment
            Post parentPost = _dataBitesContext.Posts.Where(x => x.PostId == postId).FirstOrDefault();

            //Fill view model
            PostEditorViewModel model = new PostEditorViewModel();
            model.ParentAuthor = _userContext.Users.Where(x => x.UserAccountId == parentPost.UserAccountId).FirstOrDefault();
            model.ParentContentType = parentPost.ContentType;
            model.EditorType = editorType;
            model.Post = post != null ? post : new Post();
            model.Post.ParentPostId = postId;

            return View(model);
        }
    }
}
