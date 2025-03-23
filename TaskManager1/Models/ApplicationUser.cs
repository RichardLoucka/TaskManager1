using Microsoft.AspNetCore.Identity;

namespace TaskManager1.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; }
    }
}