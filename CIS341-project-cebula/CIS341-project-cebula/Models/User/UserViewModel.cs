using CIS341_project_cebula.Areas.Identity.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CIS341_project_cebula.Models
{
    //View model for user
    public class UserViewModel
    {
        public User User { get; set; }
        public UserAccount UserAccount { get; set; }
        public bool IsAuthenticated { get; set; }
        public string HighestRole { get; set; }
        public List<string> Roles { get; set; } = new List<string>();
    }
}
