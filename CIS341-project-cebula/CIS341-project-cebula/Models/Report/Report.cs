using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CIS341_project_cebula.Models
{
    //EFCore model for report
    public class Report
    {
        public int ReportId { get; set; }
        public int ContentId { get; set; }
        public string ContentType { get; set; }
        public int? UserAccountId { get; set; }
        [Required]
        [DisplayName("Report Content")]
        [MaxLength(4000)]
        public string Body { get; set; }
        public DateTime DateCreated { get; set; }

        //Navigation properties
        public UserAccount UserAccount { get; set; }
    }
}
