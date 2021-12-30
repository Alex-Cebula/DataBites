using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CIS341_project_cebula.Models
{
    //EFCore model for post
    public class Post
    {
        public int PostId { get; set; }//PK
        public int UserAccountId { get; set; }//FK
        public int TopicId { get; set; }//FK
        public int? ParentPostId { get; set; }//FK
        public int? RootPostId { get; set; }
        [DisplayName("Title")]
        [MaxLength(75)]
        public string Title { get; set; } = null;
        [Required()]
        [DisplayName("Body")]
        [MaxLength(4000, ErrorMessage = "A post may not exceed 4,000 characters!")]
        public string Body { get; set; }
        public string ContentType { get; set; }
        public DateTime DateCreated { get; set; }

        //Navigation properties
        public ICollection<Like> Likes { get; set; } = new List<Like>();
        public ICollection<Post> Comments { get; set; } = new List<Post>();
        public ICollection<Post> Replies { get; set; } = new List<Post>();
        public UserAccount UserAccount { get; set; }
        public Topic Topic { get; set; }
        public Post ParentPost { get; set; } = null;
        public Post RootPost { get; set; } = null;
    }
}
