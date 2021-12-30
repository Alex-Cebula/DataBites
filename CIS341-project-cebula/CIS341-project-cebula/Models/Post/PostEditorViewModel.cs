using CIS341_project_cebula.Areas.Identity.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CIS341_project_cebula.Models
{
    //Vew model for post editor views
    public class PostEditorViewModel
    {
        public Post Post { get; set; } = new Post();
        public string EditorType { get; set; }
        public User ParentAuthor { get; set; }
        public string ParentContentType { get; set; }
    }
}
