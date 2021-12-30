using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CIS341_project_cebula.Models
{
    //View model for post detail view
    public class PostDetailViewModel
    {
        public PostViewModel Post { get; set; } = new PostViewModel();
        public ICollection<CommentViewModel> Comments { get; set; } = new List<CommentViewModel>();
        public bool loadEditor { get; set; }
        public PostEditorViewModel EditorContent { get; set; } = new PostEditorViewModel();
    }
}
