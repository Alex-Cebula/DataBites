using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CIS341_project_cebula.Models
{
    //View model for reports view
    public class ReportsViewModel
    {
        public List<ReportViewModel> Reports { get; set; } = new List<ReportViewModel>();
    }
}
