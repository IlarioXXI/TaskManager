using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Models;

namespace TaskManagerWEB.Api.Validators
{
    public class CommentValidation : AbstractValidator<Comment>
    {
        public CommentValidation()
        {
            RuleFor(x => x.Description).NotNull();
            RuleFor(x => x.CreationDate).Empty();
            RuleFor(x => x.TaskItemId).NotNull();
            RuleFor(x => x.TaskItem).Null();
            RuleFor(x => x.AppUserId).Null();
            RuleFor(x => x.AppUser).Null();
        }
    }
}
