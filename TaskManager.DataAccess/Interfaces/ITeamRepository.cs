using TaskManager.Models;

namespace TaskManager.DataAccess.Interfaces
{
    public interface ITeamRepository : IGenericRepository<Team>
    {
        void UpdateTeamInUsers(Team team, List<string>? selectedUserIds);

    }
}
