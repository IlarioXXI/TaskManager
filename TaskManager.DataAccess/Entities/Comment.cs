using FluentValidation;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManager.DataAccess.Entities
{
    public class Comment
    {
        [Key]
        public int Id { get; set; }
        
        public string Description { get; set; }
        public DateTime CreationDate { get; set; }

        public int TaskItemId { get; set; }

        [ForeignKey("TaskItemId")]
        public TaskItem? TaskItem { get; set; }

        public string? AppUserId { get; set; }
        [ForeignKey("AppUserId")]
        public AppUser? AppUser { get; set; }

    }

    public class CommentValidation : AbstractValidator<Comment>
    {
        public CommentValidation() 
        {
            RuleFor(x => x.Description).NotNull();
            RuleFor(x => x.CreationDate).Empty();
            RuleFor(x=>x.TaskItemId).NotNull();
            RuleFor(x => x.TaskItem).Null();
            RuleFor(x => x.AppUserId).Null();
            RuleFor(x => x.AppUser).Null();
        }
    }
}
