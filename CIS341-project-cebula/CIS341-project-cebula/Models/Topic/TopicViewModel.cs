using CIS341_project_cebula.Areas.Identity.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CIS341_project_cebula.Models
{
    //View model for topic
    public class TopicViewModel
    {
        public User Author { get; set; }
        public Topic Topic { get; set; }
        public bool IsMutable { get; set; }
        public bool IsQuickView { get; set; }
    }
}
