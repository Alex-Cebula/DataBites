using CIS341_project_cebula.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CIS341_project_cebula.Models
{
    //View model tat holds posts for home page
    public class HomeViewModel
    {
        public ICollection<PostViewModel> Posts { get; set; } = new List<PostViewModel>();
    }
}
