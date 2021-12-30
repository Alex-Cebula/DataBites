using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CIS341_project_cebula.Models
{
    public class UserAccount
    {
        //EFCore model for useraccount
        public int UserAccountId{ get; set; } //PK
        public bool Suspended { get; set; }

        //Navigation properties
        public ICollection<Topic> Topics { get; set; } = new List<Topic>();
        public ICollection<Post> Posts { get; set; } = new List<Post>();
        public ICollection<Like> Likes { get; set; } = new List<Like>();
        public ICollection<Report> Reports { get; set; } = new List<Report>();
    }
}
