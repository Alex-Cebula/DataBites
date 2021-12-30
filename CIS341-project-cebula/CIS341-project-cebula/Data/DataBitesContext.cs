using CIS341_project_cebula.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CIS341_project_cebula.Data
{
    public class DataBitesContext : DbContext
    {
        public DataBitesContext(DbContextOptions<DataBitesContext> options) : base(options)
        {

        }
        public DbSet<UserAccount> Users { get; set; }
        public DbSet<Topic> Topics { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Like> Likes { get; set; }
        public DbSet<Report> Reports { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            //Comments <=> RootPost
            builder.Entity<Post>()
                .HasMany(x => x.Comments)
                .WithOne(x => x.RootPost)
                .HasForeignKey(x => x.RootPostId)
                .OnDelete(DeleteBehavior.NoAction);
            //Replies <=> ParentPost
            builder.Entity<Post>()
                .HasMany(x => x.Replies)
                .WithOne(x => x.ParentPost)
                .HasForeignKey(x => x.ParentPostId)
                .OnDelete(DeleteBehavior.NoAction);
            //UserAccount <=> Posts
            builder.Entity<Post>()
                .HasOne(x => x.UserAccount)
                .WithMany(x => x.Posts)
                .HasForeignKey(x => x.UserAccountId)
                .OnDelete(DeleteBehavior.NoAction);
            //UserAccount <=> Topics
            builder.Entity<Topic>()
                .HasOne(x => x.UserAccount)
                .WithMany(x => x.Topics)
                .HasForeignKey(x => x.UserAccountId)
                .OnDelete(DeleteBehavior.NoAction);
            //UserAccount <=> Likes
            builder.Entity<Like>()
                .HasOne(x => x.UserAccount)
                .WithMany(x => x.Likes)
                .HasForeignKey(x => x.UserAccountId)
                .OnDelete(DeleteBehavior.NoAction);
            //UserAccount <=> Reports
            builder.Entity<Report>()
                .HasOne(x => x.UserAccount)
                .WithMany(x => x.Reports)
                .HasForeignKey(x => x.UserAccountId)
                .OnDelete(DeleteBehavior.NoAction);

            //Entities <=> UserAccount
            builder.Entity<UserAccount>(x =>
            {
                x.HasMany(x => x.Posts).WithOne(x => x.UserAccount);
                x.HasMany(x => x.Topics).WithOne(x => x.UserAccount);
                x.HasMany(x => x.Likes).WithOne(x => x.UserAccount);
                x.HasMany(x => x.Reports).WithOne(x => x.UserAccount);
            });
            base.OnModelCreating(builder);
        }
    }
}
