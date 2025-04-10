
using TaskManager.DataAccess.Interfaces;
using TaskManager.Models;

namespace TaskManager.DataAccess.Repositories
{
    public class StatusRepository : GenericRepository<Status>,IStatusRepository
    {
        private readonly AppDbContext _db;
        public StatusRepository(AppDbContext db) : base(db)
        {
            _db = db;
        }
    }
}
