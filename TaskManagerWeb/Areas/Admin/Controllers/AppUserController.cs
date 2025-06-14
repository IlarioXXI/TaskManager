﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using TaskManager.DataAccess;
using TaskManager.Models;
using TaskManagerWeb.Models;

namespace TaskManagerWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AppUserController : Controller
    {
        private readonly AppDbContext _db;


        public AppUserController(AppDbContext db)
        {
            _db = db;

        }
        public IActionResult Index()
        {
            ICollection<AppUser> users = _db.AppUsers.ToList();
            var userRoles = _db.UserRoles.ToList();
            var roles = _db.Roles.ToList();
            List<AppUserVM> appUsers = new List<AppUserVM>();
            foreach (var user in users)
            {
                //if (user.Role == null)
                //{
                //    return NotFound();
                //}
                var roleId = userRoles.FirstOrDefault(u => u.UserId == user.Id).RoleId;
                user.Role = roles.FirstOrDefault(u => u.Id == roleId).Name;
                appUsers.Add(new AppUserVM
                {
                    AppUser = user,
                    Roles = _db.Roles.Select(s => new SelectListItem
                    {
                        Text = s.Name,
                        Value = s.Id.ToString()
                    }),
                    RoleSelectedId = roleId
                });
            }
            return View(appUsers);
        }

        [HttpPost]
        public IActionResult Index(List<AppUserVM> appUserVMs)
        {
            foreach (var appUserVM in appUserVMs)
            {
                var appUser = _db.AppUsers.FirstOrDefault(u => u.Id == appUserVM.AppUser.Id);
                var userRolesToRemove = _db.UserRoles.FirstOrDefault(u => u.UserId == appUser.Id);
                _db.UserRoles.Remove(userRolesToRemove);
                _db.UserRoles.Add(new IdentityUserRole<string>
                {
                    UserId = appUser.Id,
                    RoleId = appUserVM.RoleSelectedId
                });

                _db.SaveChanges();
            }
            return RedirectToAction("Index");
        }
    }
}

