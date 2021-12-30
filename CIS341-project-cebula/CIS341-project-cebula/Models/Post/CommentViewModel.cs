using CIS341_project_cebula.Areas.Identity.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CIS341_project_cebula.Models
{
    //View model for comment partial view
    public class CommentViewModel
    {
        public User Author { get; set; }
        public Post Comment { get; set; }
        public int LikeCount { get; set; }
        public int ReplyCount { get; set; }
        public bool IsMutable { get; set; }
        public bool IsLiked { get; set; }
        public bool IsAuthenticated { get; set; }
    }
}
