

using TaskManager.Models;

namespace TaskManager.Services.Interfaces
{
    public interface IHistoryService
    {
        IEnumerable<History> GetAllByTaskId(int taskId);
    }
}
