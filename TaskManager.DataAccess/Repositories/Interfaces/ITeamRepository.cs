
using TaskManager.Models;

namespace TaskManager.DataAccess.Repositories.Interfaces
{
    public interface ITeamRepository : IGenericRepository<Team>
    {
        void UpdateTeamInUsers(Team team, List<string>? selectedUserIds);

    }
}
