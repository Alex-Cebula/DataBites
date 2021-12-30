using CIS341_project_cebula.Areas.Identity.Data;
using CIS341_project_cebula.Data;
using CIS341_project_cebula.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CIS341_project_cebula.Controllers
{
    /// <summary>
    /// Post api controller
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class PostAPIController : ControllerBase
    {
        private readonly DataBitesContext _dataBitesContext;
        private readonly UserContext _userContext;
        public PostAPIController(DataBitesContext dataBitesContext, UserContext userContext)
        {
            _dataBitesContext = dataBitesContext;
            _userContext = userContext;
        }
        /// <summary>
        /// Get all posts
        /// </summary>
        /// <returns>Json</returns>
        [HttpGet]
        public async Task<IEnumerable<PostDTO>> GetPosts()
        {
            //Get all posts and their topics
            List<Post> posts = await _dataBitesContext.Posts
                .Include(x => x.Topic).ToListAsync();

            //Get all users
            List<User> allUsers = await _userContext.Users.ToListAsync();

            List<PostDTO> response = new List<PostDTO>();
            //Convert each post to it's data transfer object and add it to a list
            foreach (Post post in posts)
            {
                User user = allUsers.Where(x => x.UserAccountId == post.UserAccountId).FirstOrDefault();
                response.Add(new PostDTO
                {
                    Id = post.PostId,
                    Topic = post.Topic.Title,
                    TopicId = post.TopicId,
                    Author = user.UserName,
                    AuthorId = user.UserAccountId,
                    Title = post.Title,
                    Body = post.Body,
                    ContentType = post.ContentType
                });
            }
            return response.AsEnumerable();
        }
        /// <summary>
        /// Get specific post
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Json</returns>
        [HttpGet("{id}")]
        //Get a single post in json format
        public async Task<ActionResult<PostDTO>> GetPost(int id)
        {
            //Get post and it's topics and check if it exists
            Post post = await _dataBitesContext.Posts.Where(x => x.PostId == id)
                .Include(x => x.Topic)
                .FirstOrDefaultAsync();
            if (post == null)
            {
                return NotFound();
            }
            //Get posts user
            User user = await _userContext.Users.Where(x => x.UserAccountId == post.UserAccountId).FirstOrDefaultAsync();

            //Convert post to it's data transfer object
            return new PostDTO
            {
                Id = post.PostId,
                Topic = post.Topic.Title,
                TopicId = post.TopicId,
                Author = user != null ? user.UserName : "UNKNOWN",
                AuthorId = user.UserAccountId,
                Title = post.Title,
                Body = post.Body,
                ContentType = post.ContentType
            };
        }
        /// <summary>
        /// Delete method for a post
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult<PostDTO>> DeletePost(int id)
        {
            //Get a post and it's nav props and check if it exists
            Post post = await _dataBitesContext.Posts.Where(x => x.PostId == id)
                .Include(x => x.Topic)
                .Include(x => x.Comments)
                .FirstOrDefaultAsync();
            if(post == null)
            {
                return NotFound();
            }
            
            //Get the posts user
            User user = await _userContext.Users.Where(x => x.UserAccountId == post.UserAccountId).FirstOrDefaultAsync();

            //Delete the posts dependancies
            _dataBitesContext.Posts.RemoveRange(post.Comments);

            //Delete the post
            _dataBitesContext.Posts.Remove(post);
            await _dataBitesContext.SaveChangesAsync();

            //Return post as data transfer object
            return new PostDTO
            {
                Id = post.PostId,
                Topic = post.Topic.Title,
                TopicId = post.TopicId,
                Author = user.UserName,
                AuthorId = user.UserAccountId,
                Title = post.Title,
                Body = post.Body,
                ContentType = post.ContentType
            };
        }
    }
}
