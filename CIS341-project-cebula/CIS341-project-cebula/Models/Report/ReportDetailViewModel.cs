using CIS341_project_cebula.Areas.Identity.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CIS341_project_cebula.Models
{
    //View model for report detail view
    public class ReportDetailViewModel
    {
        public Report Report { get; set; }
        //Display text
        public string ContentType { get; set; }
        public User ReportedUser { get; set; }
        public Object ReportedContent { get; set; }
        public User ReporteeUser { get; set; }
    }
}
