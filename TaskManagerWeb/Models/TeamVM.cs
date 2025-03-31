using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using TaskManager.DataAccess.Entities;

namespace TaskManagerWeb.Models
{
    public class TeamVM
    {
        public Team Team { get; set; }
        public IEnumerable<SelectListItem>? Users { get; set; }
        [ValidateNever]
        public IEnumerable<TaskItem>? TaskItems { get; set; }
        public List<string>? SelectedUserIds { get; set; }
    }
}
