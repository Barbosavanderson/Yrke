using Microsoft.EntityFrameworkCore;
using Yrke.Models;

namespace Yrke.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<User> Users { get; set; }
    }

}
