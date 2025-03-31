using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskManager.DataAccess.Entities
{
    public class AppUser : IdentityUser
    {
        [Required]
        public string Name { get; set; }
        public string? Avatar { get; set; }

        public List<Team> Teams { get; set; } = [];
        [NotMapped]
        public string Role { get; set; }
    }
}
