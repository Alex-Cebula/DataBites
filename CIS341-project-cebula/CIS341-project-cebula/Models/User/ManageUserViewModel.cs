using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CIS341_project_cebula.Models
{
    //View model for manage users view
    public class ManageUserViewModel
    {
        public List<UserViewModel> Users { get; set; } = new List<UserViewModel>();
    }
}
