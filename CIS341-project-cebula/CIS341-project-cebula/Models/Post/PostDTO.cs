using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CIS341_project_cebula.Models
{
    //Data transfer object for post api
    public class PostDTO
    {
        public int Id { get; set; }
        public string Topic { get; set; }
        public int TopicId { get; set; }
        public string Author { get; set; }
        public int AuthorId { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public string ContentType { get; set; }
    }
}
