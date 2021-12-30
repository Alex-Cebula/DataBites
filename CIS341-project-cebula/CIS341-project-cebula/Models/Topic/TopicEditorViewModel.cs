using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CIS341_project_cebula.Models
{
    //View model for topic editor view
    public class TopicEditorViewModel
    {
        public Topic Topic { get; set; } = new Topic();
        public string EditorType { get; set; }
    }
}
