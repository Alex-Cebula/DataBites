using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CIS341_project_cebula.Models
{
    //Vew model for topics view
    public class TopicsViewModel
    {
        public List<TopicViewModel> Topics { get; set; } = new List<TopicViewModel>();
    }
}
