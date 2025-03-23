using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace TaskManager1.Models
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options){}
        public DbSet<Task> Tasks { get; set; }
        public DbSet<Category> Categories { get; set; }

        public static async System.Threading.Tasks.Task Initialize(IServiceProvider serviceProvider, bool test = false)
        {
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();
            if (!context.Categories.Any())
            {
                context.Categories.AddRange(
                    new Category{Name = "Work"},
                    new Category{Name = "Personal"},
                    new Category{Name = "Main"},
                    new Category{Name = "Side"},
                    new Category{Name = "Urgent"});
                await context.SaveChangesAsync();
            }
        }
    }
}

