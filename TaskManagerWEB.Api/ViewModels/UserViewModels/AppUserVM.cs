using Microsoft.AspNetCore.Mvc.Rendering;
using TaskManager.Models;

namespace TaskManagerWEB.Api.ViewModels.UserViewModels;

public class AppUserVM
{
    public string Email { get; set; }
    public string Id { get; set; }
}
