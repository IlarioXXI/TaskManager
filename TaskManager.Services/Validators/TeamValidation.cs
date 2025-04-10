using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Models;
using TaskManagerWeb.Models;

namespace TaskManager.Services.Validators
{
    public class TeamValidation : AbstractValidator<TeamVM>
    {
        public TeamValidation()
        {
            RuleFor(x => x.Users).Null();
            RuleFor(x => x.TaskItems).Null();
            RuleFor(x => x.SelectedUserIds).NotNull();
            RuleFor(x => x.Team).NotNull();
        }
    }
}
