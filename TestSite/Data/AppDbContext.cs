using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TestSite.Models;
using TestSite.Models.Comments;

namespace TestSite.Data
{
    public class AppDbContext : IdentityDbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }
        public DbSet<Post> Posts { get; set; }
        public DbSet<MainComment> MainCommnets { get; set; }
        public DbSet<SubComment> SubComments { get; set; }

    }
}
