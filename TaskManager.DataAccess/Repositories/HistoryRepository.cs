
using TaskManager.DataAccess.Entities;
using TaskManager.DataAccess.Repositories.Interfaces;

namespace TaskManager.DataAccess.Repositories
{
    public class HistoryRepository : GenericRepository<History>, IHistoryRepository
    {
        public HistoryRepository(AppDbContext db) : base(db)
        {
        }
    }
}
