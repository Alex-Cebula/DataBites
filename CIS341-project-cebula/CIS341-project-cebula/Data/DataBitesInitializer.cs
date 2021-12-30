using CIS341_project_cebula.Areas.Identity.Data;
using CIS341_project_cebula.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CIS341_project_cebula.Data
{
    public static class DataBitesInitializer
    {
        public static void Initialize(DataBitesContext dataBitesContext, UserContext userContext)
        {
            dataBitesContext.Database.Migrate();
            userContext.Database.Migrate();

            if (dataBitesContext.Users.Count() > 0)
                return;
            UserAccount user = new UserAccount()
            {
                Topics = new List<Topic>(),
                Posts = new List<Post>(),
                Likes = new List<Like>()
            };
            dataBitesContext.Add(user);
            dataBitesContext.SaveChanges();

            Topic topic = new Topic
            {
                Title = "Habberdashery",
                Description = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Nunc tempor bibendum hendrerit.",
                UserAccountId = user.UserAccountId,
                UserAccount = user,
                DateCreated = DateTime.Now,
                Posts = new List<Post>() 
            };
            user.Topics.Add(topic);
            dataBitesContext.SaveChanges();

            Post post = new Post
            {
                Title = "Habberdashery in your area?",
                Body = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Nunc tempor bibendum hendrerit. Praesent bibendum ante quis nulla elementum posuere nec in dolor. Suspendisse justo purus, fringilla non lobortis viverra, condimentum non sem. Nunc posuere, risus nec faucibus tristique, velit massa porttitor odio, nec varius quam lectus ut nisi. In lacinia mi sit amet porta tempus. Praesent porta imperdiet efficitur. Sed eu posuere erat. Praesent malesuada suscipit ultricies. Fusce at ante tristique, dictum nisl eu, iaculis enim. Donec ornare sem est, a iaculis arcu commodo non. Aenean consectetur, magna hendrerit tristique ultricies, nisl neque vestibulum nunc, quis rutrum ex est a metus. Donec vitae consequat erat. Suspendisse laoreet est ut purus tempor, quis vulputate ante malesuada. Ut hendrerit, ex sed efficitur auctor, metus magna posuere dui, non bibendum orci lectus quis mi. Pellentesque habitant morbi tristique senectus et netus et malesuada fames ac turpis egestas. Quisque ut ex sit amet quam finibus vehicula id ac mi. Proin erat justo, pretium et nisi vel, efficitur posuere lectus. Maecenas luctus placerat ligula sed efficitur. Mauris bibendum elementum felis, sit amet iaculis est pharetra sed. Integer sed risus nec ante semper vehicula. Fusce vitae tempus augue. Nulla semper metus at purus malesuada, vel consequat enim posuere. Sed ornare, ligula vitae imperdiet tincidunt, ex ipsum gravida lorem, vel maximus ipsum ligula et nulla. Donec vitae magna sit amet enim facilisis faucibus. Etiam tincidunt lectus sit amet orci semper, ac aliquet neque venenatis. Nullam egestas suscipit sapien sit amet aliquam. Pellentesque maximus lorem maximus velit imperdiet, eget commodo lorem commodo. Vivamus hendrerit dictum leo, eu vehicula nunc eleifend tristique. Vestibulum auctor, justo non tristique maximus, diam massa molestie ante, ac mollis odio magna nec felis. Maecenas nulla risus, convallis eget viverra ac, finibus a ex. Aenean euismod metus vitae diam malesuada, quis dignissim ipsum placerat. Morbi vehicula magna quis ipsum sodales auctor. Pellentesque mollis eleifend dolor. Nullam diam arcu, tincidunt a enim vitae, scelerisque euismod risus. Pellentesque placerat fringilla mi dapibus scelerisque. Aliquam molestie nisi in ultricies tempus. Cras a sem arcu. Nullam ipsum mi, ullamcorper sed aliquet sed, tristique non urna. Fusce imperdiet nisi sit amet dignissim dapibus. Suspendisse interdum tortor vitae ligula auctor blandit. Fusce et magna sed tellus sagittis fermentum ut non ligula. Proin varius vel lorem ut eleifend. Fusce justo orci, consequat ut sem vitae, dictum tempor diam. Fusce iaculis mauris eu tincidunt vestibulum. Pellentesque consequat lorem id faucibus tristique. Nunc scelerisque sapien porttitor enim euismod, ut tristique metus scelerisque. Cras viverra sem leo, eget dictum sapien sagittis nec. Proin fermentum sit amet sem nec dictum. Ut vel molestie lorem, nec cursus nisl. Fusce sagittis nibh nibh. Donec sodales arcu dolor, id congue diam efficitur non. Etiam rutrum, justo id aliquet tincidunt, purus libero fringilla dolor, non egestas justo libero vel erat. Vestibulum nec hendrerit dolor, non condimentum tortor. Integer vehicula placerat eros nec convallis. Aliquam sed mauris tristique felis tempus auctor. Nunc sed dui feugiat magna iaculis ultricies. Cras dictum consectetur turpis ac fermentum. Phasellus ultricies rhoncus massa, eu egestas felis fringilla quis. Fusce vulputate placerat ex, vitae eleifend lorem fringilla ac. Vivamus lacinia, ligula at sagittis iaculis, tortor lacus suscipit mauris, a bibendum massa massa sit amet libero. Curabitur ornare placerat pulvinar. Aenean porta tempor mi eu laoreet. Integer tellus nunc, faucibus condimentum ex eget, rutrum pulvinar ligula. Mauris vel finibus tellus. Aenean ornare tristique feugiat. Maecenas eget bibendum erat.",
                UserAccountId = user.UserAccountId,
                UserAccount = user,
                DateCreated = DateTime.Now,
                ContentType = Constants.ContentType.Post,
                TopicId = topic.TopicId,
                Topic = topic,
                Likes = new List<Like>(),
                Comments = new List<Post>()
            };
            topic.Posts.Add(post);
            dataBitesContext.SaveChanges();

            Post comment1 = new Post
            {
                TopicId = topic.TopicId,
                Topic = topic,
                Body = "WHAT? FR DOE?",
                UserAccountId = user.UserAccountId,
                UserAccount = user,
                RootPostId = post.PostId,
                RootPost = post,
                ParentPostId = post.PostId,
                ParentPost = post,
                DateCreated = DateTime.Now,
                ContentType = Constants.ContentType.Comment
            };
            post.Comments.Add(comment1);
            post.Replies.Add(comment1);
            dataBitesContext.SaveChanges();

            Post comment2 = new Post
            {
                TopicId = post.TopicId,
                Topic = post.Topic,
                Body = "BBABABABABABABABABABABABABABABABABABABa sheep",
                UserAccountId = user.UserAccountId,
                UserAccount = user,
                RootPostId = post.PostId,
                RootPost = post,
                ParentPostId = post.PostId,
                ParentPost = post,
                DateCreated = DateTime.Now,
                ContentType = Constants.ContentType.Comment
            };
            post.Comments.Add(comment2);
            post.Replies.Add(comment2);
            dataBitesContext.SaveChanges();

            Post reply = new Post
            {
                TopicId = topic.TopicId,
                Topic = topic,
                Body = "What a stupid reply",
                UserAccountId = user.UserAccountId,
                UserAccount = user,
                RootPostId = comment1.RootPostId,
                RootPost = comment1.RootPost,
                ParentPostId = comment1.PostId,
                ParentPost = comment1,
                DateCreated = DateTime.Now,
                ContentType = Constants.ContentType.Reply
            };
            comment1.Replies.Add(reply);
            post.Comments.Add(reply);
            dataBitesContext.SaveChanges();

            Post subReply = new Post
            {
                TopicId = reply.TopicId,
                Topic = reply.Topic,
                Body = "Bubllegum",
                UserAccountId = user.UserAccountId,
                UserAccount = user,
                RootPostId = reply.RootPostId,
                RootPost = reply.RootPost,
                ParentPostId = reply.PostId,
                ParentPost = reply,
                DateCreated = DateTime.Now,
                ContentType = Constants.ContentType.Reply
            };
            reply.Replies.Add(subReply);
            post.Comments.Add(subReply);
            dataBitesContext.SaveChanges();

            Like like = new Like
            {
                UserAccountId = user.UserAccountId,
                UserAccount = user,
                PostId = post.PostId,
                Post = post,
                DateCreated = DateTime.Now
            };
            post.Likes.Add(like);
            dataBitesContext.SaveChanges();

            Report report = new Report
            {
                UserAccountId = user.UserAccountId,
                UserAccount = user,
                Body = "I really don't like this",
                ContentId = post.PostId,
                ContentType = Constants.ContentType.Post,
                DateCreated = DateTime.Now,
            };
            user.Reports.Add(report);
            dataBitesContext.SaveChanges();

            Topic topic2 = new Topic
            {
                UserAccountId = user.UserAccountId,
                UserAccount = user,
                Title = "NO LIKES",
                Description = "Rules: No likes and no comments on your posts",
                DateCreated = DateTime.Now
            };
            user.Topics.Add(topic2);
            dataBitesContext.SaveChanges();

            Post post2 = new Post
            {
                UserAccountId = user.UserAccountId,
                UserAccount = user,
                Title = "Nobody like or comment",
                Body = "If anyone doesn't do what I said in the title, I'm going to be mad man",
                Topic = topic2,
                TopicId = topic2.TopicId,
                DateCreated = DateTime.Now,
                ContentType = Constants.ContentType.Post
            };
            topic2.Posts.Add(post2);
            dataBitesContext.SaveChanges();

            dataBitesContext.Add(new UserAccount());
            dataBitesContext.SaveChanges();
        }
    }
}
