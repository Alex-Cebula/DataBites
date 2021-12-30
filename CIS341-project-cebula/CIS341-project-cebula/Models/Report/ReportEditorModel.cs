using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CIS341_project_cebula.Models
{
    //View model for report editor view
    public class ReportEditorModel
    {
        public Report Report { get; set; } = new Report();
        public string ContentType { get; set; }
        public string ReturnUrl { get; set; }
    }
}
