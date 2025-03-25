using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManager.DataAccess.Entities
{
    public class AppUser : IdentityUser
    {
        [Required]
        public string Name { get; set; }
        public string? Avatar { get; set; }

    }
}
