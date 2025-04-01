﻿using Microsoft.AspNetCore.Mvc.Rendering;
using TaskManager.DataAccess.Entities;

namespace TaskManagerWeb.Models;

public class AppUserVM
{
    public AppUser AppUser { get; set; }
    public IEnumerable<SelectListItem> Roles { get; set; }
    public string? RoleSelectedId { get; set; }
}
