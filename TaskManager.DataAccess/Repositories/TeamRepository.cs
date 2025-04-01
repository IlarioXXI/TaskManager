using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.DataAccess.Entities;
using TaskManager.DataAccess.Repositories.Interfaces;

namespace TaskManager.DataAccess.Repositories
{
    public class TeamRepository : GenericRepository<Team>, ITeamRepository
    {
        private readonly AppDbContext _db;
        public TeamRepository(AppDbContext db) : base(db)
        {
            _db = db;
        }
        public void UpdateTeamInUsers(Team team, List<string> selectedUserIds)
        {
            var currentTeam = _db.Teams.Include(u => u.Users).FirstOrDefault(t => t.Id == team.Id);
            var currentUsersIds = currentTeam.Users.Select(u => u.Id).ToList();

            if (selectedUserIds == null)
            {
                currentTeam.Users.Clear();
            }
            else
            {
                foreach (var userId in selectedUserIds)
                {
                    if (!currentUsersIds.Contains(userId))
                    {
                        var user = _db.AppUsers.Find(userId);
                        if (user != null)
                        {
                            currentTeam.Users.Add(user);
                        }
                    }
                }

                foreach (var userId in currentUsersIds)
                {
                    if (!selectedUserIds.Contains(userId))
                    {
                        var userToRemove = currentTeam.Users.FirstOrDefault(u => u.Id == userId);
                        if (userToRemove != null)
                        {
                            currentTeam.Users.Remove(userToRemove);
                        }
                    }
                }
            }

            currentTeam.Name = team.Name;
            currentTeam.TaskItems = team.TaskItems;
        }
        public void Update(Team team)
        {
            _db.Update(team);
        }
    }
}
