using CIS341_project_cebula.Areas.Identity.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CIS341_project_cebula.Models
{
    //View model for a post's partial view
    public class PostViewModel
    {
        public PostViewModel()
        {
            ShouldShowTopic = false;
            IsQuickView = false;
        }
        public User Author { get; set; }
        public Post Post { get; set; }
        public ICollection<CommentViewModel> Comments { get; set; } = new List<CommentViewModel>();
        public int LikeCount { get; set; } = 0;
        public int CommentCount { get; set; } = 0;
        public bool ShouldShowTopic { get; set; }
        public bool IsQuickView { get; set; }
        public bool IsMutable { get; set; }
        public bool IsLiked { get; set; }
        public bool IsAuthenticated { get; set; }
    }
}
