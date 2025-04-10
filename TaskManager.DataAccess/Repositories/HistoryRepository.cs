
using TaskManager.DataAccess.Interfaces;
using TaskManager.Models;

namespace TaskManager.DataAccess.Repositories
{
    public class HistoryRepository : GenericRepository<History>, IHistoryRepository
    {
        public HistoryRepository(AppDbContext db) : base(db)
        {
        }
    }
}
