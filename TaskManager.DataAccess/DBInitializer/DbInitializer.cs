using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TaskManager.DataAccess.Interfaces;
using TaskManager.DataAccess.Utility;
using TaskManager.Models;

namespace TaskManager.DataAccess.DBInitializer
{
    public class DbInitializer : IDbInitializer
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly AppDbContext _db;

        public DbInitializer(UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            AppDbContext db)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _db = db;
        }
        public void Initialize()
        {
            //migrazioni se non sono ancora state applicate
            try
            {
                if (_db.Database.GetPendingMigrations().Count() > 0)
                {
                    _db.Database.Migrate();
                }
            }
            catch (Exception ex) { }

            //creazione dei ruoli
            if (!_roleManager.RoleExistsAsync(SD.Role_User).GetAwaiter().GetResult())
            {

                _roleManager.CreateAsync(new IdentityRole(SD.Role_Admin)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(SD.Role_User)).GetAwaiter().GetResult();


                //se non sono stati creati i ruoli creamo un utente admin

                _userManager.CreateAsync(new AppUser
                {
                    UserName = "admin@gmail.com",
                    Email = "admin@gmail.com",
                    Name = "Admin",
                    PhoneNumber = " 1234567890",
                }, "Admin.1").GetAwaiter().GetResult();



                _db.Status.Add(new Status
                {
                    Name = "not started",
                });
                _db.SaveChanges();
                _db.Status.Add(new Status
                {
                    Name = "in progress",
                });
                _db.SaveChanges();
                _db.Status.Add(new Status
                {
                    Name = "completed",
                });
                _db.SaveChanges();



                _db.Priority.Add(new Priority
                {
                    Name = "low",
                });
                _db.SaveChanges();
                _db.Priority.Add(new Priority
                {
                    Name = "medium",
                });
                _db.SaveChanges();
                _db.Priority.Add(new Priority
                {
                    Name = "high",
                });
                _db.SaveChanges();



                AppUser user = _db.AppUsers.FirstOrDefault(u => u.Email == "admin@gmail.com");

                _userManager.AddToRoleAsync(user, SD.Role_Admin).GetAwaiter().GetResult();
            }
            return;
        }
    }
}
