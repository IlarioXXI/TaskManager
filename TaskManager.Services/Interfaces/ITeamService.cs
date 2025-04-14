using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Models;

namespace TaskManager.Services.Interfaces
{
    public interface ITeamService
    {
        IEnumerable<Team> GetAll();
        IEnumerable<Team> GetAllMyTeams();
        Team Upsert(Team team);
        bool Delete(int id);
    }
}
