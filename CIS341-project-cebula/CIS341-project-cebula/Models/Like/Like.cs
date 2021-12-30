using CIS341_project_cebula.Areas.Identity.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CIS341_project_cebula.Models
{
    //EFCore model for like
    public class Like
    {
        public int LikeId { get; set; }//PK
        public int UserAccountId { get; set; }//FK
        public int PostId { get; set; }//FK
        public DateTime DateCreated { get; set; }

        //Navgation propertes
        public UserAccount UserAccount { get; set; }
        public Post Post { get; set; }

    }
}
