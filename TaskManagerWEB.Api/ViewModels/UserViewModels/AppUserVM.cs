using Microsoft.AspNetCore.Mvc.Rendering;
using TaskManager.Models;

namespace TaskManagerWEB.Api.ViewModels.UserViewModels;

public class AppUserVM
{
    public AppUser AppUser { get; set; }
    public IEnumerable<SelectListItem> Roles { get; set; }
    public string? RoleSelectedId { get; set; }
}
