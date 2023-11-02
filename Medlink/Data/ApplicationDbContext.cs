using Medlink.Models;
using Microsoft.EntityFrameworkCore;

namespace Medlink.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
        public DbSet<UserModel> Users { get; set; }
        public DbSet<PostModel> Posts { get; set; }
        public DbSet<LikeModel> Likes { get; set; }
        public DbSet<CommentModel> Comments { get; set; }
    }
}
