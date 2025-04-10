
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace TaskManager.Models
{
    public class AppUser : IdentityUser
    {
        [AllowNull]
        public string? Name { get; set; }


        [JsonIgnore]
        public List<Team> Teams { get; set; } = [];

        [JsonIgnore]
        [NotMapped]
        [AllowNull]
        public string? Role { get; set; }
    }

    
}
