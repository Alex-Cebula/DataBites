using CIS341_project_cebula.Areas.Identity.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CIS341_project_cebula.Models
{
    //View model for report
    public class ReportViewModel
    {
        public Report Report { get; set; }
        public User User { get; set; }
        public string ContentType { get; set; }
    }
}
