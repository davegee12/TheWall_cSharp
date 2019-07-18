using TheWall_cSharp.Models;
using Microsoft.EntityFrameworkCore;

namespace TheWall_cSharp
{
    public class MyContext : DbContext
    {
        public MyContext(DbContextOptions options) : base(options) {}

        public DbSet<RegUser> Users {get;set;}
        public DbSet<Message> Messages {get;set;}
        public DbSet<Comment> Comments {get;set;}
        public DbSet<MessageLike> MessageLikes {get;set;}
        public DbSet<CommentLike> CommentLikes {get;set;}

    }
}