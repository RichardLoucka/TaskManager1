using Microsoft.EntityFrameworkCore;

namespace TaskManager1.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options){}
        public DbSet<Task> Tasks { get; set; }
    }
}

