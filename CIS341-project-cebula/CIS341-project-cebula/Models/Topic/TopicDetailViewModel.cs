using CIS341_project_cebula.Areas.Identity.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CIS341_project_cebula.Models
{
    //View model for topic detail view
    public class TopicDetailViewModel
    {
        public TopicViewModel Topic { get; set; }
        public ICollection<PostViewModel> Posts { get; set; } = new List<PostViewModel>();
    }
}
