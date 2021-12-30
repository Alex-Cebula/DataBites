using CIS341_project_cebula.Areas.Identity.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CIS341_project_cebula.Models
{
    public class Topic
    {
        //EFCore model for topic
        public int TopicId { get; set; }//PK
        public int UserAccountId { get; set; }
        [Required]
        [DisplayName("Topic Title")]
        [MaxLength(75)]
        public string Title { get; set; }
        [Required]
        [DisplayName("Topic Content")]
        [MaxLength(500)]
        public string Description { get; set; }
        public DateTime DateCreated { get; set; }

        //Navigation properties
        public ICollection<Post> Posts { get; set; } = new List<Post>();
        public UserAccount UserAccount { get; set; }
    }
}
